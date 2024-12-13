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


        [Tooltip("Основная камера, которой управляет скрипт")]
        [SerializeField] private Camera mainCamera;

        [Tooltip("Основная камера, которой управляет скрипт")]
        [SerializeField] private SensetivityChart sensetivityChart;

        [Header("Слайдер, для zoom-а окна")]
        [Tooltip("Слайдер, для zoom-а окна")]
        [SerializeField] private Slider zoomSlider;

        private void Start()
        {
            Debug.Log(zoomSlider == null ? "Zoom Slider не привязан в инспекторе!" : "Zoom Slider привязан корректно.");
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
        /// Смещает камеру вдоль очи Z, учитывая расстояние от камеры до карточек.
        /// </summary>
        /// <param name="zoomDelta">Значение на которое происходит смещение.</param>
        public void AdjustZoomByFloatValue(float zoomDelta)
        {
            // Вычисляем текущее значение Z (без учета нового изменения).
            float currentZ = mainCamera.transform.position.z;

            // Ограничиваем новое значение Z в пределах допустимого диапазона
            float rightZ = -commonInputSettings.minZoom; // Ближайшая допустимая точка зума
            float leftZ = -commonInputSettings.maxZoom;  // Дальнейшая допустимая точка зума
            

            // Применяем множитель f(z):
            float multiplier = sensetivityChart.GetZoomMultiplier(mainCamera.transform.position.z);
            
            // Константа 50f определяет наскольно сильно мы гасим чувсвительность жеста zoom щупанья пальцами экрана.
            multiplier /= 50f;

            // Вычисляем новое значение Z.
            float newZ = currentZ + zoomDelta*multiplier;

            // Debug.Log($"f(z): {multiplier}");

            // Ограничиваем значение новой позиции
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
