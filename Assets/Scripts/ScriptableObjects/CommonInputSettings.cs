using UnityEngine;

namespace PrYFam
{
    [CreateAssetMenu(fileName = "CommonInputSettings", menuName = "CommonInputSettings")]
    public class CommonInputSettings : ScriptableObject
    {
        /// <summary>
        /// «начение позиции камеры по оси Z (дальше от карточек)
        /// </summary>
        public float minZoom = -50f;
        /// <summary>
        /// «начение позиции камеры по оси Z (ближе к карточкам)
        /// </summary>
        public float maxZoom = -2f;
    }
}
