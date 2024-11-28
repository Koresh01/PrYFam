using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    public class CameraMovement : MonoBehaviour
    {
        [Header("Скорость перемещения камеры по поверхности:")]
        public float movementSpeed = 5f;

        [Header("Отдаление/приближение колесом мыши:")]
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

            // Проверка границ зума
            if (newZ > maxZoom || newZ < minZoom)
                return;

            // Создаем новый вектор с обновленным значением z
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, newZ);
            transform.position = newPosition;
        }
    }
}