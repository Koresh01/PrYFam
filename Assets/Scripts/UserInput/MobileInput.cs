using UnityEngine;

namespace PrYFam.Assets.Scripts
{
    public class MobileInput : MonoBehaviour
    {
        public Camera mainCamera; // Камера, которую мы будем перемещать и масштабировать
                                  //public float panSpeed = 0.25f; // Скорость перемещения камеры
                                  //public float zoomSpeed = 0.1f; // Скорость зума 
        [SerializeField] CommonInputSettings commonInputSettings;        
       

        private Vector2 lastPanPosition; // Последняя позиция пальца для отслеживания перемещения
        private int panFingerId; // ID пальца, который используется для перемещения
        private bool isPanning; // Флаг, чтобы отслеживать состояние перемещения

        private float lastTouchDistance; // Расстояние между пальцами для зума
        private bool isZooming; // Флаг, чтобы отслеживать состояние зума

        void Update()
        {
            if (Input.touchCount == 1) // Если на экране один палец
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

                    // Разница в пикселях на экране
                    Vector2 panDelta = currentPanPosition - lastPanPosition;

                    // Переводим разницу в мировые координаты
                    Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(currentPanPosition.x, currentPanPosition.y, mainCamera.nearClipPlane)) -
                                         mainCamera.ScreenToWorldPoint(new Vector3(lastPanPosition.x, lastPanPosition.y, mainCamera.nearClipPlane));

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
            else if (Input.touchCount == 2) // Если на экране два пальца
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    isZooming = true;
                    // Инициалзируем рассточние между пальцами в начале выполнения жеста "Приближение/Отдаление":
                    lastTouchDistance = Vector2.Distance(
                        mainCamera.ScreenToWorldPoint(new Vector3(touch1.position.x, touch1.position.y, mainCamera.nearClipPlane)),
                        mainCamera.ScreenToWorldPoint(new Vector3(touch2.position.x, touch2.position.y, mainCamera.nearClipPlane))
                    );
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    if (isZooming)
                    {
                        // Узнаём новое рассточние между пальцами в момент, когда пальцы перемещаются:
                        float currentTouchDistance = Vector2.Distance(
                            mainCamera.ScreenToWorldPoint(new Vector3(touch1.position.x, touch1.position.y, mainCamera.nearClipPlane)),
                            mainCamera.ScreenToWorldPoint(new Vector3(touch2.position.x, touch2.position.y, mainCamera.nearClipPlane))
                        );
                        
                        // Разница между разницами этих самых расстояний:
                        float worldDelta = currentTouchDistance - lastTouchDistance;    // это ооочень маленькая величина.
                        worldDelta *= 70f;

                        // Вычисляем смещение камеры по оси Z
                        Vector3 zoomDelta = new Vector3(0, 0, worldDelta);

                        // Применяем ограничение, преобразовав новую позицию камеры
                        float newZ = mainCamera.transform.position.z + zoomDelta.z;
                        float minZoom = commonInputSettings.minZoom;
                        float maxZoom = commonInputSettings.maxZoom;

                        // Если смещение выводит камеру за пределы зума, ограничиваем его
                        if (newZ < minZoom)
                        {
                            zoomDelta.z = minZoom - mainCamera.transform.position.z;
                        }
                        else if (newZ > maxZoom)
                        {
                            zoomDelta.z = maxZoom - mainCamera.transform.position.z;
                        }

                        // Перемещаем камеру с учетом ограничений
                        mainCamera.transform.Translate(zoomDelta, Space.World);

                        lastTouchDistance = currentTouchDistance;   // обновляем расстояние
                    }
                }
                else if (touch1.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Canceled ||
                         touch2.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Canceled)
                {
                    isZooming = false;
                }
            }
        }
    }
}
