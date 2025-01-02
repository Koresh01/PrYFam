using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// Перечисление для обозначения статуса выбранного человека.
    /// </summary>
    public enum ActivePersonStatus
    {
        [Tooltip("Карточка не является корневой, относительно которой происходит отрисовка.")]
        Disabled,

        [Tooltip("Карточка активна, но КруговоеМеню НЕ отображаем.")]
        DisabledRoundMenu,

        [Tooltip("Карточка активна, КруговоеМеню отображаем.")]
        ShowRoundMenu
    }
}
