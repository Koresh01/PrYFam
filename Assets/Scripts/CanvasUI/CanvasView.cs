using NUnit.Framework;
using System.Collections.Generic;
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
        [SerializeField] Button editBtn;

        [Header("Панели:")]
        [Tooltip("Панель детальной информации о человеке.")]    [SerializeField] GameObject detailedPanel;
        [Tooltip("Панель кнопки файл.")]                        [SerializeField] GameObject filePanel;
        [Tooltip("Панель кнопки правка.")]                      [SerializeField] GameObject editPanel;

        void OnEnable()
        {
            fileBtn.onClick.AddListener(() =>
            {
                TogglePanel(filePanel);
            });
            editBtn.onClick.AddListener(() =>
            {
                TogglePanel(editPanel);
            });
        }
        void OnDisable()
        {
            fileBtn.onClick.RemoveAllListeners();
            editBtn.onClick.RemoveAllListeners();
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
        /// Переключает видимость панели.
        /// </summary>
        /// <param name="panel">Панель, видимость которой надо переключить на противоположную.</param>
        public void TogglePanel(GameObject panel)
        {
            // Тушим предыдущую панель:
            List<GameObject> AllPanels = new List<GameObject>() { filePanel, editPanel };
            foreach (GameObject p in AllPanels)
            {
                if (p != panel)
                    p.SetActive(false);
            }

            // Отображаем/тушим текущую:
            if (panel != null)
            {
                // Переключаем видимость панели
                panel.SetActive(!panel.activeSelf);
            }
            else
            {
                Debug.LogWarning("filePanel не назначен в инспекторе!");
            }
        }
    }
}
