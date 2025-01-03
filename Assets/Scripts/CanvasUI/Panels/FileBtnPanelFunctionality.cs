using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
{
    /// <summary>
    /// Скрпит висит на панели кнопки "файл", и реализует логику импорта экспорта древа из файла в файл.
    /// </summary>
    class FileBtnPanelFunctionality : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] FamilyService familyService;
        [SerializeField] TreeTraversal treeTraversal;
        [SerializeField] CanvasView canvasView;

        [Header("Кнопки:")]
        [SerializeField] Button export;
        [SerializeField] Button import;


        void Awake()
        {
            // Префабам нельзя прокидывать сущьности со сцены.
            familyService = GameObject.FindAnyObjectByType<FamilyService>();
            treeTraversal = GameObject.FindAnyObjectByType<TreeTraversal>();
            canvasView = GameObject.FindAnyObjectByType<CanvasView>();
        }
        void OnEnable()
        {
            export.onClick.AddListener(() =>
            {
                Import();
            });
            import.onClick.AddListener(() =>
            {
                Export();
            });
        }
        void OnDisable()
        {
            export.onClick.RemoveAllListeners();
            import.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Экспортирует древо в .json
        /// </summary>
        void Export()
        {
            FamilyData familyData = familyService.familyData;
            List<MembersConnection> relationships = familyData.relationships;

            foreach (var relationship in relationships)
            {
                Member from = relationship.From;
                Member to = relationship.To;
                Relationship r = relationship.Relationship;

                string FirstName1 = from.FirstName;
                string LastName1 = from.LastName;
                string MiddleName1 = from.MiddleName;
                string ProfilePicture1 = SpriteConverter.SpriteToString(from.ProfilePicture);
                string DateOfBirth1 = from.DateOfBirth;
                string PlaceOfBirth1 = from.PlaceOfBirth;
                string Biography1 = from.Biography;

                string FirstName2 = to.FirstName;
                string LastName2 = to.LastName;
                string MiddleName2 = to.MiddleName;
                string ProfilePicture2 = SpriteConverter.SpriteToString(to.ProfilePicture);
                string DateOfBirth2 = to.DateOfBirth;
                string PlaceOfBirth2 = to.PlaceOfBirth;
                string Biography2 = to.Biography;

                // как то записать в файл json надо.
            }
        }


        /// <summary>
        /// Импортирует древо из .json
        /// </summary>
        void Import()
        {
            
        }
    }
}
}
