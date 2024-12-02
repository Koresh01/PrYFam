using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;

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
        private float horizontalSpacing, verticalSpacing;
        private Dictionary<Member, Vector2> coordinates;

        /// <summary>
        /// �������������� ��������� ��������� ��� �������������.
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
        /// �������������� ���������� ��������.
        /// </summary>
        public Dictionary<Member, Vector2> ReCalculate(Member root, FamilyService familyService, Vector2 basePosition, float horizontalSpacing, float verticalSpacing)
        {
            Initialize(root, familyService, basePosition, horizontalSpacing, verticalSpacing);
            Calculate();
            return coordinates;
        }
        
        
        

        /// <summary>
        /// ��������� ���������� ������, �� ������ ���, � ������� �������� ��� ������
        /// </summary>
        private void Calculate()
        {
            // ��������� ���������� ��� ���� �����������
            CalculateNodeCoordinatesDirectionatly(root, basePosition.x, basePosition.y, Direction.Down);
            CalculateNodeCoordinatesDirectionatly(root, basePosition.x, basePosition.y, Direction.Up);
        }

        /// <summary>
        /// ���������� ��������� ���������� ����� ������, ��� � �������� �����������: ����� ��� ����.
        /// </summary>
        private void CalculateNodeCoordinatesDirectionatly(Member current, float x, float y, Direction direction)
        {
            if (current == null)
                return;

            coordinates[current] = new Vector2(x, y);
            

            // �������� ��������� ������ � �� �������� ������
            List<Member> relatedMembers = direction == Direction.Down ? familyService.GetRelatedMembers(current, Relationship.ToChild) : familyService.GetRelatedMembers(current, Relationship.ToParent);

            // �������� �������� ������ ��������� � ����������� ������� ��������
            List<int> subtreeWidths = new List<int>();
            foreach (var member in relatedMembers)
            {
                subtreeWidths.Add(CalculateMnimWidth(member, direction));
            } 
                

            // -------------- ������������ ��������� ������ � ��������� ���������� --------------
            // �������� �� ��������� ������� �� ����������� ������
            float offsetY = direction == Direction.Down ? -verticalSpacing : verticalSpacing;

            // �������� �� ��������� ������
            for (int i = 0; i < relatedMembers.Count; i++)
            {
                Member related = relatedMembers[i];


                float cumulativeWidth = 0;
                for (int k = 0; k < i; k++)
                    cumulativeWidth += subtreeWidths[k];

                // ������������� �������:
                float newY = y + offsetY;
                float newX = x + horizontalSpacing * (-subtreeWidths.Sum() / 2f + cumulativeWidth + subtreeWidths[i] / 2f);
                // ���������� ��������� ����������
                CalculateNodeCoordinatesDirectionatly(related, newX , newY, direction);
            }
            // ------------------------------------------------------------------------------------
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

            return Math.Max(1, width);
        }
    }
}
