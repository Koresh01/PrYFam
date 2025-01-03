using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PrYFam
{
    /// <summary>
    /// ������������ ����� ����� ����� ������� �����.
    /// �������� �������� ������, ������� ������ � ��� ��������� ����� ����.
    /// </summary>
    [System.Serializable]
    public class MembersConnection
    {
        public Member From;
        public Member To;
        public Relationship Relationship;
    }


    /// <summary>
    /// �������� ������ � �������� �����, ������� ������ ���� ������ ����� ������� �����.
    /// ������������� ������ ��� ����������, �������� � �������� ������.
    /// </summary>
    [System.Serializable]
    public class FamilyData : MonoBehaviour
    {
        // ������ ��� �������� ���������
        public List<MembersConnection> relationships = new List<MembersConnection>();

        /// <summary>
        /// ���������� ������ ���� ������� ��������, ��������� � ������� �����.
        /// </summary>
        /// <returns>������ GameObject, �������������� ���� ������ �����.</returns>
        public List<GameObject> GetAllPersonCards()
        {
            // ���������� ������ ���� ���������� ������� ��������, ��������� � �������� � ��������� relationships
            return relationships // �������� � ��������� relationships, ������� �������� ������ � ������ ����� ���������
                .SelectMany(entry => new[]
                {
                    // ��� ������ ������ � relationships ������� ������ �� ���� ��������:
                    entry.From.gameObject, // ��������� ������� ������, ��������� � From
                    entry.To.gameObject    // ��������� ������� ������, ��������� � To
                })
                .Distinct() // ������� ��������� �� ������������ ��������� ��������
                .ToList();  // ����������� �������������� ��������� � ������ � ����������
        }

        /// <summary>
        /// ���������� ������ ���� ���������� ������ �����.
        /// </summary>
        /// <returns>������ �������� Member, �������������� ���� ������ �����.</returns>
        public List<Member> GetAllMembers()
        {
            // �������� ���� ���������� ������ ����� �� ������
            return relationships
                .SelectMany(entry => new[] { entry.From, entry.To })
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// ������� ��� �������� � �������� ������ ���������.
        /// </summary>
        public void DestroyTree()
        {
            foreach (var entry in relationships)
            {
                try { Destroy(entry.From.gameObject);  } catch { Debug.Log("��� ������� ���"); }
                try { Destroy(entry.To.gameObject); } catch { Debug.Log("��� ������� ���"); }
            }

            relationships.Clear();
        }
    }
}