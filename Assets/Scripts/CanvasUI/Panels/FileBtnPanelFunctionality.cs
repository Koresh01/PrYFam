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
    /// Скрипт отвечает за панель, появляющуюся при нажатии на кнопку "ФАЙЛ"
    /// </summary>
    public class FileBtnPanelFunctionality : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] private FamilyService familyService;
        [SerializeField] private TreeTraversal treeTraversal;

        [Header("Кнопки:")]
        [Tooltip("Кнопка экспорта древа в .json")]                  [SerializeField] private Button exportButton;
        [Tooltip("Кнопка импорта древа из .json")]                  [SerializeField] private Button importButton;
        [Tooltip("Кнопка сброса древа до начального состояния.")]   [SerializeField] private Button deleteTreeButton;

        private void Awake()
        {
            familyService = GameObject.FindAnyObjectByType<FamilyService>();
            treeTraversal = GameObject.FindAnyObjectByType<TreeTraversal>();
        }

        private void OnEnable()
        {
            exportButton.onClick.AddListener(() => StartExport());
            importButton.onClick.AddListener(() => StartImport());
            deleteTreeButton.onClick.AddListener(() => RefreshTree());
        }

        private void OnDisable()
        {
            exportButton.onClick.RemoveAllListeners();
            importButton.onClick.RemoveAllListeners();
            deleteTreeButton.onClick.RemoveAllListeners();
        }


        #region АСИНХРОННЫЙ ЭКСПОРТ
        private async void StartExport()
        {
            WarningPanelsController.ShowPanel("Загрузка");

            // Ждем хотя бы 100мс(несколько кадров) перед вызовом блокирующего диалога
            await Task.Delay(100);

            // Запускаем экспорт в отдельном потоке, но диалог выбора файла будет в основном потоке
            await ExportFamilyTreeAsync();

            WarningPanelsController.ClosePanel("Загрузка"); // Отключаем панель после завершения экспорта
            Debug.Log("Export finished!");
        }
        private async Task ExportFamilyTreeAsync()
        {
            // Шаг 1: Получаем данные семейного древа (БЕЗ ProfilePicture) 
            List<MemberData> members = familyService.familyData.GetAllMembers().Select(member => new MemberData
            {
                UniqueId = member.UniqueId,
                FirstName = member.FirstName,
                LastName = member.LastName,
                MiddleName = member.MiddleName,
                ProfilePicture = null, // Пока оставляем пустым, обработаем позже
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

            // Шаг 2: Обрабатываем ProfilePicture в главном потоке
            foreach (var member in members)
            {
                var originalMember = familyService.familyData.GetAllMembers().FirstOrDefault(m => m.UniqueId == member.UniqueId);
                if (originalMember != null)
                {
                    member.ProfilePicture = SpriteConverter.SpriteToString(originalMember.ProfilePicture);
                }
                await Task.Yield(); // Позволяем UI не зависать
            }

            // Шаг 3: Сериализация в фоновом потоке
            string json = await Task.Run(() => JsonUtility.ToJson(new FamilyTreeData { Members = members, Relationships = relationships }, true));

            // Шаг 4: Диалог сохранения (в главном потоке)
#if UNITY_ANDROID
            string path = Path.Combine(Application.persistentDataPath, "FamilyTree.json");
            await File.WriteAllTextAsync(path, json);
            NativeFilePicker.ExportFile(path);
            Debug.Log($"Family tree exported and shared: {path}");
#else 
            string path = StandaloneFileBrowser.SaveFilePanel("Save Family Data", "", "FamilyTree", "json");
            if (!string.IsNullOrEmpty(path))
            {
                await File.WriteAllTextAsync(path, json);
                Debug.Log($"Family tree exported to {path}");
            }
#endif
        }
        #endregion

        #region АСИНХРОННЫЙ ИМПОРТ
        private async void StartImport()
        {
            WarningPanelsController.ShowPanel("Загрузка");

            // Ждем хотя бы 100мс(несколько кадров) перед вызовом блокирующего диалога
            await Task.Delay(100);

            await ImportFamilyTreeAsync();
            WarningPanelsController.ClosePanel("Загрузка");
            Debug.Log("Import finished!");
        }

        private async Task ImportFamilyTreeAsync()
        {
            string path = "";
#if UNITY_ANDROID
            path = await PickFileAsync();
#else
            // Обновление UI
            path = StandaloneFileBrowser.OpenFilePanel("Open Family Data", "", "json", false).FirstOrDefault();
#endif
            if (!string.IsNullOrEmpty(path))
            {
                await ProcessImportedFile(path);
            }
            else
            {
                Debug.LogWarning("No file selected.");
            }
        }

        private Task<string> PickFileAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            NativeFilePicker.PickFile((path) =>
            {
                tcs.SetResult(path);
            }, new[] { "application/json" });

            return tcs.Task;
        }

        private async Task ProcessImportedFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
                return;
            }

            string json = await File.ReadAllTextAsync(path);
            var serializedTree = JsonUtility.FromJson<FamilyTreeData>(json);

            familyService.familyData.DestroyTree();

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
                
                
                await Task.Yield(); // говорит Unity: "Подожди один кадр перед выполнением следующей строки кода."
            }

            foreach (var relationshipData in serializedTree.Relationships)
            {
                if (createdMembers.TryGetValue(relationshipData.FromId, out var from) &&
                    createdMembers.TryGetValue(relationshipData.ToId, out var to))
                {
                    var relationship = (Relationship)System.Enum.Parse(typeof(Relationship), relationshipData.RelationshipType);
                    familyService.AddBidirectionalRelationship(from, to, relationship);


                    await Task.Yield(); // говорит Unity: "Подожди один кадр перед выполнением следующей строки кода."
                }
            }

            var root = createdMembers.Values.FirstOrDefault();
            if (root != null)
            {
                // Рисование дерева после загрузки данных
                treeTraversal.ReDrawTree(root, Vector2.zero);
            }
            Debug.Log("Конец переотрисовки древа");
        }
        #endregion

        #region Сброс древа до первоначального состояния
        /// <summary>
        /// Сбрасывает древо до первоначального состояния.
        /// </summary>
        void RefreshTree()
        {
            familyService.familyData.DestroyTree();
            CreateNewFamilyTree();

        }

        /// <summary>
        /// Создает новое семейное древо. (как при старте прогрммы)
        /// </summary>
        private void CreateNewFamilyTree()
        {
            GameObject go1 = familyService.CreateCard();
            GameObject go2 = familyService.CreateCard();

            Member from = go1.GetComponent<Member>();
            Member to = go2.GetComponent<Member>();


            familyService.AddConnection(from, to, Relationship.ToHalf);
            treeTraversal.ReDrawTree(from, new Vector2(0, 0));


            // Debug.Log("Family tree was created.");
        }
        #endregion
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
