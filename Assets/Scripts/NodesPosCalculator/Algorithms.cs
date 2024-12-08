using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using PrYFam.Assets.Scripts.NodesPosCalculator;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// ����� ��� ���������� ���������� ������ � �������� �������.
    /// </summary>
    public class Algorithms
    {
        private static Algorithms _singleton;

        /// <summary>
        /// ������ ��� ��������� ������������� ���������� ������.
        /// ������� ��������� ��� ������ ���������.
        /// </summary>
        public static Algorithms Singleton
        {
            get => _singleton == null ? new Algorithms() : _singleton;
            private set => _singleton = value; // �������� ������ ��� ����������� �������������
        }
        private Member root;
        private FamilyService familyService;
        private Vector2 basePosition;
        private float CardWidthWithOffset, CardHeight;
        private Dictionary<Member, Vector2> coordinates;

        private ITraversalStrategy traversalStrategy = new LeftToRightTraversal(); // �� ��������� ����� �������


        /// <summary>
        /// �������������� ��������� ��������� ��� �������������.
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
        /// �������������� ���������� ��������.
        /// </summary>
        public Dictionary<Member, Vector2> ReCalculate(Member root, FamilyService familyService, Vector2 basePosition, float CardWidth, float CardHeight)
        {
            Initialize(root, familyService, basePosition, CardWidth, CardHeight);
            Calculate();
            return coordinates;
        }

        /// <summary>
        /// ��������� ���������� ������, �� ������ ���, � ������� �������� ��� ������
        /// </summary>
        private void Calculate()
        {
            bool hasHalf = familyService.hasHalf(root); // ��������� ���� �� ����. [���������������, ��� ���� �������� ����]
            Member half = hasHalf ? familyService.GetRelatedMembers(root, Relationship.ToHalf).FirstOrDefault() : null;

            

            if (!hasHalf)
            {
                Vector2 startPosition = basePosition;

                CalculateNodeCoordinatesDirectionatly(root, startPosition, Direction.Down);    // ��������� ����� �����.
                CalculateNodeCoordinatesDirectionatly(root, startPosition, Direction.Up);      // ��������� ����� ����.
            }
            if (hasHalf)
            {
                // ��������� �� ���� ��������� ����� ������������. �� ����� �������� ��� ������?
                Vector2 clickedPos  = root.gameObject.GetComponent<RectTransform>().anchoredPosition;
                Vector2 halfPos     = half.gameObject.GetComponent<RectTransform>().anchoredPosition;

                if (clickedPos.x < halfPos.x)   // ������ ������� ��������, ���������� ����� ������� ������������� �������� ���� ��� ��������.
                {
                    SetTraversalStrategy(new LeftToRightTraversal());
                    // ���� pivot[��������] ��� �������� ��������:
                    Vector2 spouseCardsMidpoint = new Vector2(
                        basePosition.x + CardWidthWithOffset / 2f,
                        basePosition.y
                    );

                    // ������������ ����� ����, ������������ midcenter.
                    CalculateNodeCoordinatesDirectionatly(root, spouseCardsMidpoint, Direction.Down);
                    CalculateNodeCoordinatesDirectionatly(root, clickedPos, Direction.Up);
                }
                if (clickedPos.x > halfPos.x)
                {
                    SetTraversalStrategy(new RightToLeftTraversal());
                    
                    // ���� pivot[��������] ��� �������� ��������:
                    Vector2 spouseCardsMidpoint = new Vector2(
                        basePosition.x - CardWidthWithOffset / 2f,
                        basePosition.y
                    );

                    // ������������ ����� ����, ������������ midcenter.
                    CalculateNodeCoordinatesDirectionatly(root, spouseCardsMidpoint, Direction.Down);
                    CalculateNodeCoordinatesDirectionatly(root, clickedPos, Direction.Up);
                }
            }
        }


        /// <summary>
        /// ���������� ��������� ���������� ����� ������, ��� � �������� �����������: ����� ��� ����. 
        /// </summary>
        /// <param name="current"> ���� ����� ������������ �������� ���������� ����� �����. </param>
        /// <param name="branchMidpoint"> ������� �������� ���� ��������. </param>
        /// <param name="direction"> ������������ ����������� ������.</param>
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
                        Member half = familyService.GetRelatedMembers(current, Relationship.ToHalf).FirstOrDefault();
                        coordinates[half]       = branchMidpoint + new Vector2(offset, 0);
                    }
                    if (!traversalStrategy.IsLeftToRight)
                    {
                        coordinates[current]    = branchMidpoint + new Vector2(offset, 0);
                        Member half = familyService.GetRelatedMembers(current, Relationship.ToHalf).FirstOrDefault();
                        coordinates[half]       = branchMidpoint - new Vector2(offset, 0);
                    }
                }
            }
            if (direction == Direction.Up)
            {
                coordinates[current] = branchMidpoint;
            }

            // ���������� ������� "���������" ��� ������� ������. �� ����� ���� ������� ������ ���� � ��� ��... �� ��� � ������ ������� ���������� relatedMembers.
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
                
                // �������� �������� ������ ���������, ������� ����� ���������������:
                float cumulativeWidth = 0;
                for (int k = 0; k < i; k++)
                    cumulativeWidth += subtreeWidths[k];


                float newY = branchMidpoint.y + offsetY;
                float newX = 0;
                if (traversalStrategy.IsLeftToRight)    // ������ ���� ������� �� ����� �������� ��������.
                {
                    newX = branchMidpoint.x + CardWidthWithOffset * (-subtreeWidths.Sum() / 2f + cumulativeWidth + subtreeWidths[i] / 2f);
                }
                if (!traversalStrategy.IsLeftToRight)   // ������ ���� ������� �� ������ �������� ��������.
                {  
                    newX = branchMidpoint.x + CardWidthWithOffset * (subtreeWidths.Sum() / 2f - cumulativeWidth - subtreeWidths[i] / 2f);
                }


                Vector2 nextMidPoint = new Vector2(newX, newY);
                CalculateNodeCoordinatesDirectionatly(related, nextMidPoint, direction);
            }
        }


        /// <summary>
        /// ��������� �������� ������ �������� ������������ ���������� �����, ��� �����/����.
        /// </summary>
        /// <param name="root">�������� ����, �� �������� ���������� ������ ������.</param>
        /// <param name="direction">����������� ������: ����� (� ���������) ��� ���� (� �����).</param>
        /// <param name="visited">��������� ���������� ����� ��� �������������� ������������.</param>
        /// <returns>�������� ������ ��������.</returns>
        private int CalculateMnimWidth(Member root, Direction direction)
        {
            int width = 0;

            // ��������� ��������� ������ � ����������� �� ����������� ������
            var relatedMembers = direction == Direction.Down
                ? familyService.GetRelatedMembers(root, Relationship.ToChild)
                : familyService.GetRelatedMembers(root, Relationship.ToParent);

            // ����� ���� "�����".
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

            // ��������� ��������� ������� (��������, ������������ �����������)
            throw new ArgumentException($"Unknown direction: {direction}");
        }

        /// <summary>
        ///  ������������� ��������� ��� �������������� �������������� �������� ��� ���������� ��������� ��������. �� ���� ���� ���� ������������ ������� �� ������ ��������, �� ������ ��������� ������-������, � ���� ������ �� ����� �� ������������ �����-�������.
        /// </summary>
        /// <param name="strategy"> ��������� ������ LeftToRightTraversal ��� RightToLeftTraversal. </param>
        public void SetTraversalStrategy(ITraversalStrategy strategy)
        {
            traversalStrategy = strategy;
        }
    }
}