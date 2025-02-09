using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using PrYFam;

namespace PrYFam
{
    /// <summary>
    /// Класс для выполнения алгоритмов работы с семейным деревом.
    /// </summary>
    public class Algorithms
    {
        private static Algorithms _singleton;

        /// <summary>
        /// Геттер для получения единственного экземпляра класса.
        /// Создаёт экземпляр при первом обращении и сохраняет его в _singleton.
        /// </summary>
        public static Algorithms Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new Algorithms();
                }
                return _singleton;
            }
        }
        public Member root;
        private FamilyService familyService;
        private Vector2 basePosition;
        private float CardWidthWithOffset, CardHeight;
        private Dictionary<Member, Vector2> coordinates;

        private ITraversalStrategy traversalStrategy = new LeftToRightTraversal(); // По умолчанию слева направо


        /// <summary>
        /// Закрытый конструктор для предотвращения создания экземпляров вне класса
        /// </summary>
        private Algorithms() { }

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
            Member half = hasHalf ? familyService.GetSelectedHalf(root) : null;

            

            if (!hasHalf)
            {
                Vector2 startPosition = basePosition;

                CalculateNodeCoordinatesDirectionatly(root, startPosition, Direction.Down);    // Отрисовка древа вверх.
                CalculateNodeCoordinatesDirectionatly(root, startPosition, Direction.Up);      // Отрисовка древа вниз.
            }
            if (hasHalf)
            {
                // Определим на кого конкретно нажал пользователь. На левую карточку или правую?
                Vector2 clickedPos  = basePosition; // root.gameObject.GetComponent<RectTransform>().anchoredPosition;
                Vector2 halfPos     = half.gameObject.GetComponent<RectTransform>().anchoredPosition;


                if (clickedPos.x <= halfPos.x)   // Весьма опасная проверка, неизвестно какая позиция присваивается карточке жены при создании.
                {
                    SetTraversalStrategy(new LeftToRightTraversal());
                    // Ищем pivot[середины] для карточек супругов:
                    Vector2 spouseCardsMidpoint = new Vector2(
                        basePosition.x + CardWidthWithOffset / 2f,
                        basePosition.y
                    );

                    // Отрисовываем древо вниз, относительно midcenter.
                    CalculateNodeCoordinatesDirectionatly(root, spouseCardsMidpoint, Direction.Down);
                    CalculateNodeCoordinatesDirectionatly(root, clickedPos, Direction.Up);
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
                    CalculateNodeCoordinatesDirectionatly(root, spouseCardsMidpoint, Direction.Down);
                    CalculateNodeCoordinatesDirectionatly(root, clickedPos, Direction.Up);
                }
            }
        }






        /// <summary>
        /// Рекурсивно назначает координаты узлам дерева, идя в заданном направлении: вверх или вниз. 
        /// </summary>
        /// <param name="current"> Член семьи относительно которого производим обход древа. </param>
        /// <param name="branchMidpoint"> Позиция середины этой подветви. </param>
        /// <param name="direction"> Вертикальное направление обхода.</param>
        private void CalculateNodeCoordinatesDirectionatly(Member current, Vector2 branchMidpoint, Direction direction)
        {
            if (current == null)
                return;

            if (direction == Direction.Down)
            {
                float offset = CardWidthWithOffset / 2f;
                if (!familyService.hasHalf(current))
                {
                    coordinates[current] = branchMidpoint;
                }
                if (familyService.hasHalf(current))
                {
                    if (traversalStrategy.IsLeftToRight)
                    {
                        coordinates[current]    = branchMidpoint - new Vector2(offset, 0);
                        Member half = familyService.GetSelectedHalf(current);
                        coordinates[half]       = branchMidpoint + new Vector2(offset, 0);
                    }
                    if (!traversalStrategy.IsLeftToRight)
                    {
                        coordinates[current]    = branchMidpoint + new Vector2(offset, 0);
                        Member half = familyService.GetSelectedHalf(current);
                        coordinates[half]       = branchMidpoint - new Vector2(offset, 0);
                    }
                }
            }
            if (direction == Direction.Up)
            {
                coordinates[current] = branchMidpoint;
            }


            // Получение связанных членов в зависимости от направления обхода
            List<Member> relatedMembers = GetRelatedMembers(current, direction);
            // Используем паттерн "стратегия" для порядка обхода. На самом деле порядок всегда один и тот же... Мы там в обычом порядке возвращаем relatedMembers.
            relatedMembers = traversalStrategy.Traverse(relatedMembers).ToList();


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


                float newY = branchMidpoint.y + offsetY;
                float newX = 0;



                float c = 0;
                if (direction == Direction.Up && subtreeWidths.Count == 2)
                {
                    c = Math.Abs(subtreeWidths[0] - subtreeWidths[1]) / 4f;

                    int rightBranch = subtreeWidths[1];
                    int leftBranch = subtreeWidths[0];

                    if (rightBranch > leftBranch)
                    {
                        c = traversalStrategy.IsLeftToRight ? +c : -c;
                    }
                    else if (rightBranch < leftBranch)
                    {
                        c = traversalStrategy.IsLeftToRight ? -c : +c;
                    }
                }    
                    


                if (traversalStrategy.IsLeftToRight)    // Значит было нажатие на левую карточку супругов.
                {
                    newX = branchMidpoint.x + CardWidthWithOffset * (-subtreeWidths.Sum() / 2f + c + cumulativeWidth + subtreeWidths[i] / 2f);
                }
                if (!traversalStrategy.IsLeftToRight)   // Значит было нажатие на правую карточку супругов.
                {
                    newX = branchMidpoint.x + CardWidthWithOffset * (subtreeWidths.Sum() / 2f + c - cumulativeWidth - subtreeWidths[i] / 2f);
                }


                Vector2 nextMidPoint = new Vector2(newX, newY);
                CalculateNodeCoordinatesDirectionatly(related, nextMidPoint, direction);
            }
        }
        /// <summary>
        /// Вычисляет условную ширину подветви относительно указанного корня, идя вверх/вниз.
        /// </summary>
        /// <param name="root">Корневой узел, от которого начинается расчёт ширины.</param>
        /// <param name="direction">Направление обхода: вверх (к родителям) или вниз (к детям).</param>
        /// <param name="visited">Множество посещённых узлов для предотвращения зацикливания.</param>
        /// <returns>Условная ширина подветви.</returns>
        private int CalculateMnimWidth(Member cur, Direction direction)
        {
            int width = 0;

            // Получение связанных членов в зависимости от направления обхода
            List<Member> relatedMembers = GetRelatedMembers(cur, direction);

            // обход всех "детей".
            foreach (var relatedMember in relatedMembers)
                width += CalculateMnimWidth(relatedMember, direction);


            if (direction == Direction.Down)
            {
                bool spoon = familyService.hasHalf(cur);
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
        /// Возвращает членов семьи в зависимости от направления обхода.
        /// При движении вверх возвращает родителей, вниз — детей.
        /// Если у члена семьи есть жена, возвращает только общих детей с ней.
        /// </summary>
        private List<Member> GetRelatedMembers(Member cur, Direction direction)
        {
            List<Member> relatedMembers = new List<Member>();
            if (direction == Direction.Down)
            {
                // Нужно не всех детей нашего cur выдавать, а только общих, с selected-женой, если такая имеется.
                if (familyService.hasHalf(cur))
                {
                    Member selectedHalf = familyService.GetSelectedHalf(cur);
                    relatedMembers = familyService.GetChildMembers(cur, selectedHalf);
                }
                else
                    relatedMembers = familyService.GetChildMembers(cur);
            }
            if (direction == Direction.Up)
            {
                relatedMembers = familyService.GetParentMembers(cur);
            }

            return relatedMembers;
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
