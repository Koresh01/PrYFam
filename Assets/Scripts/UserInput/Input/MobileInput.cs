using PrYFam.Assets.Scripts.UserInput;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Скрипт для управления масштабированием камеры с использованием графика зависимости множителя x(z).
    /// </summary>
    public class MobileInput : MonoBehaviour
    {
        [Tooltip("Общие настройки ввода")]
        [SerializeField]
        private CommonInputSettings commonInputSettings;

        [Tooltip("Основная камера, которой управляет скрипт")]
        [SerializeField] private Camera mainCamera;

        /// <summary>
        /// Скрпит отвечающий за приближение/отдажение.
        /// </summary>
        [Header("Скрпит отвечающий за приближение/отдажение:")]
        [SerializeField] private ZoomController zoomController;

        [Tooltip("Основная камера, которой управляет скрипт")]
        [SerializeField] private SensetivityChart sensetivityChart;

        [Header("Область предназначенная для касаний:")]
        [Tooltip("Область предназначенная для касаний:")]
        [SerializeField] private RectTransform touchZone;



        private Vector2 lastPanPosition;
        private int panFingerId;
        private bool isPanning;

        private float lastTouchDistance;
        private bool isZooming;

        void Update()
        {
            if (Input.touchCount == 1) // Перемещение
            {
                Touch touch = Input.GetTouch(0);
                // Проверяем, попадает ли касание на слой "touchplace"
                if (IsOnTouchingArea(touch))
                {
                    HandlePanGesture(touch); // Передаем касание в обработчик перемещения
                }
            }
            else if (Input.touchCount == 2) // Зум
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (IsOnTouchingArea(touch1) && IsOnTouchingArea(touch2))
                {
                    HandleZoomGesture(touch1, touch2); // Передаем касание в обработчик перемещения
                }
            }
        }

        /// <summary>
        /// Проверка, попадает ли касание на заданный RectTransform.
        /// </summary>
        bool IsOnTouchingArea(Touch touch)
        {
            // Преобразуем позицию касания из экранных координат в мировые
            Vector2 touchPosition = touch.position;

            // Проверяем, находится ли точка внутри RectTransform
            return RectTransformUtility.RectangleContainsScreenPoint(touchZone, touchPosition, mainCamera);
        }

        /// <summary>
        /// Обрабатывает жест перемещения камеры.
        /// </summary>
        private void HandlePanGesture(Touch touch)
        {
            //Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isPanning = true;
                panFingerId = touch.fingerId;
                lastPanPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                Vector2 currentPanPosition = touch.position;
                Vector3 worldDelta = CalculateWorldDelta(lastPanPosition, currentPanPosition);

                // Применяем множитель x(z):
                float multiplier = sensetivityChart.GetZoomMultiplier(mainCamera.transform.position.z);
                worldDelta *= multiplier;

                mainCamera.transform.Translate(-worldDelta.x, -worldDelta.y, 0, Space.World);
                lastPanPosition = currentPanPosition;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }
        }

        /// <summary>
        /// Обрабатывает жест масштабирования камеры.
        /// </summary>
        private void HandleZoomGesture(Touch touch1, Touch touch2)
        {            
            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                isZooming = true;
                lastTouchDistance = CalculateTouchDistance(touch1.position, touch2.position);
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                if (isZooming)
                {
                    float currentTouchDistance = CalculateTouchDistance(touch1.position, touch2.position);  // Обратите внимание: 10 пикселей на экране и 10 единиц в игровом мире — это разные масштабы.
                    float delta = currentTouchDistance - lastTouchDistance;

                    // Преобразуем разницу расстояния в экранных координатах в условное изменение масштаба в мировых координатах.
                    // Обратите внимание: 10 пикселей на экране и 10 единиц в игровом мире — это разные масштабы.
                    // Константа 18f определяет, насколько сильно пиксельные изменения влияют на изменение масштаба.
                    delta /= 18f;

                    // Настраиваем зум
                    zoomController.AdjustZoomByFloatValue(delta);
                    lastTouchDistance = currentTouchDistance;
                }
            }
            else if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Canceled)
            {
                isZooming = false;
            }
        }

        /// <summary>
        /// Вычисляет разницу(которая может быть и отрицательной) между двумя точками в мировых координатах.
        /// </summary>
        private Vector3 CalculateWorldDelta(Vector2 startScreenPos, Vector2 endScreenPos)
        {
            Vector3 startWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(startScreenPos.x, startScreenPos.y, mainCamera.nearClipPlane));
            Vector3 endWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(endScreenPos.x, endScreenPos.y, mainCamera.nearClipPlane));
            return endWorldPos - startWorldPos;
        }

        /// <summary>
        /// Вычисляет расстояние(только положительное значение) между двумя точками на экране.
        /// </summary>
        /// <returns>
        /// Значение в пикселях.
        /// </returns>
        private float CalculateTouchDistance(Vector2 pos1, Vector2 pos2)
        {
            return Vector2.Distance(pos1, pos2);
        }
    }
}
