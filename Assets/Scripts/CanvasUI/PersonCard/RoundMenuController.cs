using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
{
    /// <summary>
    /// Скрипт висит на карточке персонажа и отвечает 
    /// за логику появления кругового меню над карточкой 
    /// члена семьи.
    /// </summary>
    class RoundMenuController : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] FamilyService familyService;

        [Header("Кнопки:")]
        [SerializeField] Button enter;
        [Tooltip("Кнопка включения панели детальной информации члена семьи.")]
        [SerializeField] Button showDetailedPanel;

        [Header("Круговое меню:")]
        [SerializeField] GameObject roundMenu;
        [SerializeField] RoundMenuStatus CardStatus = RoundMenuStatus.NotPressedYet;

        private void Awake()
        {
            // Префабам нельзя прокидывать сущьности со сцены.
            familyService = GameObject.FindAnyObjectByType<FamilyService>();
        }
        private void OnEnable()
        {
            enter.onClick.AddListener(() =>
            {
                Member root = transform.GetComponent<Member>();
                Handle(root);
            });
            showDetailedPanel.onClick.AddListener(() =>
            {
                roundMenu.SetActive(false);
            });
        }

        private void OnDisable()
        {
            enter.onClick.RemoveAllListeners();
            showDetailedPanel.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Реализует логику появления кругового меню около карточки персонажа.
        /// </summary>
        /// <param name="root"></param>
        void Handle(Member root)
        {
            List<GameObject> personCards = familyService.familyData.GetAllPersonCards();
            foreach (GameObject card in personCards)
            {
                Member memeber = card.GetComponent<Member>();

                if (memeber == root)    // Если полученная карточка члена семьи - Является той относительно которой рисуется всё древо.
                {
                    if (CardStatus == RoundMenuStatus.NotPressedYet)
                    {
                        CardStatus = RoundMenuStatus.FirstPressed;
                    }
                    else if (CardStatus == RoundMenuStatus.FirstPressed)
                    {
                        CardStatus = RoundMenuStatus.SecondPressed;
                        roundMenu.SetActive(true);
                    }
                    else if (CardStatus == RoundMenuStatus.SecondPressed)
                    {
                        CardStatus = RoundMenuStatus.FirstPressed;
                        roundMenu.SetActive(false);
                    }
                }
                if (memeber != root)    // Если полученная карточка члена семьи - не является той относительно которой рисуется всё древо.
                {
                    RoundMenuController roundMenuController = card.GetComponent<RoundMenuController>();
                    // Скрываем круговое меню:
                    roundMenuController.roundMenu.SetActive(false);
                    // Обнуляем статус кругового меню(как будто на эту карточку не нажали ниразу)
                    roundMenuController.CardStatus = RoundMenuStatus.NotPressedYet;

                }
            }
        }
    }
}
