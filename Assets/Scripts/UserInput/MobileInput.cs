using UnityEngine;

namespace PrYFam.Assets.Scripts
{
    public class MobileInput : MonoBehaviour
    {
        public Camera mainCamera; // Камера, которую мы будем перемещать и масштабировать
        public float panSpeed = 0.25f; // Скорость перемещения камеры
        public float zoomSpeed = 0.1f; // Скорость зума 
        public float maxZoom = -10f; // Максимальное значение позиции камеры по оси Z (ближе)
        public float minZoom = -50f; // Минимальное значение позиции камеры по оси Z (дальше)
        public float zoomThreshold = 1f; // Минимальное изменение расстояния между пальцами для зума

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
                    Vector2 panDelta = currentPanPosition - lastPanPosition;

                    // ------------- Корректируем размер -------------
                    // Определяем расстояние до плоскости Canvas
                    float referenceDistance = 2f; // Плоскость Canvas находится на расстоянии 2 от камеры
                    float distanceFactor = Mathf.Abs(mainCamera.transform.position.z) / referenceDistance;

                    // Масштабируем panDelta в зависимости от distanceFactor
                    panDelta *= distanceFactor * panSpeed;
                    // -------------

                    // Перемещаем камеру в направлении, противоположном свайпу
                    mainCamera.transform.Translate(
                        -panDelta.x * Time.deltaTime,
                        -panDelta.y * Time.deltaTime,
                        0,
                        Space.World
                    );


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
                    lastTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    if (isZooming)
                    {
                        float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                        float distanceDelta = currentTouchDistance - lastTouchDistance;

                        // Проверяем, достаточно ли большое изменение расстояния для зума
                        if (Mathf.Abs(distanceDelta) > zoomThreshold)
                        {
                            // Масштабируем камеру, перемещая её по оси Z
                            float newZ = mainCamera.transform.position.z + distanceDelta * zoomSpeed * Time.deltaTime;
                            newZ = Mathf.Clamp(newZ, minZoom, maxZoom); // Ограничиваем Z в заданных пределах. Внимание!!! Эта функция принимает сначала значение поменьше, а затем побольше. В нашем случае minZoom = - 2; maxZoom = -80, canvas.Z = 0;
                            
                            
                            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x,
                                                                         mainCamera.transform.position.y,
                                                                         newZ);

                            lastTouchDistance = currentTouchDistance;   // обновляем расстояние
                        }
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
