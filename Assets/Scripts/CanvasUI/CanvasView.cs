using UnityEngine;
using UnityEngine.UI;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Висит на canvas и отвечает за логику всего UI.
    /// </summary>
    class CanvasView : MonoBehaviour
    {
        [Header("Ссылки на скрипты:")]

        [SerializeField]
        [Tooltip("Скрипт, который обрабатывает свайпы/touches.")]
        MobileInput mobileInput;

        [SerializeField]
        [Tooltip("Скрипт, который обрабатывает компьютерный ввод.")]
        ComputerInput computerInput;

        [Header("Панель детальной информации о человеке:")]
        [SerializeField]
        [Tooltip("Панель детальной информации о человеке.")]
        GameObject detailedPanel;

        /// <summary>
        /// Включает панель
        /// детальной информации о человке.
        /// </summary>
        public void ShowDetailedPersonPanel()
        {
            // выключаем обработку свайпов и тачей
            mobileInput.enabled = false;
            computerInput.enabled = false;

            // Показываем панель детальной информации
            detailedPanel.SetActive(true);
        }

        /// <summary>
        /// Выключает панель
        /// детальной информации о человке.
        /// </summary>
        public void HideDetailedPersonPanel()
        {
            // включаем обработку свайпов и тачей
            mobileInput.enabled = true;
            computerInput.enabled = true;

            // Выключаем панель детальной информации
            detailedPanel.SetActive(false);
        }
    
        // public void CollectInfoFromDetailedPanel() собирает информацию из панели детализации.
    }
}
