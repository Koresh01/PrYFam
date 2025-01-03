using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SFB; // Для использования Standalone File Browser

namespace PrYFam
{
    /// <summary>
    /// Скрипт висит на панели кнопки "Файл" и реализует логику импорта и экспорта древа из файла.
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
            exportButton.onClick.AddListener(ExportFamilyTree);
            importButton.onClick.AddListener(ImportFamilyTree);
        }

        private void OnDisable()
        {
            exportButton.onClick.RemoveAllListeners();
            importButton.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Экспортирует семейное древо в JSON.
        /// </summary>
        private void ExportFamilyTree()
        {
            // Получаем данные семейного древа
            FamilyData familyData = familyService.familyData;

            // Формируем объект для сериализации
            var serializedTree = new FamilyTreeData
            {
                // Сначала сохраним всех членов семейного древа.
                Members = familyData.GetAllMembers().Select(member => new MemberData
                {
                    UniqueId = member.UniqueId,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    MiddleName = member.MiddleName,
                    ProfilePicture = SpriteConverter.SpriteToString(member.ProfilePicture),
                    DateOfBirth = member.DateOfBirth,
                    PlaceOfBirth = member.PlaceOfBirth,
                    Biography = member.Biography
                }).ToList(),
                
                // Теперь сохраним все их типы взаимодействий.
                Relationships = familyData.relationships.Select(rel => new RelationshipData
                {
                    FromId = rel.From.UniqueId,
                    ToId = rel.To.UniqueId,
                    RelationshipType = rel.Relationship.ToString()
                }).ToList()
            };

            // Открываем диалог сохранения файла
            string path = StandaloneFileBrowser.SaveFilePanel("Save Family Data", "", "Семейное древо", "json");
            if (string.IsNullOrEmpty(path)) return;

            // Сериализуем данные в JSON и сохраняем в файл
            string json = JsonUtility.ToJson(serializedTree, true);
            File.WriteAllText(path, json);

            Debug.Log($"Family tree exported to {path}");
        }

        /// <summary>
        /// Импортирует семейное древо из JSON.
        /// </summary>
        private void ImportFamilyTree()
        {
            string path = StandaloneFileBrowser.OpenFilePanel("Open Family Data", "", "json", false).FirstOrDefault();
            if (string.IsNullOrEmpty(path)) return;

            string json = File.ReadAllText(path);
            var serializedTree = JsonUtility.FromJson<FamilyTreeData>(json);

            // if (serializedTree == null || serializedTree.Members == null || serializedTree.Relationships == null) return;



            // Избавимся от старого древа.
            familyService.familyData.DestroyTree();
            
            // Сюда будем запоминать, кого уже добавили.
            var createdMembers = new Dictionary<string, Member>();

            // Сначала создаём всех членов семьи():
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

            // Теперь устанавливаем им виды связи:
            foreach (var relationshipData in serializedTree.Relationships)
            {
                if (createdMembers.TryGetValue(relationshipData.FromId, out var from) &&
                    createdMembers.TryGetValue(relationshipData.ToId, out var to))
                {
                    var relationship = (Relationship)System.Enum.Parse(typeof(Relationship), relationshipData.RelationshipType);
                    familyService.AddConnection(from, to, relationship);
                }
            }

            // Отрисовка дерева
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
