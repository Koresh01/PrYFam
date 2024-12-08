using UnityEngine;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Скрипт для обработки жестов на мобильных устройствах, включая перемещение камеры и зум.
    /// </summary>
    public class MobileInput : MonoBehaviour
    {
        [Header("Основные настройки")]
        [Tooltip("Камера, которую необходимо перемещать и масштабировать")]
        public Camera mainCamera;

        [Tooltip("Общие настройки ввода (например, ограничения зума)")]
        [SerializeField]
        private CommonInputSettings commonInputSettings;

        private Vector2 lastPanPosition; // Последняя позиция пальца для отслеживания перемещения
        private int panFingerId; // ID пальца, который используется для перемещения
        private bool isPanning; // Флаг, чтобы отслеживать состояние перемещения

        private float lastTouchDistance; // Расстояние между пальцами для зума
        private bool isZooming; // Флаг, чтобы отслеживать состояние зума

        void Update()
        {
            if (Input.touchCount == 1) // Обработка перемещения
            {
                HandlePanGesture();
            }
            else if (Input.touchCount == 2) // Обработка зума
            {
                HandleZoomGesture();
            }
        }

        /// <summary>
        /// Обрабатывает жест перемещения (один палец).
        /// </summary>
        private void HandlePanGesture()
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) // Начало касания
            {
                isPanning = true;
                panFingerId = touch.fingerId;
                lastPanPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning) // Перемещение пальца
            {
                Vector2 currentPanPosition = touch.position;

                // Вычисляем разницу в мировых координатах
                Vector3 worldDelta = CalculateWorldDelta(lastPanPosition, currentPanPosition);

                // Настраиваем масштаб перемещения
                worldDelta *= 70f;

                // Перемещаем камеру
                mainCamera.transform.Translate(-worldDelta.x, -worldDelta.y, 0, Space.World);

                lastPanPosition = currentPanPosition;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) // Завершение касания
            {
                isPanning = false;
            }
        }

        /// <summary>
        /// Обрабатывает жест масштабирования (два пальца).
        /// </summary>
        private void HandleZoomGesture()
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began) // Начало зума
            {
                isZooming = true;
                lastTouchDistance = CalculateTouchDistance(touch1.position, touch2.position);
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) // Перемещение пальцев
            {
                if (isZooming)
                {
                    float currentTouchDistance = CalculateTouchDistance(touch1.position, touch2.position);

                    // Разница в расстоянии между пальцами
                    float worldDelta = currentTouchDistance - lastTouchDistance;
                    worldDelta *= 70f;

                    // Настраиваем зум
                    ApplyZoom(worldDelta);

                    lastTouchDistance = currentTouchDistance; // Обновляем расстояние
                }
            }
            else if (touch1.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Canceled ||
                     touch2.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Canceled) // Завершение зума
            {
                isZooming = false;
            }
        }

        /// <summary>
        /// Вычисляет разницу(которая может быть отрицательной) между двумя точками касания в мировых координатах.
        /// </summary>
        /// <param name="startScreenPos">Начальная позиция на экране</param>
        /// <param name="endScreenPos">Конечная позиция на экране</param>
        /// <returns>Разница в мировых координатах</returns>
        private Vector3 CalculateWorldDelta(Vector2 startScreenPos, Vector2 endScreenPos)
        {
            Vector3 startWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(startScreenPos.x, startScreenPos.y, mainCamera.nearClipPlane));
            Vector3 endWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(endScreenPos.x, endScreenPos.y, mainCamera.nearClipPlane));
            return endWorldPos - startWorldPos;
        }

        /// <summary>
        /// Вычисляет расстояние(которое всегда положительно) между двумя пальцами в мировых координатах.
        /// </summary>
        /// <param name="pos1">Позиция первого пальца</param>
        /// <param name="pos2">Позиция второго пальца</param>
        /// <returns>Расстояние между пальцами</returns>
        private float CalculateTouchDistance(Vector2 pos1, Vector2 pos2)
        {
            Vector3 worldPos1 = mainCamera.ScreenToWorldPoint(new Vector3(pos1.x, pos1.y, mainCamera.nearClipPlane));
            Vector3 worldPos2 = mainCamera.ScreenToWorldPoint(new Vector3(pos2.x, pos2.y, mainCamera.nearClipPlane));
            return Vector3.Distance(worldPos1, worldPos2);
        }

        /// <summary>
        /// Применяет зум с учетом ограничений на минимальное и максимальное приближение.
        /// </summary>
        /// <param name="zoomDelta">Смещение зума</param>
        private void ApplyZoom(float zoomDelta)
        {
            Vector3 zoomVector = new Vector3(0, 0, zoomDelta);

            float newZ = mainCamera.transform.position.z + zoomVector.z;
            float minZoom = commonInputSettings.minZoom;
            float maxZoom = commonInputSettings.maxZoom;

            // Применяем ограничения
            if (newZ < minZoom)
            {
                zoomVector.z = minZoom - mainCamera.transform.position.z;
            }
            else if (newZ > maxZoom)
            {
                zoomVector.z = maxZoom - mainCamera.transform.position.z;
            }

            // Перемещаем камеру
            mainCamera.transform.Translate(zoomVector, Space.World);
        }
    }
}
