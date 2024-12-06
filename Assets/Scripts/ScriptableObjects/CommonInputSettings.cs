using UnityEngine;

namespace PrYFam
{
    [CreateAssetMenu(fileName = "CommonInputSettings", menuName = "CommonInputSettings")]
    public class CommonInputSettings : ScriptableObject
    {
        /// <summary>
        /// Максимальное значение позиции камеры по оси Z (ближе)
        /// </summary>
        public float maxZoom = -10f;
        /// <summary>
        /// Минимальное значение позиции камеры по оси Z (дальше)
        /// </summary>
        public float minZoom = -50f;
    }
}
