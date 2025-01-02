using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
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

        [Header("���� ��� �� ������� ������� ��������.")]
        public TextMeshProUGUI FIO;

        [Header("�������� ����:")]
        [SerializeField] GameObject roundMenu;
        private ActivePersonStatus activePersonStatus = ActivePersonStatus.Disabled;

        [Header("������:")]
        [SerializeField] Button enter;
        [SerializeField] Button addParent;
        [SerializeField] Button addChild;
        [SerializeField] Button addHalf;
        [Tooltip("������ �������� ����� �����.")]
        [SerializeField] Button delete;
        [Tooltip("������ ����� ����.")]
        [SerializeField] Button changeWife;

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

                HandleTreeRedraw();
            });
            addParent.onClick.AddListener(() => {
                familyService.AddNewMember(transform.gameObject, Relationship.ToParent);
                HandleTreeRedraw();
            });
            addChild.onClick.AddListener(() => {
                familyService.AddNewMember(transform.gameObject, Relationship.ToChild);
                HandleTreeRedraw();
            });
            addHalf.onClick.AddListener(() => {
                familyService.AddNewMember(transform.gameObject, Relationship.ToHalf);
                HandleTreeRedraw();
            });

            // ���������� ������ ��������� ����������
            showDetailedPanel.onClick.AddListener(() =>
            {
                HandleTreeRedraw();
                canvasView.ShowDetailedPersonPanel();

                roundMenu.SetActive(false);
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

            // ����� ����
            changeWife.onClick.AddListener(() =>
            {
                SelectedWifeController wifeController = transform.GetComponent<SelectedWifeController>();   // ����� �������� ������ ������ ����
                Member current = transform.GetComponent<Member>();              // ���� ���� �� ������� ��������� �������
                List<Member> wifes = familyService.GetHalfMembers(current);     // ������ ���� ��� ��
                wifeController.NextWife(wifes);     // ������� ������

                HandleTreeRedraw();
            });
        }

        private void OnDisable()
        {
            RemoveAllListeners(enter, addChild, addParent, addHalf, delete, showDetailedPanel, changeWife);
        }

        /// <summary>
        /// ��������������� ����� ��� ���������� ������
        /// </summary>
        public void HandleTreeRedraw()
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
