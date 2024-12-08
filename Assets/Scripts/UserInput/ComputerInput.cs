using PrYFam.Assets.Scripts;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// ��������� ������ � ���������� � ���� ��� ����������� � ��������������� ������.
    /// </summary>
    public class ComputerInput : MonoBehaviour
    {
        /// <summary>
        /// ����� ��������� ����� (��������, ����������� ����).
        /// </summary>
        [SerializeField] private CommonInputSettings commonInputSettings;

        /// <summary>
        /// ������, ������� ��������� ������.
        /// </summary>
        [Tooltip("�������� ������, ������� ��������� ������")]
        private Camera mainCamera;

        [Header("��������� ����������� ������")]
        [Tooltip("�������� ����������� ������ �� �����������.")]
        [SerializeField] private float movementSpeed = 5f;

        [Header("��������� ��������������� ������")]
        [Tooltip("�������� �����������/��������� ������� ����.")]
        [SerializeField] private float zoomStep = 5.0f;

        /// <summary>
        /// ������������� ������ �� ��������.
        /// </summary>
        private void Start()
        {
            mainCamera = commonInputSettings.mainCamera;
        }

        /// <summary>
        /// ������������ ���� � ���������� � ���� ������ ����.
        /// </summary>
        private void Update()
        {
            HandleCameraMovement();
            HandleMouseScroll();
        }

        /// <summary>
        /// ���������� ������ � ������� ������ WASD ��� �������.
        /// </summary>
        private void HandleCameraMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal"); // �������� �� ��� X
            float verticalInput = Input.GetAxis("Vertical");     // �������� �� ��� Y

            // ��������� �������� � ���������� ������
            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * movementSpeed * Time.deltaTime;
            mainCamera.transform.Translate(movement);
        }

        /// <summary>
        /// ������������ ��������������� ������ � ������� ������ ����.
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
        /// �������� �������� ������� ��������������� ������ ����� ��� Z � ��������� �����������.
        /// </summary>
        /// <param name="direction">����������� ��������� �������� (����������� ��� ���������).</param>
        public void AdjustZoomByStep(int zoomDirection)
        {
            //int zoomDirection = direction == Direction.AwayFromUs ? 1 : -1; // ���������� �����������
            float currentZ = mainCamera.transform.position.z;
            float newZ = currentZ + zoomDirection * zoomStep;

            float rightZoomX = -commonInputSettings.minZoom; // ��������� ���������� ����� ����.
            float leftZoomX = -commonInputSettings.maxZoom; // ���������� ���������� ����� ����.

            // ������������ ��� � �������� ���������� ��������
            if (newZ > leftZoomX || newZ < rightZoomX)
                return;

            // ��������� ������� ������
            Vector3 newPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, newZ);
            mainCamera.transform.position = newPosition;
        }

    }
}
