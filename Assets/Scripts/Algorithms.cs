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
        private float horizontalSpacing, verticalSpacing;
        private Dictionary<Member, Vector2> coordinates;

        /// <summary>
        /// Инициализирует стартовые параметры для переотрисовки.
        /// </summary>
        private void Initialize(Member root, FamilyService familyService, Vector2 basePosition, float horizontalSpacing, float verticalSpacing)
        {
            this.root = root;
            this.familyService = familyService;
            this.basePosition = basePosition;
            this.horizontalSpacing = horizontalSpacing;
            this.verticalSpacing = verticalSpacing;

            this.coordinates = new Dictionary<Member, Vector2>();
        }
        
        /// <summary>
        /// Перессчитывает координаты карточек.
        /// </summary>
        public Dictionary<Member, Vector2> ReCalculate(Member root, FamilyService familyService, Vector2 basePosition, float horizontalSpacing, float verticalSpacing)
        {
            Initialize(root, familyService, basePosition, horizontalSpacing, verticalSpacing);
            Calculate();
            return coordinates;
        }
        
        
        

        /// <summary>
        /// Вычисляет координаты вершин, но только тех, В КОТОРЫЕ ПОПАДАЕТ ПРИ ОБХОДЕ
        /// </summary>
        private void Calculate()
        {
            // Вычисляем координаты для двух направлений
            CalculateNodeCoordinatesDirectionatly(root, basePosition.x, basePosition.y, Direction.Down);
            CalculateNodeCoordinatesDirectionatly(root, basePosition.x, basePosition.y, Direction.Up);
        }

        /// <summary>
        /// Рекурсивно назначает координаты узлам дерева, идя в заданном направлении: вверх или вниз.
        /// </summary>
        private void CalculateNodeCoordinatesDirectionatly(Member current, float x, float y, Direction direction)
        {
            if (current == null)
                return;

            coordinates[current] = new Vector2(x, y);
            

            // Получаем связанных членов и их условные ширины
            List<Member> relatedMembers = direction == Direction.Down ? familyService.GetRelatedMembers(current, Relationship.ToChild) : familyService.GetRelatedMembers(current, Relationship.ToParent);

            // Получаем условные ширинЫ подветвей в направлении которых движемся
            List<int> subtreeWidths = new List<int>();
            foreach (var member in relatedMembers)
            {
                subtreeWidths.Add(CalculateMnimWidth(member, direction));
            } 
                

            // -------------- Обрабатываем связанных членов и назначаем координаты --------------
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
                CalculateNodeCoordinatesDirectionatly(related, newX , newY, direction);
            }
            // ------------------------------------------------------------------------------------
        }
        

        /// <summary>
        /// Вычисляет условную ширину подветви относительно указанного корня, идя вверх/вниз.
        /// </summary>
        /// <param name="root">Корневой узел, от которого начинается расчёт ширины.</param>
        /// <param name="direction">Направление обхода: вверх (к родителям) или вниз (к детям).</param>
        /// <param name="visited">Множество посещённых узлов для предотвращения зацикливания.</param>
        /// <returns>Условная ширина подветви.</returns>
        private int CalculateMnimWidth(Member root, Direction direction)
        {
            int width = 0;

            // Получение связанных членов в зависимости от направления обхода
            var relatedMembers = direction == Direction.Down
                ? familyService.GetRelatedMembers(root, Relationship.ToChild)
                : familyService.GetRelatedMembers(root, Relationship.ToParent);

            // обход всех "детей".
            foreach (var relatedMember in relatedMembers)
                width += CalculateMnimWidth(relatedMember, direction);

            return Math.Max(1, width);
        }
    }
}
