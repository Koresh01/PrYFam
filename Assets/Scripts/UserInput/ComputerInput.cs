using PrYFam.Assets.Scripts;
using PrYFam.Assets.Scripts.UserInput;
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
        [Header("Scriptable object общих настроек ввода")]
        [SerializeField] private CommonInputSettings commonInputSettings;


        /// <summary>
        /// Скрпит отвечающий за приближение/отдажение.
        /// </summary>
        [Header("Скрпит отвечающий за приближение/отдажение:")]
        [SerializeField] private ZoomController zoomController;

        /// <summary>
        /// Камера, которой управляет скрипт.
        /// </summary>
        [Tooltip("Основная камера, которой управляет скрипт")]
        [SerializeField] private Camera mainCamera;

        [Header("Настройки перемещения камеры")]
        [Tooltip("Скорость перемещения камеры по поверхности.")]
        [SerializeField] private float movementSpeed = 5f;


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
                zoomController.AdjustZoomByFloatValue(+1f); // Direction.AwayFromUs
            if (scroll < 0f)
                zoomController.AdjustZoomByFloatValue(-1f); // Direction.TowardsUs
        }

    }
}
