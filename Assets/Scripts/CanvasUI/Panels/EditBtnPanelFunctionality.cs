using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
{
    /// <summary>
    /// Скрипт отвечает за панель, появляющуюся при нажатии на кнопку "ППАВКА".
    /// </summary>
    public class EditBtnPanelFunctionality : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] private FamilyService familyService;
        [SerializeField] private TreeTraversal treeTraversal;

        [Header("Кнопки:")]
        [Tooltip("Кнопка поиска члена семьи по имени.")]    [SerializeField] Button findBtn;
        [Tooltip("Кнопка обзора древа целиком.")]           [SerializeField] Button showTreeBtn;


        void OnEnable()
        {
            
        }

        void OnDisable()
        {

        }

        
    }
}
