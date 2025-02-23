using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
{
    /// <summary>
    /// Скрипт перемещает камеру таким образом, чтобы все отображающиеся карточки попадали в кадр камеры.
    /// </summary>
    public class CameraAdjuster : MonoBehaviour
    {
        [Tooltip("Кнопка")]                                     [SerializeField] Button btn;


        [Header("Слайдер позиции камеры:")]
        [Tooltip("Слайдер, для zoom-а окна")]
        [SerializeField] private Slider zoomSlider;
        
        [Tooltip("Общие настройки ввода (например, ограничения зума).")]
        [SerializeField] private CommonInputSettings commonInputSettings;


        [Header("Настройки скрипта:")]
        [Tooltip("Камера")]                                     [SerializeField] Camera mainCamera;
        [Tooltip("Спавнер карточек")]                           [SerializeField] GameObject cardsPlaceholder;
        [Tooltip("Карточки членов семьи")]                      [SerializeField] List<GameObject> familyCards;
        [Tooltip("Запас по краям, чтобы древо не обрезалось")]  [SerializeField] float fixedPadding = 1f;


        void OnEnable()
        {
            btn.onClick.AddListener(moveCamera);
        }

        void OnDisable()
        {
            btn.onClick.RemoveAllListeners();
        }


        void moveCamera()
        {
            familyCards = GetFirstLevelChildren(cardsPlaceholder);
            MoveCamera();
            UpdateSliderValue();
        }

        /// <summary>
        /// Достает все карточки членов семьи, которые являются дочерними по отношению к cardsSpawner
        /// </summary>
        List<GameObject> GetFirstLevelChildren(GameObject parent)
        {
            List<GameObject> firstLevelChildren = new List<GameObject>();

            foreach (Transform child in parent.transform)
            {
                if (child.gameObject.activeSelf)
                    firstLevelChildren.Add(child.gameObject);
            }

            return firstLevelChildren;
        }

        /// <summary>
        /// Перемещает камеру.
        /// </summary>
        void MoveCamera()
        {
            if (familyCards.Count == 0) return;

            // Найдем границы всех карточек
            Vector3 minWorld = familyCards[0].transform.position;
            Vector3 maxWorld = familyCards[0].transform.position;

            foreach (var card in familyCards)
            {
                Vector3 worldPos = card.transform.position;

                minWorld.x = Mathf.Min(minWorld.x, worldPos.x);
                minWorld.y = Mathf.Min(minWorld.y, worldPos.y);
                maxWorld.x = Mathf.Max(maxWorld.x, worldPos.x);
                maxWorld.y = Mathf.Max(maxWorld.y, worldPos.y);
            }

            // Центр области
            Vector3 center = (minWorld + maxWorld) / 2f;

            // Вычисляем необходимый размер древа
            float width = maxWorld.x - minWorld.x;
            float height = maxWorld.y - minWorld.y;

            // Добавляем padding: фиксированный + процентный
            // float fixedPadding = 2f;  // Постоянный запас в юнитах
            float percentPadding = 0.1f * height; //Mathf.Max(width, height); // 10% от размера древа

            width += fixedPadding + percentPadding;
            height += fixedPadding + percentPadding;

            // Камера проецирует на экран перспективно, учитываем поле зрения
            float aspectRatio = Screen.width / (float)Screen.height;
            float cameraHeight = height / 2f;
            float cameraWidth = width / (2f * aspectRatio);

            // Берем большее из двух, чтобы полностью вместить древо
            float requiredDistance = Mathf.Max(cameraHeight, cameraWidth) / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            // Перемещаем камеру
            mainCamera.transform.position = new Vector3(center.x, center.y, -requiredDistance);
            mainCamera.transform.LookAt(center);
        }
        
        /// <summary>
        /// Обновляем значение слайдера ZoomSlider.
        /// </summary>
        void UpdateSliderValue()
        {
            // Ограничиваем новое значение Z в пределах допустимого диапазона
            float rightZ = -commonInputSettings.minZoom; // Ближайшая допустимая точка зума
            float leftZ = -commonInputSettings.maxZoom;  // Дальнейшая допустимая точка зума

            // Ограничиваем значение новой позиции
            float newZ = mainCamera.transform.position.z;

            // Обновляем значение слайдера
            if (zoomSlider != null)
            {
                float sliderValue = (leftZ - newZ) / (leftZ - rightZ);
                sliderValue = Mathf.Min(1, sliderValue);    // Просто иногда камера пытается выпрыгнуть за рамки допустимого максимального расстояния до карточек.
                zoomSlider.value = sliderValue;
            }
            else
                Debug.LogError("Zoom Slider не привязан в инспекторе!");
        }
    }
}
