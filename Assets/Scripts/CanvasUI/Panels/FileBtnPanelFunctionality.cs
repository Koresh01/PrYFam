using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SFB; // Для Standalone File Browser (Windows) -> проводник windows
using System.Threading.Tasks;
//using NativeFilePickerNamespace; // Для UnityNativeFilePicker (Android) -> проводник android

namespace PrYFam
{
    /// <summary>
    /// Скрипт для импорта и экспорта семейного древа.
    /// </summary>
    public class FileBtnPanelFunctionality : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] private FamilyService familyService;
        [SerializeField] private TreeTraversal treeTraversal;

        [Header("Кнопки:")]
        [SerializeField] private Button exportButton;
        [SerializeField] private Button importButton;

        private void Awake()
        {
            familyService = GameObject.FindAnyObjectByType<FamilyService>();
            treeTraversal = GameObject.FindAnyObjectByType<TreeTraversal>();
        }

        private void OnEnable()
        {
            exportButton.onClick.AddListener(() => StartExport());
            importButton.onClick.AddListener(ImportFamilyTree);
        }

        private void OnDisable()
        {
            exportButton.onClick.RemoveAllListeners();
            importButton.onClick.RemoveAllListeners();
        }

        private async void StartExport()
        {
            WarningPanelsController.ShowPanel("Загрузка");

            // Запускаем экспорт в отдельном потоке, но диалог выбора файла будет в основном потоке
            await ExportFamilyTreeAsync();

            WarningPanelsController.ClosePanel("Загрузка"); // Отключаем панель после завершения экспорта
            Debug.Log("Export finished!");
        }

        private async Task ExportFamilyTreeAsync()
        {
            // Получаем данные семейного древа (ТОЛЬКО в главном потоке!)
            List<MemberData> members = familyService.familyData.GetAllMembers().Select(member => new MemberData
            {
                UniqueId = member.UniqueId,
                FirstName = member.FirstName,
                LastName = member.LastName,
                MiddleName = member.MiddleName,
                ProfilePicture = SpriteConverter.SpriteToString(member.ProfilePicture), // Теперь выполняется в главном потоке
                DateOfBirth = member.DateOfBirth,
                PlaceOfBirth = member.PlaceOfBirth,
                Biography = member.Biography
            }).ToList();

            List<RelationshipData> relationships = familyService.familyData.relationships.Select(rel => new RelationshipData
            {
                FromId = rel.From.UniqueId,
                ToId = rel.To.UniqueId,
                RelationshipType = rel.Relationship.ToString()
            }).ToList();

            // Запускаем сериализацию в фоновом потоке
            string json = await Task.Run(() => JsonUtility.ToJson(new FamilyTreeData { Members = members, Relationships = relationships }, true));

            // Сохранение файла (Unity API использовать нельзя, но можно писать в файл)
#if UNITY_ANDROID
            string path = Path.Combine(Application.persistentDataPath, "FamilyTree.json");
            await File.WriteAllTextAsync(path, json);
            NativeFilePicker.ExportFile(path);
            Debug.Log($"Family tree exported and shared: {path}");
#else
            await Task.Yield();  // Вернуться в главный поток для диалога сохранения
            string path = StandaloneFileBrowser.SaveFilePanel("Save Family Data", "", "FamilyTree", "json");
            if (!string.IsNullOrEmpty(path))
            {
                await File.WriteAllTextAsync(path, json);
                Debug.Log($"Family tree exported to {path}");
            }
#endif
        }

        /// <summary>
        /// Импортирует семейное древо из JSON.
        /// </summary>
        private void ImportFamilyTree()
        {
#if UNITY_ANDROID
            // Используем UnityNativeFilePicker для выбора файла на Android
            NativeFilePicker.PickFile(
                (path) => // Передаём в метод колбэк-функцию, которая будет вызвана после выбора файла
                {
                    // Проверяем, выбрал ли пользователь файл
                    if (string.IsNullOrEmpty(path)) // Если путь пустой, значит файл не был выбран
                    {
                        Debug.LogWarning("No file selected."); // Выводим предупреждение в консоль
                        return; // Завершаем выполнение функции
                    }

                    // Если файл выбран, выводим его путь в консоль
                    Debug.Log($"Selected file: {path}");

                    // Передаём путь к выбранному файлу в метод для обработки
                    ProcessImportedFile(path);
                },
                new[] { "application/json" } // Указываем фильтр MIME-типов, чтобы показывались только файлы формата JSON
            );
#else
            // На Windows используем SFB для выбора файла
            string path = StandaloneFileBrowser.OpenFilePanel("Open Family Data", "", "json", false).FirstOrDefault();
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log($"Selected file: {path}");
                ProcessImportedFile(path);
            }
#endif
        }

        /// <summary>
        /// Обрабатывает импортированный файл.
        /// </summary>
        /// <param name="path">Путь к JSON файлу.</param>
        private void ProcessImportedFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
                return;
            }

            string json = File.ReadAllText(path);
            var serializedTree = JsonUtility.FromJson<FamilyTreeData>(json);

            // Удаляем старое древо
            familyService.familyData.DestroyTree();

            // Создаём членов семьи
            var createdMembers = new Dictionary<string, Member>();
            foreach (var memberData in serializedTree.Members)
            {
                var member = familyService.CreateCard().GetComponent<Member>();
                member.UniqueId = memberData.UniqueId;
                member.FirstName = memberData.FirstName;
                member.LastName = memberData.LastName;
                member.MiddleName = memberData.MiddleName;
                member.ProfilePicture = SpriteConverter.StringToSprite(memberData.ProfilePicture);
                member.DateOfBirth = memberData.DateOfBirth;
                member.PlaceOfBirth = memberData.PlaceOfBirth;
                member.Biography = memberData.Biography;

                createdMembers[memberData.UniqueId] = member;
            }

            // Устанавливаем отношения
            foreach (var relationshipData in serializedTree.Relationships)
            {
                if (createdMembers.TryGetValue(relationshipData.FromId, out var from) &&
                    createdMembers.TryGetValue(relationshipData.ToId, out var to))
                {
                    var relationship = (Relationship)System.Enum.Parse(typeof(Relationship), relationshipData.RelationshipType);
                    familyService.AddBidirectionalRelationship(from, to, relationship);
                }
            }

            // Отрисовываем дерево
            var root = createdMembers.Values.FirstOrDefault();
            if (root != null) treeTraversal.ReDrawTree(root, Vector2.zero);
        }
    }








    /// <summary>
    /// Класс для сериализации семейного древа.
    /// </summary>
    [System.Serializable]
    public class FamilyTreeData
    {
        public List<MemberData> Members;
        public List<RelationshipData> Relationships;
    }

    [System.Serializable]
    public class MemberData
    {
        public string UniqueId;
        public string FirstName;
        public string LastName;
        public string MiddleName;
        public string ProfilePicture;
        public string DateOfBirth;
        public string PlaceOfBirth;
        public string Biography;
    }

    [System.Serializable]
    public class RelationshipData
    {
        public string FromId;
        public string ToId;
        public string RelationshipType;
    }
}
