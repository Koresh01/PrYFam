using UnityEngine;
using UnityEngine.UI;


namespace PrYFam
{
    /// <summary>
    /// Этот скрипт висит на карточке персонажа,
    /// и отвечает за логику всех кнопок на этой карточке.
    /// </summary>
    public class CardView : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] FamilyService familyService;
        [SerializeField] TreeTraversal treeTraversal;
        [SerializeField] CanvasView canvasView;
        
        [Header("Кнопки:")]
        [SerializeField] Button enter;
        [SerializeField] Button addParent;
        [SerializeField] Button addChild;
        [SerializeField] Button addHalf;

        [Tooltip("Кнопка включения панели детальной информации члена семьи.")]
        [SerializeField] Button showDetailedPanel;

        [Header("Каёмка для активной рамки:")]
        public GameObject DefaultBoundImage;
        public GameObject ActiveBoundImage;
        
        private void Awake() {
            // Префабам нельзя прокидывать сущьности со сцены.
            familyService = GameObject.FindAnyObjectByType<FamilyService>();
            treeTraversal = GameObject.FindAnyObjectByType<TreeTraversal>();
            canvasView = GameObject.FindAnyObjectByType<CanvasView>();
        }

        private void OnEnable() {
            enter.onClick.AddListener(() => {
                Vector2 curPos = transform.GetComponent<RectTransform>().anchoredPosition;
                Member cur = transform.GetComponent<Member>();
                treeTraversal.ReDrawTree(cur, curPos);
            });
            addParent.onClick.AddListener(() => {
                familyService.CreateMemberWithConnection(transform.gameObject, Relationship.ToParent);

                Vector2 curPos = transform.GetComponent<RectTransform>().anchoredPosition;
                Member cur = transform.GetComponent<Member>();
                treeTraversal.ReDrawTree(cur, curPos);
            });
            addChild.onClick.AddListener(() => {
                familyService.CreateMemberWithConnection(transform.gameObject, Relationship.ToChild);

                Vector2 curPos = transform.GetComponent<RectTransform>().anchoredPosition;
                Member cur = transform.GetComponent<Member>();
                treeTraversal.ReDrawTree(cur, curPos);
            });
            addHalf.onClick.AddListener(() => {
                familyService.CreateMemberWithConnection(transform.gameObject, Relationship.ToHalf);

                Vector2 curPos = transform.GetComponent<RectTransform>().anchoredPosition;
                Member cur = transform.GetComponent<Member>();
                treeTraversal.ReDrawTree(cur, curPos);
            });

            // отобразить панель детальной информации
            showDetailedPanel.onClick.AddListener(() =>
            {
                Vector2 curPos = transform.GetComponent<RectTransform>().anchoredPosition;
                Member cur = transform.GetComponent<Member>();
                treeTraversal.ReDrawTree(cur, curPos);


                canvasView.ShowDetailedPersonPanel();
            });
        }

        private void OnDisable() {
            enter.onClick.RemoveAllListeners();
            addChild.onClick.RemoveAllListeners();
            addParent.onClick.RemoveAllListeners();
            addHalf.onClick.RemoveAllListeners();

            showDetailedPanel.onClick.RemoveAllListeners();
        }
    }
}
