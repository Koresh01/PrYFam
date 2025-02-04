using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace PrYFam
{
    /// <summary>
    /// Устанавливает ноды в правильные координаты. Это главный скрипт отвечающий за отрисовку.
    /// </summary>
    public class TreeTraversal : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] private FamilyService familyService;
        [SerializeField] private LinesController linesController;
        [SerializeField] private DetailedPersonPanel deltailedPersonPanel;

        [Header("Отступы:")]
        public float CardHeight;
        public float CardWidth;
        [Range(0f, 400f)] public float GlobalTreeOffset;    // Расстояние между 2умя карточками.

        [Header("Точные координаты каждой карточки:")]
        [SerializeField] Dictionary<Member, Vector2> coordinates;


        private void Awake()
        {
            CardHeight = familyService.personCardPrefab.GetComponent<RectTransform>().sizeDelta.y;
            CardWidth = familyService.personCardPrefab.GetComponent<RectTransform>().sizeDelta.x;

            CardWidth += GlobalTreeOffset;
            CardHeight += GlobalTreeOffset;
        }

        public void ReDrawTree(Member root, Vector2 basePosition)
        {
            // 1.
            FadeCards();
            FadeSelectedCardBound(root);

            // 2. Рассчет точных координат:
            coordinates = Algorithms.Singleton.ReCalculate(
                root,
                familyService,
                basePosition,
                CardWidth,
                CardHeight
            );

            // 3.
            hardRepositionCards();  // - жестко

            // 4. Отрисовка линий
            ReDrawLines();
        }

        /// <summary> Перерисовывает все линии между людьми. </summary>
        private void ReDrawLines()
        {
            linesController.delAllLines();
            foreach (var pair in familyService.familyData.relationships)
            {
                Member from = pair.From;
                Member to = pair.To;
                Relationship relationship = pair.Relationship;

                if (!(coordinates.ContainsKey(from) && coordinates.ContainsKey(to))) continue;

                if (relationship == Relationship.ToChild) {
                    if (familyService.hasHalf(from))
                    {
                        Member half = familyService.GetSelectedHalf(from);
                        linesController.DrawMergedLine(from.gameObject, half.gameObject, to.gameObject);
                    }
                    if (!familyService.hasHalf(from))
                    {
                        linesController.DrawPolyLine(from.gameObject, to.gameObject);
                    }
                }

                
                // Заглушка чтоб у нас рисовалась линия к жене, если дети пока что НЕ ДОБАВЛЕНЫ.
                if (relationship == Relationship.ToHalf && familyService.GetChildMembers(from).Count == 0)
                {
                    linesController.DrawLineToHalf(from.gameObject, to.gameObject);
                }
            }
        }
        /// <summary> Получаем карточки всех персонажей и тушим их. </summary>
        private void FadeCards() {
            List<GameObject> personCards = familyService.familyData.GetAllPersonCards();
            foreach (var card in personCards)
            {
                card.SetActive(false);
            }
                
        }
        /// <summary> Тушит все рамки и подсвечивает только над root. </summary>
        private void FadeSelectedCardBound(Member root) {
            List<GameObject> personCards = familyService.familyData.GetAllPersonCards();
            foreach (GameObject card in personCards)
            {
                Member memeber = card.GetComponent<Member>();
                CardView cardView = card.gameObject.GetComponent<CardView>();

                if (memeber == root)
                {
                    cardView.ActiveBoundImage.SetActive(true);
                    cardView.DefaultBoundImage.SetActive(false);
                }
                if (memeber != root)
                {
                    cardView.ActiveBoundImage.SetActive(false);
                    cardView.DefaultBoundImage.SetActive(true);
                }
            }
                
        }

        /// <summary> Применяем позиции к RectTransform каждого члена семьи. </summary>
        public void hardRepositionCards() {
            foreach (Member member in coordinates.Keys)
            {
                var memberGO = member.gameObject;
                
                // Высвечиваем карточку(раз нам удалось нашим направленынм DFS её посетить при перессчёте координат)
                memberGO.SetActive(true);

                if (memberGO.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    rectTransform.anchoredPosition = coordinates[member];
                }
            }

            // Обновляем внешние данные карточки, полагаясь на данные из Member.cs:
            foreach (Member member in coordinates.Keys)
            {
                // Обновляем изображение на карточке
                Transform faceSpriteTransform = member.transform.Find("Environment/Image (Face Sprite)");
                Image faceSpriteImage = faceSpriteTransform.GetComponent<Image>();
                faceSpriteImage.sprite = member.ProfilePicture;

                // А также меняем ФИО на лицевой стороне карточке
                CardView cardView = member.GetComponent<CardView>();
                string FIO = member.LastName + " " + member.FirstName + " " + member.MiddleName;
                if (FIO == "  ")
                    cardView.FIO.text = "ФИО";
                else
                    cardView.FIO.text = FIO;
            }
        }
    }
}
