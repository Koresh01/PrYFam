using System;
using System.Linq;  // ��� ������� � .Where � List<>
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// ������ ��� ������ � ������� ��������� �����.
    /// ������������� ���������������� ��� ��������, ��������� � ��������� �������� ������.
    /// </summary>
    public class FamilyService : MonoBehaviour
    {
        [SerializeField] GameObject personCardPrefab;
        [SerializeField] Transform cardsPlaceHolder;
        [Header("�����������:")]
        public FamilyData familyData;

        /// <summary>
        /// ���������� ������ ������ ��������� ������, ��������� � ��������� ������ ����� ����� �������� ��� �����.
        /// </summary>
        /// <param name="from">���� �����, ��� �������� ������ ��������� ���������.</param>
        /// <param name="relationship">��� �����, ��������, �������� ��� �������.</param>
        /// <returns>������ ��������� ������ �����.</returns>
        public List<Member> GetRelatedMembers(Member from, Relationship relationship)
        {
            return familyData.relationships
                .Where(e => e.From == from && e.Relationship == relationship)
                .Select(e => e.To)
                .ToList();
        }
        #region creating new person
        /// <summary>
        /// ��������� ������ �������� � ����� � ��������� SMART-����� � ����.
        /// </summary>
        public GameObject CreateMemberWithConnection(GameObject parent, Relationship relationship)
        {
            GameObject newMemberObj = createGameObject();
            AddConnection(parent, newMemberObj, relationship);
            return newMemberObj;
        }
        /// <summary>
        /// ������ �������� ������� ��������.
        /// </summary>
        private GameObject createGameObject()
        {
            var go = Instantiate(personCardPrefab, cardsPlaceHolder);
            return go;
        }
        /// <summary>
        /// ��������� ����� ����� �������� ��������� prefab-���: [PersonCard] � �������� �����.
        /// </summary>
        private void AddConnection(GameObject From, GameObject To, Relationship relationship = Relationship.None)
        {
            Member from = From.GetComponent<Member>();
            Member to = To.GetComponent<Member>();

            // ��� ��� �� ����� ��������
            // ���� ���� relationship == ToHalf => ������������ ������ �������� ����.
            // � ������ ���� Member from - ��� ��� � ���� Member to;
            // to.children = from.children

            AddBidirectionalRelationship(from, to, relationship);
        }
        #endregion
        #region relationships

        /// <summary> ��������� ��������������� ����� ����� �������. </summary>
        private void AddBidirectionalRelationship(Member from, Member to, Relationship relationship)
        {
            Debug.LogFormat("�������� �� {0} �������� ����� � {1}", from.name, to.name);

            // ���������, ���� �� ��� ����� �����
            if (!RelationshipExists(from, to))
            {
                if (to != null)
                {
                    // ��������� ������ �����:
                    familyData.relationships.Add(new MembersConnection
                    {
                        From = from,
                        To = to,
                        Relationship = relationship
                    });
                    // ��������� �������� �����:
                    familyData.relationships.Add(new MembersConnection
                    {
                        From = to,
                        To = from,
                        Relationship = GetReverseRelationship(relationship)
                    });
                }
            }
        }
        /// <summary> ���������, ���������� �� ���� �� ���� ������� � ������, ��������������� ����� ���������.. </summary>
        /// <returns>bool</returns>
        public bool RelationshipExists(Member from, Member to)
        {
            return familyData.relationships.Exists(entry => entry.From == from && entry.To == to);
        }
        public Relationship? GetRelationship(Member from, Member to)
        {
            // ���������� ����� Find, ����� ����� ������ ������� � ������ `relationships`,
            // ������� ������������� �������: ���� From ����� from, � ���� To ����� to.
            // ���� ������� ������, �� ����� ������� � ���������� entry.
            // ���� ������� �� ������, entry ����� ����� null.
            var entry = familyData.relationships.Find(e => e.From == from && e.To == to);

            // ���������� �������� null-�������� ��������� (?.) ��� ����������� �������.
            // ���� entry �� ����� null, ���������� ���� Relationship �� ���������� ��������.
            // ���� entry ����� null, ���������� null.
            return entry?.Relationship;
        }
        public void RemoveRelationship(Member from, Member to)
        {
            // ������� ������ �����
            familyData.relationships.RemoveAll(entry => entry.From == from && entry.To == to);

            // ������� �������� �����
            familyData.relationships.RemoveAll(entry => entry.From == to && entry.To == from);
        }
        /// <summary> �������� �������� ����� ����� ������� </summary>
        private Relationship GetReverseRelationship(Relationship relationship)
        {
            return relationship switch
            {
                Relationship.ToParent => Relationship.ToChild,
                Relationship.ToHalf => Relationship.ToHalf,
                Relationship.ToChild => Relationship.ToParent,
                _ => throw new ArgumentOutOfRangeException(nameof(relationship), "����������� ���������"),
            };
        }
        #endregion
        #region simplified interaction
        public void DebugRelationships()    // ������� �������
        {
            foreach (var entry in familyData.relationships)
            {
                Debug.Log($"{entry.From?.name} -> {entry.To?.name}: {entry.Relationship}");
            }
        }
        public bool hasNoChildren(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToChild).Count == 0 ? true : false;
        }
        public bool hasNoParents(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToParent).Count == 0 ? true : false;
        }
        public bool hasAllParents(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToParent).Count == 2 ? true : false;
        }
        public bool hasHalf(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToHalf).Count > 0 ? true : false;
        }
        #endregion
    }
}
