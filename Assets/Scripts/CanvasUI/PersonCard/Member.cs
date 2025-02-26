using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// ������ ��� meta-���������� � ���������� ����� ��������� �����.
    /// </summary>
    public class Member : MonoBehaviour
    {
        public string UniqueId;
        [Header("�������� ����������")]
        [Tooltip("��� ��������")]
        public string FirstName;

        [Tooltip("������� ��������")]
        public string LastName;

        [Tooltip("�������� ��������")]
        public string MiddleName;

        [Space]

        [Tooltip("����������� � ������� ���������� ����� �����.")]
        public Sprite ProfilePicture;

        [Header("���� � ����� ��������")]
        [Tooltip("���� �����:")]
        public string DateOfBirth;

        [Tooltip("����� ��������")]
        public string PlaceOfBirth;


        [Tooltip("������� ��������� ��� ��������")]
        [TextArea]
        public string Biography;
    }
}
