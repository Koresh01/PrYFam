using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using PrYFam.Assets.Scripts.NodesPosCalculator;

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
        private float CardWidthWithOffset, CardHeight;
        private Dictionary<Member, Vector2> coordinates;

        private ITraversalStrategy traversalStrategy = new LeftToRightTraversal(); // По умолчанию слева направо


        /// <summary>
        /// Инициализирует стартовые параметры для переотрисовки.
        /// </summary>
        private void Initialize(Member root, FamilyService familyService, Vector2 basePosition, float horizontalSpacing, float verticalSpacing)
        {
            this.root = root;
            this.familyService = familyService;
            this.basePosition = basePosition;
            this.CardWidthWithOffset = horizontalSpacing;
            this.CardHeight = verticalSpacing;

            this.coordinates = new Dictionary<Member, Vector2>();
        }

        /// <summary>
        /// Перессчитывает координаты карточек.
        /// </summary>
        public Dictionary<Member, Vector2> ReCalculate(Member root, FamilyService familyService, Vector2 basePosition, float CardWidth, float CardHeight)
        {
            Initialize(root, familyService, basePosition, CardWidth, CardHeight);
            Calculate();
            return coordinates;
        }

        /// <summary>
        /// Вычисляет координаты вершин, но только тех, В КОТОРЫЕ ПОПАДАЕТ ПРИ ОБХОДЕ
        /// </summary>
        private void Calculate()
        {
            bool hasHalf = familyService.hasHalf(root); // Проверяем есть ли жена. [Подразумевается, что жена максимум одна]
            Member half = hasHalf ? familyService.GetRelatedMembers(root, Relationship.ToHalf).FirstOrDefault() : null;

            if (!hasHalf)
            {
                float startY = basePosition.y;
                float startX = basePosition.x;

                CalculateNodeCoordinatesDirectionatly(root, startX, startY, Direction.Down);    // Отрисовка древа вверх.
                CalculateNodeCoordinatesDirectionatly(root, startX, startY, Direction.Up);      // Отрисовка древа вниз.
            }
            if (hasHalf)
            {
                // Определим на кого конкретно нажал пользователь. На левую карточку или правую?
                Vector2 clickedPos  = root.gameObject.GetComponent<RectTransform>().anchoredPosition;
                Vector2 halfPos     = half.gameObject.GetComponent<RectTransform>().anchoredPosition;

                if (clickedPos.x < halfPos.x)   // Весьма опасная проверка, неизвестно какая позиция присваивается карточке жены при создании.
                {
                    SetTraversalStrategy(new LeftToRightTraversal());
                    // Ищем pivot[середины] для карточек супругов:
                    Vector2 spouseCardsMidpoint = new Vector2(
                        basePosition.x + CardWidthWithOffset / 2f,
                        basePosition.y
                    );

                    // Отрисовываем древо вниз, относительно midcenter.
                    CalculateNodeCoordinatesDirectionatly(root, spouseCardsMidpoint.x, spouseCardsMidpoint.y, Direction.Down);
                    CalculateNodeCoordinatesDirectionatly(root, clickedPos.x, clickedPos.y, Direction.Up);
                }
                if (clickedPos.x > halfPos.x)
                {
                    SetTraversalStrategy(new RightToLeftTraversal());
                    
                    // Ищем pivot[середины] для карточек супругов:
                    Vector2 spouseCardsMidpoint = new Vector2(
                        basePosition.x - CardWidthWithOffset / 2f,
                        basePosition.y
                    );

                    // Отрисовываем древо вниз, относительно midcenter.
                    CalculateNodeCoordinatesDirectionatly(root, spouseCardsMidpoint.x, spouseCardsMidpoint.y, Direction.Down);
                    CalculateNodeCoordinatesDirectionatly(root, clickedPos.x, clickedPos.y, Direction.Up);
                }
            }
        }

        /// <summary>
        /// Рекурсивно назначает координаты узлам дерева, идя в заданном направлении: вверх или вниз.
        /// </summary>
        private void CalculateNodeCoordinatesDirectionatly(Member current, float x, float y, Direction direction)
        {
            if (current == null)
                return;

            if (direction == Direction.Down)
            {
                float offset = CardWidthWithOffset / 2f;
                if (!familyService.hasHalf(current))
                {
                    coordinates[current] = new Vector2(x, y);
                }
                if (familyService.hasHalf(current))
                {
                    if (traversalStrategy.IsLeftToRight)
                    {
                        coordinates[current] = new Vector2(x - offset, y);
                        Member half = familyService.GetRelatedMembers(current, Relationship.ToHalf).FirstOrDefault();
                        coordinates[half] = new Vector2(x + offset, y);
                    }
                    if (!traversalStrategy.IsLeftToRight)
                    {
                        coordinates[current] = new Vector2(x + offset, y);
                        Member half = familyService.GetRelatedMembers(current, Relationship.ToHalf).FirstOrDefault();
                        coordinates[half] = new Vector2(x - offset, y);
                    }
                }
            }
            if (direction == Direction.Up)
            {
                coordinates[current] = new Vector2(x, y);
            }

            // Используем паттерн "стратегия" для порядка обхода.
            var relatedMembers = traversalStrategy.Traverse(
                direction == Direction.Down
                    ? familyService.GetRelatedMembers(current, Relationship.ToChild)
                    : familyService.GetRelatedMembers(current, Relationship.ToParent)
            ).ToList();

            List<int> subtreeWidths = new List<int>();
            foreach (var member in relatedMembers)
            {
                subtreeWidths.Add(CalculateMnimWidth(member, direction));
            }

            float offsetY = direction == Direction.Down ? -CardHeight : CardHeight;

            for (int i = 0; i < relatedMembers.Count; i++)
            {
                Member related = relatedMembers[i];
                
                // Подссчёт условной ширины подветвей, которые левее рассматриваемой:
                float cumulativeWidth = 0;
                for (int k = 0; k < i; k++)
                    cumulativeWidth += subtreeWidths[k];


                float newY = y + offsetY;
                float newX = 0;
                if (traversalStrategy.IsLeftToRight)    // ?
                {
                    newX = x + CardWidthWithOffset * (-subtreeWidths.Sum() / 2f + cumulativeWidth + subtreeWidths[i] / 2f);
                }
                if (!traversalStrategy.IsLeftToRight) // а теперь идём справа налево
                {  
                    newX = x + CardWidthWithOffset * (subtreeWidths.Sum() / 2f - cumulativeWidth - subtreeWidths[i] / 2f);
                }



                CalculateNodeCoordinatesDirectionatly(related, newX, newY, direction);
            }
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


            if (direction == Direction.Down)
            {
                bool spoon = familyService.hasHalf(root);
                return Math.Max(spoon ? 2 : 1, width);
            }
            if (direction == Direction.Up)
            {
                return Math.Max(1, width);
            }

            // Обработка остальных случаев (например, неизвестного направления)
            throw new ArgumentException($"Unknown direction: {direction}");
        }

        /// <summary>
        ///  Устанавливает интерфейс для горизонтальной направленности движения при вычислении координат карточек. То есть если было осуществлено нажатие на ПРАВУЮ карточку, то делаем отрисовку справа-налево, а если нажали на ЛЕВУЮ то отрисовываем слева-направо.
        /// </summary>
        /// <param name="strategy"> Экземпляр класса LeftToRightTraversal или RightToLeftTraversal. </param>
        public void SetTraversalStrategy(ITraversalStrategy strategy)
        {
            traversalStrategy = strategy;
        }
    }
}
