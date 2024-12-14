using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ���������� ������������ ���� ��� ������ � UI

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// ������ ��� �������� ��������� ����� UI-���������� Image � �������������� ���������.
    /// ���� ���������� �� ��������� ��������� � ������������ ���������.
    /// </summary>
    public class ColorChanger : MonoBehaviour
    {
        [Header("��������� Image, ������� ������ ������������ ������.")]
        [SerializeField]
        Image _image; // ������ �� ��������� Image

        [Header("��������")]
        [SerializeField]
        Gradient _gradient; // �������� ��� ��������� �����

        [Header("�������� ��������� �����")]
        [SerializeField]
        float speed = 1.0f; // �������� ��������� (1.0f = ����������� ��������)

        void Update()
        {
            if (_image != null)
            {
                // �������� Time.time �� speed, ����� ������������ �������� ��������� �����
                _image.color = _gradient.Evaluate(Mathf.PingPong(Time.time * speed, 1f));
            }
        }
    }
}