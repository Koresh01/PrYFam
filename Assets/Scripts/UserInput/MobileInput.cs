using PrYFam.Assets.Scripts.UserInput;
using System;
using UnityEngine;

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

        /// <summary>
        /// Скрпит отвечающий за приближение/отдажение.
        /// </summary>
        [Header("Скрпит отвечающий за приближение/отдажение:")]
        [SerializeField] private ZoomController zoomController;

        [Tooltip("Основная камера, которой управляет скрипт")]
        [SerializeField] private Camera mainCamera;

        [Header("График множителя")]
        [Tooltip("График зависимости множителя x от позиции камеры z")]
        public AnimationCurve zoomMultiplierCurve;

        private Vector2 lastPanPosition;
        private int panFingerId;
        private bool isPanning;

        private float lastTouchDistance;
        private bool isZooming;

        private void Start()
        {
            InitGraphic();
            
        }

        void Update()
        {
            if (Input.touchCount == 1) // Перемещение
            {
                HandlePanGesture();
            }
            else if (Input.touchCount == 2) // Зум
            {
                HandleZoomGesture();
            }
        }

        #region input
        
        /// <summary>
        /// Обрабатывает жест перемещения камеры.
        /// </summary>
        private void HandlePanGesture()
        {
            Touch touch = Input.GetTouch(0);

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

                // Применяем множитель x(z)
                float multiplier = GetZoomMultiplier(mainCamera.transform.position.z);
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
        private void HandleZoomGesture()
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

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

        #endregion



        #region touchMultiplier

        /// <summary>
        /// Создаёт график зависимости множителя свайпа от расстояния камеры ДО карточек.
        /// </summary>
        private void InitGraphic()
        {
            float leftZoomX = commonInputSettings.maxZoom;
            float rightZoomX = commonInputSettings.minZoom;

            // Создаём линейную зависимость для множителя multiplier.

            // Коэффициент 3f определяет насколько сильнее будет умножаться разница между пальцами.
            float maxValue = rightZoomX * 3f;
            float minValue = leftZoomX * 3f;
            zoomMultiplierCurve = CreateLinearZoomCurve(leftZoomX, rightZoomX, minValue, maxValue);
        }

        /// <summary>
        /// Создаёт линейную зависимость для AnimationCurve между двумя точками.
        /// </summary>
        /// <param name="minZoom">Минимальное значение z (например, самое далёкое положение камеры).</param>
        /// <param name="maxZoom">Максимальное значение z (например, самое близкое положение камеры).</param>
        /// <param name="maxValue">Значение функции при максимальном z.</param>
        /// <param name="minValue">Значение функции при минимальном z.</param>
        /// <returns>Линейная AnimationCurve между заданными точками.</returns>
        private AnimationCurve CreateLinearZoomCurve(float leftZoomX, float rightZoomX, float minValue, float maxValue)
        {
            // Создаём новую AnimationCurve
            AnimationCurve curve = new AnimationCurve();

            // Добавляем ключи
            Keyframe firstKey = new Keyframe(leftZoomX, minValue);
            Keyframe secondKey = new Keyframe(rightZoomX, maxValue);

            // Вычисляем тангенсы для линейной зависимости
            float tangent = (maxValue - minValue) / (rightZoomX - leftZoomX);


            firstKey.inTangent = 0;           // Входной тангенс начальной точки
            firstKey.outTangent = tangent;          // Выходной тангенс начальной точки
            secondKey.inTangent = tangent;          // Входной тангенс конечной точки
            secondKey.outTangent = 0;         // Выходной тангенс конечной точки

            // Добавляем ключи в график
            curve.AddKey(firstKey);
            curve.AddKey(secondKey);

            return curve;
        }

        /// <summary>
        /// Получает множитель x(z) на основе позиции камеры.
        /// </summary>
        /// <param name="z">Текущее значение z камеры</param>
        /// <returns>Значение множителя</returns>
        private float GetZoomMultiplier(float z)
        {
            return zoomMultiplierCurve.Evaluate(-z);    // т.к. позиция камеры по оси Z у нас отрицательная.
        }
        
        #endregion
    }
}
