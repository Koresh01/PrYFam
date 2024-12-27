using System;
using System.Linq;  // ��� ������� � .Where � List<>
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PrYFam
{
    /// <summary>
    /// ������ ��� ������ � ������� ��������� �����.
    /// ������������� ���������������� ��� ��������, ��������� � ��������� �������� ������.
    /// </summary>
    public class FamilyService : MonoBehaviour
    {
        public GameObject personCardPrefab;
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
        public void CreateMemberWithConnection(GameObject From, Relationship relationship)
        {
            Member from = From.GetComponent<Member>();
            if (!CanAddConnection(from, relationship)) return;
            
            GameObject newMemberObj = createMemberGameObject();
            Member to = newMemberObj.GetComponent<Member>();

            AddConnection(from, to, relationship);
        }
        /// <summary>
        /// ������ �������� ������� ��������.
        /// </summary>
        public GameObject createMemberGameObject()
        {
            var go = Instantiate(personCardPrefab, cardsPlaceHolder);
            go.SetActive(false);    // ������������ ��� ������ ����� ���������.
            return go;
        }
        /// <summary>
        /// ��������� ����� ����� �������� ��������� prefab-���: [PersonCard] � �������� �����.
        /// </summary>
        public void AddConnection(Member from, Member to, Relationship relationship = Relationship.None)
        {
            // ��������� ����������� ��������
            switch (relationship)
            {
                case Relationship.ToHalf:
                    HandleToHalf(from, to);     // ������ � �������� �������� ����������
                    break;
                case Relationship.ToParent:
                    HandleToParent(from, to);   // ������ � �������� �������� ����������
                    break;
                case Relationship.ToChild:
                    HandleToChild(from, to);    // ������ � �������� �������� ����������
                    break;
                default:
                    Debug.LogWarning("����������� ���������: " + relationship);
                    break;
            }


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
        

        /// <summary>
        /// ������� ��� ��������� ����������� Member �� FamilyData.
        /// </summary>
        /// <param name="person">������� ��� ��������</param>
        public void DeletePerson(Member person)
        {
            // ������� ����� �� PERSON
            familyData.relationships.RemoveAll(entry => entry.From == person);
            // ������� ����� � PERSON
            familyData.relationships.RemoveAll(entry => entry.To == person);
        }
        /// <summary>
        /// ������� ����������� �� FamilyData.
        /// </summary>
        private void RemoveRelationship(Member from, Member to)
        {
            // ������� ������ �����
            familyData.relationships.RemoveAll(entry => entry.From == from && entry.To == to);

            // ������� �������� �����
            familyData.relationships.RemoveAll(entry => entry.From == to && entry.To == from);
        }
        /// <summary>
        /// �������� �������� ����� ����� �������
        /// </summary>
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

        /// <summary>
        /// �������� �� ���� ����� ������� � �����.
        /// </summary>
        public bool IsLeaf(Member cur) {
            if (hasParents(cur) && !hasChildren(cur) && !hasHalf(cur))
                return true;
            if (!hasParents(cur) && hasChildren(cur) && hasHalf(cur))
                return true;
            if (!hasParents(cur) && hasChildren(cur) && !hasHalf(cur) && GetRelatedMembers(cur, Relationship.ToChild).Count <= 1)
                return true;

            Debug.Log("���������� ������� ��� ����������� ����������� �����!!!");
            return false;
        }

        public bool hasChildren(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToChild).Count == 0 ? false : true;
        }
        public bool hasParents(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToParent).Count == 0 ? false : true;
        }
        public bool hasAllParents(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToParent).Count == 2 ? true : false;
        }
        public bool hasHalf(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToHalf).Count > 0 ? true : false;
        }


        /// <summary> ��������� ����� �� �������� �����������. </summary>
        private bool CanAddConnection(Member from, Relationship relationship)
        {
            switch (relationship) {
                case Relationship.ToHalf:
                    if (!CanAddHalf(from)) return false;
                    break;
                case Relationship.ToParent:
                    if (!CanAddParent(from)) return false;
                    break;
                case Relationship.ToChild:
                    break;
            }
            return true;
        }
        /// <summary> ��������� ����� �� �������� ����. </summary>
        private bool CanAddHalf(Member from)
        {
            if (hasHalf(from))
            {
                Debug.Log("������ ��������� ������ ����� ����.");
                return false;
            }
            return true;
        }
        /// <summary> ��������� ����� �� �������� ��� ��������. </summary>
        private bool CanAddParent(Member from)
        {
            if (GetRelatedMembers(from, Relationship.ToParent).Count >= 2)
            {
                Debug.Log("������ ��������� ������ ���� ���������.");
                return false;
            }
            return true;
        }


        /// <summary>
        /// ������������ ���������� ������� (���� ��� ����) ��� ���������� ����� �����. 
        /// ���� � ����� ����� ��� ���� ������, �� ���������� �� �����������. 
        /// ����� ��������� ����� ����� �������� � ������.
        /// </summary>
        private void HandleToHalf(Member from, Member to)
        {
            AddBidirectionalRelationship(from, to, Relationship.ToHalf);    // ������ ����� 100% ���������

            // ������:
            foreach (var child in GetRelatedMembers(from, Relationship.ToChild))
            {
                AddBidirectionalRelationship(child, to, Relationship.ToParent);
            }
        }
        /// <summary>
        /// ������������ ���������� �������� ��� ���������� ����� �����. 
        /// ���� � ����� ����� ��� ���� ��� ��������, �� ���������� �� �����������. 
        /// ����� ������������� ����� ����� ����� ��������� � ��� ������������ ������ ��������� ��� "�������".
        /// </summary>
        private void HandleToParent(Member from, Member to)
        {
            AddBidirectionalRelationship(from, to, Relationship.ToParent);    // ������ ����� 100% ���������

            // ������:
            foreach (var parent in GetRelatedMembers(from, Relationship.ToParent))
            {
                if (parent != to)
                {
                    AddBidirectionalRelationship(parent, to, Relationship.ToHalf);
                }
            }
        }
        /// <summary>
        /// ������������ ���������� ������ ��� ���������� ����� �����. 
        /// ������������� ����� ����� ����������� ������� � �������� ����� �����.
        /// </summary>
        private void HandleToChild(Member from, Member to)
        {
            AddBidirectionalRelationship(from, to, Relationship.ToChild);    // ������ ����� 100% ���������

            // ������:
            foreach (var half in GetRelatedMembers(from, Relationship.ToHalf))
            {
                AddBidirectionalRelationship(half, to, Relationship.ToChild);
            }
        }


        #endregion
    }
}
