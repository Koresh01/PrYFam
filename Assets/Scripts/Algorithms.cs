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
        private float horizontalSpacing, verticalSpacing, halfCardOffset;
        private Dictionary<Member, Vector2> coordinates;

        # region main
        /// <summary> �������������� ��������� ��������� ��� �������������. </summary>
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

        /// <summary> ��������� ���������� ������, �� ������ ���, � ������� �������� ��� ������ </summary>
        private void CalculateNodeCoordinates()
        {
            // ��������� ���������� ��� ���� �����������
            CalculateNodeCoordinatesDirectionatly(root, basePosition.x, basePosition.y, Direction.Up);
            CalculateNodeCoordinatesDirectionatly(root, basePosition.x, basePosition.y, Direction.Down);
        }

        /// <summary> ���������� ��������� ���������� ����� ������, ��� � �������� �����������: ����� ��� ����. </summary>
        private void CalculateNodeCoordinatesDirectionatly(Member current, float x, float y, Direction direction)
        {
            if (current == null)
                return;


            coordinates[current] = new Vector2(x, y);

            // --------------- �������� ���� ---------------------
            if (familyService.hasHalf(current))
            {
                List<Member> halfs = familyService.GetRelatedMembers(current, Relationship.ToHalf);
                for (int i = 1; i <= halfs.Count; i++)
                {
                    Member concreteHalf = halfs[i - 1];
                    concreteHalf.gameObject.transform.SetAsFirstSibling();  // ����������� ������� �� ������ ����
                    coordinates[concreteHalf] = new Vector2(x, y) + i * new Vector2(halfCardOffset, halfCardOffset);
                }
            }
            // ---------------------------------------------------------
            


            // �������� ��������� ������ � �� �������� ������
            var relatedMembers = direction == Direction.Down
                ? familyService.GetRelatedMembers(current, Relationship.ToChild)
                : familyService.GetRelatedMembers(current, Relationship.ToParent);

            // �������� �������� ������ ��������� � ����������� ������� ��������
            var subtreeWidths = CalculateSubtreeWidths(current, direction);

            // ������������ ��������� ������ � ��������� ����������
            ProcessRelatedMembers(
                relatedMembers,
                subtreeWidths,
                x, y,
                direction);
        }
        /// <summary> ������������ ��������� ������ ����, ��������� �� ���������� � �������� ����������� ����������. </summary>
        private void ProcessRelatedMembers(
            List<Member> relatedMembers,
            List<int> subtreeWidths,
            float x, float y,
            Direction direction)
        {
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
                CalculateNodeCoordinatesDirectionatly(related, newX, newY, direction);
            }
        }
        #endregion
        # region MnimWidth
        /// <summary>��������� ������ ����������� ��� ������� �� ��������� �����.</summary>
        /// <returns>������ int ��������.</returns>
        private List<int> CalculateSubtreeWidths(Member current, Direction direction)
        {
            // ��������� ������ ��������� ������ (����� ��� ���������)
            var relatedMembers = direction == Direction.Down
                ? familyService.GetRelatedMembers(current, Relationship.ToChild)
                : familyService.GetRelatedMembers(current, Relationship.ToParent);

            // ��� ������� ���� ��������� �������� ������ ��������
            return relatedMembers
                .Select(member => CalculateMnimWidth(member, direction))
                .ToList(); // ����������� ��������� � ������
        }

        /// <summary>
        /// ��������� �������� ������ �������� ������������ ���������� �����, ��� �����/����.
        /// </summary>
        /// <param name="root">�������� ����, �� �������� ���������� ������ ������.</param>
        /// <param name="direction">����������� ������: ����� (� ���������) ��� ���� (� �����).</param>
        /// <param name="visited">��������� ���������� ����� ��� �������������� ������������.</param>
        /// <returns>�������� ������ ��������.</returns>
        private int CalculateMnimWidth(Member root, Direction direction, HashSet<Member> visited = null)
        {
            // ������� �������: ��� �����, ��� �������� ������ ��� ���� ��� �������
            if (root == null || familyService == null)
                return 0;

            visited ??= new HashSet<Member>(); // ������������� ��������� ���������� �����

            if (visited.Contains(root))
                return 0;

            visited.Add(root); // �������� ���� ��� ����������

            // ��������� ��������� ������ � ����������� �� ����������� ������
            var relatedMembers = direction == Direction.Down
                ? familyService.GetRelatedMembers(root, Relationship.ToChild)
                : familyService.GetRelatedMembers(root, Relationship.ToParent);

            

            // ---------------------���� � ��������� ����--------------------------
            // ����������� ������ ������ ��� ���� ��������� �����
            int cumulateWidth = relatedMembers
                .Select(member => CalculateMnimWidth(member, direction, visited))
                .Sum(); // ������� �������� ������ ��������.

            // �������� ������� ����
            var spouse = familyService.hasHalf(root);
            if (spouse)
            {
                cumulateWidth = Math.Max(2, cumulateWidth); // ���� ���� ���� � ������ ������ 2, ������������� ������ 2
            }
            else {  // ���� ��� ����
                cumulateWidth = Math.Max(1, cumulateWidth);   // ���� ��� ����, �� �������� ����������� �������� ���������� ������ � ��� ��� ����� �����...
            }
            // --------------------------------------------------------------------

            return cumulateWidth;
        }
        #endregion
    }
}
