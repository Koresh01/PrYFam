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
        [Tooltip("������ �������� ����� �����.")]
        [SerializeField] Button delete;

        [Tooltip("������ ��������� ������ ��������� ���������� ����� �����.")]
        [SerializeField] Button showDetailedPanel;

        [Header("����� ��� �������� �����:")]
        public GameObject DefaultBoundImage;
        public GameObject ActiveBoundImage;

        private void Awake()
        {
            // �������� ������ ����������� ��������� �� �����.
            familyService = GameObject.FindAnyObjectByType<FamilyService>();
            treeTraversal = GameObject.FindAnyObjectByType<TreeTraversal>();
            canvasView = GameObject.FindAnyObjectByType<CanvasView>();
        }

        private void OnEnable()
        {
            enter.onClick.AddListener(() => {
                HandleTreeRedraw();
            });
            addParent.onClick.AddListener(() => {
                familyService.CreateMemberWithConnection(transform.gameObject, Relationship.ToParent);
                HandleTreeRedraw();
            });
            addChild.onClick.AddListener(() => {
                familyService.CreateMemberWithConnection(transform.gameObject, Relationship.ToChild);
                HandleTreeRedraw();
            });
            addHalf.onClick.AddListener(() => {
                familyService.CreateMemberWithConnection(transform.gameObject, Relationship.ToHalf);
                HandleTreeRedraw();
            });

            // ���������� ������ ��������� ����������
            showDetailedPanel.onClick.AddListener(() =>
            {
                HandleTreeRedraw();
                canvasView.ShowDetailedPersonPanel();
            });

            // �������� ����� �����:
            delete.onClick.AddListener(() =>
            {
                Member cur = transform.GetComponent<Member>();

                // ���� ��������� ���� ����� �������� �������(��������)
                if (familyService.IsLeaf(cur))
                {
                    familyService.DeletePerson(cur);
                    Destroy(gameObject);
                }
            });
        }

        private void OnDisable()
        {
            RemoveAllListeners(enter, addChild, addParent, addHalf, delete, showDetailedPanel);
        }

        // ��������������� ����� ��� ���������� ������
        private void HandleTreeRedraw()
        {
            Vector2 curPos = transform.GetComponent<RectTransform>().anchoredPosition;
            Member cur = transform.GetComponent<Member>();
            treeTraversal.ReDrawTree(cur, curPos);
        }

        // �������� ���� ���������� � ������
        private void RemoveAllListeners(params Button[] buttons)
        {
            foreach (var button in buttons)
            {
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                }
            }
        }
    }
}
