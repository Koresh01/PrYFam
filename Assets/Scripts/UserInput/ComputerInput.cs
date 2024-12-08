using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    public class ComputerInput : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        [SerializeField] CommonInputSettings commonInputSettings;

        [Header("Скорость перемещения камеры по поверхности:")]
        public float movementSpeed = 5f;

        [Header("Отдаление/приближение колесом мыши:")]
        public float zoomSpeed = 5.0f;

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
            mainCamera.transform.Translate(movement);
        }
        private void SCROLL()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            float oldZpos = mainCamera.transform.position.z;

            float newZ = oldZpos + scroll * zoomSpeed;

            float minZoom = -commonInputSettings.minZoom;
            float maxZoom = -commonInputSettings.maxZoom;
            // Проверка границ зума
            if (newZ < minZoom || newZ > maxZoom)
                return;

            // Создаем новый вектор с обновленным значением z
            Vector3 newPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, newZ);
            mainCamera.transform.position = newPosition;
        }
    }
}