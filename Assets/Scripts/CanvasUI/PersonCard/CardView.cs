using UnityEngine;
using UnityEngine.UI;


namespace PrYFam
{
    /// <summary>
    /// ���� ������ ����� �� �������� ���������,
    /// � �������� �� ������ ���� ������ �� ���� ��������.
    /// </summary>
    public class CardView : MonoBehaviour
    {
        [Header("�����������:")]
        [SerializeField] FamilyService familyService;
        [SerializeField] TreeTraversal treeTraversal;
        [SerializeField] CanvasView canvasView;
        
        [Header("������:")]
        [SerializeField] Button enter;
        [SerializeField] Button addParent;
        [SerializeField] Button addChild;
        [SerializeField] Button addHalf;

        [Tooltip("������ ��������� ������ ��������� ���������� ����� �����.")]
        [SerializeField] Button showDetailedPanel;

        [Header("����� ��� �������� �����:")]
        public GameObject DefaultBoundImage;
        public GameObject ActiveBoundImage;
        
        private void Awake() {
            // �������� ������ ����������� ��������� �� �����.
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

            // ���������� ������ ��������� ����������
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
