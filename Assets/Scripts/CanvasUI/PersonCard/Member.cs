using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// ������������ ����� ��������� �����.
    /// ������ ������ ����� ������ �������� ������ �� ����� ���������, ����� � ��������� ������� �������������.
    /// </summary>
    public class Member : MonoBehaviour
    {
        [Header("�������� ����������")]
        [Tooltip("��� ��������")]
        public string FirstName;

        [Tooltip("������� ��������")]
        public string LastName;

        [Tooltip("�������� ��������")]
        public string MiddleName;

        [Space]

        [Tooltip("���������� ��������")]
        public Sprite ProfilePicture;

        [Header("���� � ����� ��������")]
        [Tooltip("���� ��������")]
        public DateTime DateOfBirth;

        [Tooltip("����� ��������")]
        public string PlaceOfBirth;

        [Space]
        
        [Header("�������������� ����������")]
        [Tooltip("���� ������ (�������� ������, ���� ���)")]
        public DateTime? DateOfDeath;

        [Tooltip("����� ������ (���� ���������)")]
        public string PlaceOfDeath;

        [Tooltip("������� ��������� ��� ��������")]
        [TextArea]
        public string Biography;
    }
}
