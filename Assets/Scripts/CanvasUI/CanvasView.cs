using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
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

        [Header("Кнопки:")]
        [SerializeField] Button fileBtn;

        [Header("Панели:")]
        [Tooltip("Панель детальной информации о человеке.")]    [SerializeField] GameObject detailedPanel;
        [Tooltip("Панель кнопки файл.")]                        [SerializeField] GameObject filePanel;

        void OnEnable()
        {
            fileBtn.onClick.AddListener(() =>
            {
                ToggleFilePanel();
            });
        }
        void OnDisable()
        {
            fileBtn.onClick.RemoveAllListeners();
        }







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





        /// <summary>
        /// Переключает видимость filePanel.
        /// </summary>
        public void ToggleFilePanel()
        {
            if (filePanel != null)
            {
                // Переключаем видимость панели
                filePanel.SetActive(!filePanel.activeSelf);
            }
            else
            {
                Debug.LogWarning("filePanel не назначен в инспекторе!");
            }
        }
    }
}
