using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam.Assets.Scripts.CanvasUI.Panels
{
    /// <summary>
    /// Скрпит который висит на панели детальной информации о члене семьи.
    /// Он реализует логику всех кнопочек на этой панели.
    /// Восновном он вызывает методы из canvas view.
    /// </summary>
    class DetailedPersonPanel : MonoBehaviour
    {
        [Header("Ссылка на canvasView")]
        [SerializeField] CanvasView canvasView;

        [Header("Кнопка закрытия панели детальной информации о члене семьи.")]
        [Tooltip("Крестик на панели детальной информации о человеке.")]
        [SerializeField] Button closeDetailedPanel;

        void OnEnable()
        {
            // обращается к root(активному) Member.
            // static :) Algorithms.singleton.root - это наш активный член семьи.
            // Берет данные из скрипта Member
            // Вводит их во все textboxes.

            closeDetailedPanel.onClick.AddListener(() =>
            {
                canvasView.HideDetailedPersonPanel();
            });
        }

        void OnDisable()
        {
            closeDetailedPanel.onClick.RemoveAllListeners();
        }
    }
}
