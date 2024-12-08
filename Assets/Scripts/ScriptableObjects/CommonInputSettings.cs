using UnityEngine;

namespace PrYFam
{
    [CreateAssetMenu(fileName = "CommonInputSettings", menuName = "CommonInputSettings")]
    public class CommonInputSettings : ScriptableObject
    {
        [Header("Настройки камеры")]
        
        public Camera mainCamera;
        /// <summary>
        /// Значение позиции камеры по оси Z (дальше от карточек)
        /// </summary>
        public float minZoom = -50f;
        /// <summary>
        /// Значение позиции камеры по оси Z (ближе к карточкам)
        /// </summary>
        public float maxZoom = -2f;
    }
}
