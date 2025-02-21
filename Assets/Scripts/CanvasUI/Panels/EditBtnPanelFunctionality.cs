using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
{
    /// <summary>
    /// Скрипт отвечает за панель, появляющуюся при нажатии на кнопку "ППАВКА".
    /// </summary>
    public class EditBtnPanelFunctionality : MonoBehaviour
    {
        [Header("Кнопки:")]
        [Tooltip("Кнопка поиска члена семьи по имени.")]    [SerializeField] Button findBtn;
        [Tooltip("Кнопка обзора древа целиком.")]           [SerializeField] Button showTreeBtn;
    }
        
}
