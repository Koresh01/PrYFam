using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
{
    /// <summary>
    /// Скрипт висит на карточке персонажа и отвечает 
    /// за логику появления кругового меню над карточкой 
    /// члена семьи.
    /// </summary>
    class RoundMenuController : MonoBehaviour
    {
        [Header("Кнопки:")]
        [SerializeField] Button enter;

        [Header("Круговое меню:")]
        [SerializeField] GameObject roundMenu;
        [SerializeField] ActivePersonStatus activePersonStatus = ActivePersonStatus.Disabled;


        private void OnEnable()
        {
            enter.onClick.AddListener(() =>
            {
                if (activePersonStatus == ActivePersonStatus.Disabled)
                {
                    activePersonStatus = ActivePersonStatus.DisabledRoundMenu;
                    roundMenu.SetActive(false);
                }

                else if (activePersonStatus == ActivePersonStatus.DisabledRoundMenu)
                {
                    activePersonStatus = ActivePersonStatus.ShowRoundMenu;
                    roundMenu.SetActive(true);
                }

                else if (activePersonStatus == ActivePersonStatus.ShowRoundMenu)
                {
                    activePersonStatus = ActivePersonStatus.Disabled;
                    roundMenu.SetActive(false);
                }
            });
        }

        private void OnDisable()
        {
            enter.onClick.RemoveAllListeners();
        }
    }
}
