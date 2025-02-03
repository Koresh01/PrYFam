using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
{
    /// <summary>
    /// Контроллер всплывающих информационных изображений для пользователей.
    /// </summary>
    class WarningPanelsController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> panels;

        // Singleton-экземпляр
        public static WarningPanelsController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            foreach (GameObject panel in panels)
            {
                Button closeBtn = panel.GetComponentInChildren<Button>();
                if (closeBtn != null)
                {
                    // Используем лямбда-выражение для передачи параметра
                    closeBtn.onClick.AddListener(() => ClosePanel(panel));
                }
                else
                {
                    Debug.LogWarning($"На панели {panel.name} нет компонента Button!");
                }
            }
        }

        private GameObject GetPanel(string name)
        {
            foreach (GameObject panel in panels)
            {
                if (panel.name == name)
                    return panel;
            }

            throw new ArgumentException("Не нашли панель с таким именем");
        }

        private void ClosePanel(GameObject panel)
        {
            panel.SetActive(false);
        }
        /// <summary>
        /// Закрывает панель.
        /// </summary>
        /// <param name="name">Название панели.</param>
        public static void ClosePanel(string name)
        {
            if (Instance == null)
            {
                Debug.LogError("WarningPanelsController не инициализирован!");
                return;
            }

            GameObject panel = Instance.GetPanel(name);
            panel.SetActive(false);
        }

        /// <summary>
        /// Показывает панель с указанным именем.
        /// </summary>
        /// <param name="name">Имя панели.</param>
        public static void ShowPanel(string name)
        {
            if (Instance == null)
            {
                Debug.LogError("WarningPanelsController не инициализирован!");
                return;
            }

            GameObject panel = Instance.GetPanel(name);
            panel.SetActive(true);
        }
    }
}