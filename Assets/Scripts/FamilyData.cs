using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PrYFam.Assets.Scripts
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
            return relationships
                .SelectMany(entry => new[] { entry.From.gameObject, entry.To.gameObject })
                .Distinct()
                .ToList();
        }
    }
}