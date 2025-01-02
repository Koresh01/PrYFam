using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace PrYFam
{
    /// <summary>
    /// Этот скрипт висит на карточке персонажа,
    /// и отвечает за логику всех кнопок на этой карточке.
    /// </summary>
    public class CardView : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] FamilyService familyService;
        [SerializeField] TreeTraversal treeTraversal;
        [SerializeField] CanvasView canvasView;

        [Header("Поле ФИО на лицевой стороне карточки.")]
        public TextMeshProUGUI FIO;

        [Header("Круговое меню:")]
        [SerializeField] GameObject roundMenu;
        private ActivePersonStatus activePersonStatus = ActivePersonStatus.Disabled;

        [Header("Кнопки:")]
        [SerializeField] Button enter;
        [SerializeField] Button addParent;
        [SerializeField] Button addChild;
        [SerializeField] Button addHalf;
        [Tooltip("Кнопка удаления члена семьи.")]
        [SerializeField] Button delete;
        [Tooltip("Кнопка смены жены.")]
        [SerializeField] Button changeWife;

        [Tooltip("Кнопка включения панели детальной информации члена семьи.")]
        [SerializeField] Button showDetailedPanel;

        [Header("Каёмка для активной рамки:")]
        public GameObject DefaultBoundImage;
        public GameObject ActiveBoundImage;

        private void Awake()
        {
            // Префабам нельзя прокидывать сущьности со сцены.
            familyService = GameObject.FindAnyObjectByType<FamilyService>();
            treeTraversal = GameObject.FindAnyObjectByType<TreeTraversal>();
            canvasView = GameObject.FindAnyObjectByType<CanvasView>();
        }

        private void OnEnable()
        {
            enter.onClick.AddListener(() => {
                if (activePersonStatus == ActivePersonStatus.Disabled)
                {
                    activePersonStatus = ActivePersonStatus.DisabledRoundMenu;
                    roundMenu.SetActive(false);
                }

                else if (activePersonStatus == ActivePersonStatus.DisabledRoundMenu)
                {
                    activePersonStatus = ActivePersonStatus.ShowRoundMenu;
                    roundMenu.SetActive(true);
                }

                else if (activePersonStatus == ActivePersonStatus.ShowRoundMenu)
                {
                    activePersonStatus = ActivePersonStatus.Disabled;
                    roundMenu.SetActive(false);
                }

                HandleTreeRedraw();
            });
            addParent.onClick.AddListener(() => {
                familyService.AddNewMember(transform.gameObject, Relationship.ToParent);
                HandleTreeRedraw();
            });
            addChild.onClick.AddListener(() => {
                familyService.AddNewMember(transform.gameObject, Relationship.ToChild);
                HandleTreeRedraw();
            });
            addHalf.onClick.AddListener(() => {
                familyService.AddNewMember(transform.gameObject, Relationship.ToHalf);
                HandleTreeRedraw();
            });

            // отобразить панель детальной информации
            showDetailedPanel.onClick.AddListener(() =>
            {
                HandleTreeRedraw();
                canvasView.ShowDetailedPersonPanel();

                roundMenu.SetActive(false);
            });

            // Удаление члена семьи:
            delete.onClick.AddListener(() =>
            {
                Member cur = transform.GetComponent<Member>();

                // Если удаляемый член семьи является крайним(листовым)
                if (familyService.IsLeaf(cur))
                {
                    familyService.DeletePerson(cur);
                    Destroy(gameObject);
                }
            });

            // Смена жены
            changeWife.onClick.AddListener(() =>
            {
                SelectedWifeController wifeController = transform.GetComponent<SelectedWifeController>();   // чтоюы вытащить индекс нужной жены
                Member current = transform.GetComponent<Member>();              // член семь на котором произошло нажатие
                List<Member> wifes = familyService.GetHalfMembers(current);     // список всех его жён
                wifeController.NextWife(wifes);     // Смещаем индекс

                HandleTreeRedraw();
            });
        }

        private void OnDisable()
        {
            RemoveAllListeners(enter, addChild, addParent, addHalf, delete, showDetailedPanel, changeWife);
        }

        /// <summary>
        /// Вспомогательный метод для обновления дерева
        /// </summary>
        public void HandleTreeRedraw()
        {
            Vector2 curPos = transform.GetComponent<RectTransform>().anchoredPosition;
            Member cur = transform.GetComponent<Member>();
            treeTraversal.ReDrawTree(cur, curPos);
        }

        // Удаление всех слушателей с кнопок
        private void RemoveAllListeners(params Button[] buttons)
        {
            foreach (var button in buttons)
            {
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                }
            }
        }
    }
}
