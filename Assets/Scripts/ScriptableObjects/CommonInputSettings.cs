using UnityEngine;

namespace PrYFam
{
    [CreateAssetMenu(fileName = "CommonInputSettings", menuName = "CommonInputSettings")]
    public class CommonInputSettings : ScriptableObject
    {
        /// <summary>
        /// �������� ������� ������ �� ��� Z (������ �� ��������)
        /// </summary>
        public float minZoom = -50f;
        /// <summary>
        /// �������� ������� ������ �� ��� Z (����� � ���������)
        /// </summary>
        public float maxZoom = -2f;
    }
}
