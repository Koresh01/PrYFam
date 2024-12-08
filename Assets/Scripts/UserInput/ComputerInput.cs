using PrYFam.Assets.Scripts;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// Управляет вводом с клавиатуры и мыши для перемещения и масштабирования камеры.
    /// </summary>
    public class ComputerInput : MonoBehaviour
    {
        /// <summary>
        /// Общие настройки ввода (например, ограничения зума).
        /// </summary>
        [SerializeField] private CommonInputSettings commonInputSettings;

        /// <summary>
        /// Камера, которой управляет скрипт.
        /// </summary>
        [Tooltip("Основная камера, которой управляет скрипт")]
        private Camera mainCamera;

        [Header("Настройки перемещения камеры")]
        [Tooltip("Скорость перемещения камеры по поверхности.")]
        [SerializeField] private float movementSpeed = 5f;

        [Header("Настройки масштабирования камеры")]
        [Tooltip("Скорость приближения/отдаления колесом мыши.")]
        [SerializeField] private float zoomStep = 5.0f;

        /// <summary>
        /// Инициализация камеры из настроек.
        /// </summary>
        private void Start()
        {
            mainCamera = commonInputSettings.mainCamera;
        }

        /// <summary>
        /// Обрабатывает ввод с клавиатуры и мыши каждый кадр.
        /// </summary>
        private void Update()
        {
            HandleCameraMovement();
            HandleMouseScroll();
        }

        /// <summary>
        /// Перемещает камеру с помощью клавиш WASD или стрелок.
        /// </summary>
        private void HandleCameraMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal"); // Движение по оси X
            float verticalInput = Input.GetAxis("Vertical");     // Движение по оси Y

            // Вычисляем смещение и перемещаем камеру
            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * movementSpeed * Time.deltaTime;
            mainCamera.transform.Translate(movement);
        }

        /// <summary>
        /// Обрабатывает масштабирование камеры с помощью колеса мыши.
        /// </summary>
        private void HandleMouseScroll()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll > 0f)
                AdjustZoomByStep(+1); // Direction.AwayFromUs
            if (scroll < 0f)
                AdjustZoomByStep(-1); // Direction.TowardsUs
        }

        /// <summary>
        /// Пошагово изменяет уровень масштабирования камеры вдоль оси Z в указанном направлении.
        /// </summary>
        /// <param name="direction">Направление изменения масштаба (приближение или отдаление).</param>
        public void AdjustZoomByStep(int zoomDirection)
        {
            //int zoomDirection = direction == Direction.AwayFromUs ? 1 : -1; // Определяем направление
            float currentZ = mainCamera.transform.position.z;
            float newZ = currentZ + zoomDirection * zoomStep;

            float rightZoomX = -commonInputSettings.minZoom; // Ближайшая допустимая точка зума.
            float leftZoomX = -commonInputSettings.maxZoom; // Дальнейшая допустимая точка зума.

            // Ограничиваем зум в пределах допустимых значений
            if (newZ > leftZoomX || newZ < rightZoomX)
                return;

            // Обновляем позицию камеры
            Vector3 newPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, newZ);
            mainCamera.transform.position = newPosition;
        }

    }
}
