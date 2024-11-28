using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    public class CameraMovement : MonoBehaviour
    {
        [Header("�������� ����������� ������ �� �����������:")]
        public float movementSpeed = 5f;

        [Header("���������/����������� ������� ����:")]
        public float zoomSpeed = 5.0f;
        private float minZoom = -80f;
        private float maxZoom = -3f;

        void Update()
        {
            WASD();
            SCROLL();
        }

        private void WASD()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * movementSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
        private void SCROLL()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            float oldZpos = transform.position.z;

            float newZ = oldZpos + scroll * zoomSpeed;

            // �������� ������ ����
            if (newZ > maxZoom || newZ < minZoom)
                return;

            // ������� ����� ������ � ����������� ��������� z
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, newZ);
            transform.position = newPosition;
        }
    }
}