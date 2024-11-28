using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Устанавливает ноды в правильные координаты. Это главный скрипт отвечающий за отрисовку.
    /// </summary>
    public class TreeTraversal : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] private FamilyService familyService;

        [Header("Отступы:")]
        [SerializeField] float horizontalSpacing;
        [SerializeField] float verticalSpacing;
        [SerializeField] float halfCardOffset;

        [Header("Время за которое карточки встают в свои позиции:")]
        [SerializeField] float duration;

        [Header("Точные координаты каждой карточки:")]
        [SerializeField] Dictionary<Member, Vector2> coordinates;

        public void ReDrawTree(Member root, Vector2 basePosition)
        {
            Debug.Log("Перерисовка древа на основе FamilyData.");
            // 1.
            FadeCards();

            // 2. Рассчет точных координат:
            coordinates = Algorithms.Singleton.ReCalculate(
                root,
                familyService,
                basePosition,
                horizontalSpacing,
                verticalSpacing,
                halfCardOffset
            );

            // 3.
            hardRepositionCards();  // - жестко
            //StartCoroutine(SmoothRepositionCards()); // - мягко
            
        }
        /// <summary> Получаем карточки всех персонажей и тушим их. </summary>
        private void FadeCards() {
            List<GameObject> personCards = familyService.familyData.GetAllPersonCards();
            foreach (var card in personCards)
                card.SetActive(false);
        }

        /// <summary> Применяем позиции к RectTransform каждого члена </summary>
        private void hardRepositionCards() {
            foreach (var member in coordinates.Keys)
            {
                var memberGO = member.gameObject;
                
                // Высвечиваем карточку(раз нам удалось нашим направленынм DFS её посетить при перессчёте координат)
                memberGO.SetActive(true);

                if (memberGO.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    rectTransform.anchoredPosition = coordinates[member];
                }
            }
        }

        /// <summary Плавно меняет позиции всех карточек. </summary>
        private IEnumerator SmoothRepositionCards()
        {
            // Для каждой карточки плавно меняем позицию
            foreach (var member in coordinates.Keys)
            {
                var memberGO = member.gameObject;

                // Высвечиваем карточку(раз нам удалось нашим направленынм DFS её посетить при перессчёте координат)
                memberGO.SetActive(true);

                if (memberGO.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    Vector2 targetPosition = coordinates[member];
                    yield return StartCoroutine(SmoothMove(rectTransform, targetPosition, duration)); // Плавное движение, 0.5 секунд для анимации
                }
            }
        }
        private IEnumerator SmoothMove(RectTransform rectTransform, Vector2 targetPosition, float duration)
        {
            Vector2 startPosition = rectTransform.anchoredPosition;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Линейная интерполяция между стартовой и целевой позицией
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Убедимся, что конечная позиция установлена точно
            rectTransform.anchoredPosition = targetPosition;
        }
    }
}
