using PrYFam.Assets.Scripts;
using PrYFam.Assets.Scripts.UserInput;
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
        [Header("Scriptable object ����� �������� �����")]
        [SerializeField] private CommonInputSettings commonInputSettings;


        /// <summary>
        /// ������ ���������� �� �����������/���������.
        /// </summary>
        [Header("������ ���������� �� �����������/���������:")]
        [SerializeField] private ZoomController zoomController;

        /// <summary>
        /// ������, ������� ��������� ������.
        /// </summary>
        [Tooltip("�������� ������, ������� ��������� ������")]
        [SerializeField] private Camera mainCamera;

        [Header("��������� ����������� ������")]
        [Tooltip("�������� ����������� ������ �� �����������.")]
        [SerializeField] private float movementSpeed = 5f;


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
                zoomController.AdjustZoomByFloatValue(+1f); // Direction.AwayFromUs
            if (scroll < 0f)
                zoomController.AdjustZoomByFloatValue(-1f); // Direction.TowardsUs
        }

    }
}
