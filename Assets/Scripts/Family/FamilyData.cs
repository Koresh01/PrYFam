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
    }
}