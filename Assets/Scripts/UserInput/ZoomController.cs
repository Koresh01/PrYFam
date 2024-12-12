using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam.Assets.Scripts.UserInput
{
    class ZoomController : MonoBehaviour
    {
        [Header("Scriptable object общих настроек ввода")]
        [Tooltip("Общие настройки ввода (например, ограничения зума).")]
        [SerializeField] private CommonInputSettings commonInputSettings;

        [Header("Настройки масштабирования камеры")]
        [Tooltip("Скорость приближения/отдаления колесом мыши.")]
        [SerializeField] private float zoomStep = 5.0f;


        [Tooltip("Основная камера, которой управляет скрипт")]
        [SerializeField] private Camera mainCamera;


        [Header("Слайдер, для zoom-а окна")]
        [Tooltip("Слайдер, для zoom-а окна")]
        [SerializeField] private Slider zoomSlider;

        private void Start()
        {
            if (zoomSlider == null)
            {
                Debug.LogError("Zoom Slider не привязан в инспекторе!");
            }
            else
            {
                Debug.Log("Zoom Slider привязан корректно.");
            }


        }

        /// <summary>
        /// Изменения значения позиции камеры при изменении значения слайдера-приближения. 
        /// </summary>
        /// <param name="value">новое значение</param>
        public void OnSliderValueChanged()
        {
            float value = zoomSlider.value;

            float leftZ = commonInputSettings.maxZoom;
            float rightZ = commonInputSettings.minZoom;

            float newZPOS = leftZ + (rightZ - leftZ) * value;
            mainCamera.transform.position = new Vector3(
                mainCamera.transform.position.x,
                mainCamera.transform.position.y,
                -newZPOS
            );
        }

        /// <summary>
        /// Изменяет уровень масштабирования камеры вдоль оси Z.
        /// Значение корректируется в пределах заданных ограничений minZoom и maxZoom,
        /// а значение слайдера обновляется для отображения текущего уровня масштаба.
        /// </summary>
        /// <param name="zoomDelta">Значение изменения зума вдоль оси Z (может быть положительным или отрицательным).</param>
        public void AdjustZoomByFloatValue(float zoomDelta)
        {
            // Вычисляем новое значение Z с учетом изменения
            float currentZ = mainCamera.transform.position.z;
            float newZ = currentZ + zoomDelta;

            // Ограничиваем новое значение Z в пределах допустимого диапазона
            float rightZ = -commonInputSettings.minZoom; // Ближайшая допустимая точка зума
            float leftZ = -commonInputSettings.maxZoom;  // Дальнейшая допустимая точка зума
            newZ = Mathf.Clamp(newZ, rightZ, leftZ);

            // Обновляем позицию камеры
            mainCamera.transform.position = new Vector3(
                mainCamera.transform.position.x,
                mainCamera.transform.position.y,
                newZ
            );

            // Обновляем значение слайдера
            if (zoomSlider != null)
            {
                float sliderValue = (leftZ - newZ) / (leftZ - rightZ);
                zoomSlider.value = sliderValue;
            }
            else
                Debug.LogError("Zoom Slider не привязан в инспекторе!");
        }

    }
}
