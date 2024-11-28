using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Класс для выполнения алгоритмов работы с семейным деревом.
    /// </summary>
    public class Algorithms
    {
        private static Algorithms _singleton;

        /// <summary>
        /// Геттер для получения единственного экземпляра класса.
        /// Создает экземпляр при первом обращении.
        /// </summary>
        public static Algorithms Singleton
        {
            get => _singleton == null ? new Algorithms() : _singleton;
            private set => _singleton = value; // Закрытый сеттер для внутреннего использования
        }
        private Member root;
        private FamilyService familyService;
        private Vector2 basePosition;
        private float horizontalSpacing, verticalSpacing, halfCardOffset;
        private Dictionary<Member, Vector2> coordinates;

        # region main
        /// <summary> Инициализирует стартовые параметры для переотрисовки. </summary>
        private void Initialize(Member root, FamilyService familyService, Vector2 basePosition, float horizontalSpacing, float verticalSpacing, float halfCardOffset)
        {
            this.root = root;
            this.familyService = familyService;
            this.basePosition = basePosition;
            this.horizontalSpacing = horizontalSpacing;
            this.verticalSpacing = verticalSpacing;
            this.halfCardOffset = halfCardOffset;

            this.coordinates = new Dictionary<Member, Vector2>();
        }
        /// <summary> ,,, </summary>
        public Dictionary<Member, Vector2> ReCalculate(Member root, FamilyService familyService, Vector2 basePosition, float horizontalSpacing, float verticalSpacing, float halfCardOffset)
        {
            Initialize(root, familyService, basePosition, horizontalSpacing, verticalSpacing, halfCardOffset);
            CalculateNodeCoordinates();
            return coordinates;
        }
        #endregion
        #region NodeCoordinates
        
        /// <summary> Вычисляет координаты вершин, но только тех, В КОТОРЫЕ ПОПАДАЕТ ПРИ ОБХОДЕ </summary>
        private void CalculateNodeCoordinates()
        {
            // Вычисляем координаты для двух направлений
            CalculateNodeCoordinatesDirectionatly(root, basePosition.x, basePosition.y, Direction.Up);
            CalculateNodeCoordinatesDirectionatly(root, basePosition.x, basePosition.y, Direction.Down);
        }

        /// <summary> Рекурсивно назначает координаты узлам дерева, идя в заданном направлении: вверх или вниз. </summary>
        private void CalculateNodeCoordinatesDirectionatly(Member current, float x, float y, Direction direction)
        {
            if (current == null)
                return;

            
            coordinates[current] = new Vector2(x, y);

            if (familyService.hasHalf(current)) {
                List<Member> halfs = familyService.GetRelatedMembers(current, Relationship.ToHalf);
                for (int i = 1; i <= halfs.Count; i++) {
                    Member concreteHalf = halfs[i-1];
                    concreteHalf.gameObject.transform.SetAsFirstSibling();  // Переместить элемент на задний план
                    coordinates[concreteHalf] = new Vector2(x,y) + i * new Vector2(halfCardOffset, halfCardOffset);
                }
            }
                

            // Получаем связанных членов и их условные ширины
            var relatedMembers = direction == Direction.Down
                ? familyService.GetRelatedMembers(current, Relationship.ToChild)
                : familyService.GetRelatedMembers(current, Relationship.ToParent);

            // Получаем условные ширинЫ подветвей в направлении которых движемся
            var subtreeWidths = CalculateSubtreeWidths(current, direction);

            // Обрабатываем связанных членов и назначаем координаты
            ProcessRelatedMembers(
                relatedMembers,
                subtreeWidths,
                x, y,
                direction);
        }
        /// <summary> Обрабатывает связанных членов узла, вычисляет их координаты и вызывает рекурсивное назначение. </summary>
        private void ProcessRelatedMembers(
            List<Member> relatedMembers,
            List<int> subtreeWidths,
            float x, float y,
            Direction direction)
        {
            // Смещение по вертикали зависит от направления обхода
            float offsetY = direction == Direction.Down ? -verticalSpacing : verticalSpacing;

            // Итерация по связанным членам
            for (int i = 0; i < relatedMembers.Count; i++)
            {
                Member related = relatedMembers[i];


                float cumulativeWidth = 0;
                for (int k = 0; k < i; k++)
                    cumulativeWidth += subtreeWidths[k];

                // воспроизводим формулу:
                float newY = y + offsetY;
                float newX = x + horizontalSpacing * (-subtreeWidths.Sum() / 2f + cumulativeWidth + subtreeWidths[i] / 2f);

                // Рекурсивно назначаем координаты
                CalculateNodeCoordinatesDirectionatly(related, newX, newY, direction);
            }
        }
        #endregion
        # region MnimWidth
        /// <summary>Вычисляет ширины поддеревьев для каждого из связанных узлов.</summary>
        /// <returns>Список int значений.</returns>
        private List<int> CalculateSubtreeWidths(Member current, Direction direction)
        {
            // Получение списка связанных членов (детей или родителей)
            var relatedMembers = direction == Direction.Down
                ? familyService.GetRelatedMembers(current, Relationship.ToChild)
                : familyService.GetRelatedMembers(current, Relationship.ToParent);

            // Для каждого узла вычисляем условную ширину подветви
            return relatedMembers
                .Select(member => CalculateMnimWidth(member, direction))
                .ToList(); // Преобразуем результат в список
        }

        /// <summary> Вычисляет условную ширину подветви относительно указанного корня, идя вверх/вниз. </summary>
        /// <returns>Условная ширина подветви.</returns>
        private int CalculateMnimWidth(Member root, Direction direction)
        {
            if (root == null || familyService == null) return 0;

            var visited = new HashSet<Member>(); // Множество для отслеживания посещённых узлов

            int Traverse(Member current)
            {
                if (current == null || visited.Contains(current))
                    return 0;

                visited.Add(current);

                // Получение связанных членов в зависимости от направления обхода
                var relatedMembers = direction == Direction.Down
                    ? familyService.GetRelatedMembers(current, Relationship.ToChild)
                    : familyService.GetRelatedMembers(current, Relationship.ToParent);

                // Условную ширину для карточки жены перессчитывать ненадо, т.к. я решил делать карточку жены позади.
                // ...

                // Если нет связанных узлов, ширина равна 1
                if (!relatedMembers.Any())
                    return 1;

                // Рекурсивный обход связанных узлов
                return relatedMembers.Sum(member => Traverse(member));
            }

            return Traverse(root);
        }
        #endregion
    }
}
