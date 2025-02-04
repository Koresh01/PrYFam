using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace PrYFam
{
    /// <summary>
    /// ������������� ���� � ���������� ����������. ��� ������� ������ ���������� �� ���������.
    /// </summary>
    public class TreeTraversal : MonoBehaviour
    {
        [Header("�����������:")]
        [SerializeField] private FamilyService familyService;
        [SerializeField] private LinesController linesController;
        [SerializeField] private DetailedPersonPanel deltailedPersonPanel;

        [Header("�������:")]
        public float CardHeight;
        public float CardWidth;
        [Range(0f, 400f)] public float GlobalTreeOffset;    // ���������� ����� 2��� ����������.

        [Header("������ ���������� ������ ��������:")]
        [SerializeField] Dictionary<Member, Vector2> coordinates;


        private void Awake()
        {
            CardHeight = familyService.personCardPrefab.GetComponent<RectTransform>().sizeDelta.y;
            CardWidth = familyService.personCardPrefab.GetComponent<RectTransform>().sizeDelta.x;

            CardWidth += GlobalTreeOffset;
            CardHeight += GlobalTreeOffset;
        }

        public void ReDrawTree(Member root, Vector2 basePosition)
        {
            // 1.
            FadeCards();
            FadeSelectedCardBound(root);

            // 2. ������� ������ ���������:
            coordinates = Algorithms.Singleton.ReCalculate(
                root,
                familyService,
                basePosition,
                CardWidth,
                CardHeight
            );

            // 3.
            hardRepositionCards();  // - ������

            // 4. ��������� �����
            ReDrawLines();
        }

        /// <summary> �������������� ��� ����� ����� ������. </summary>
        private void ReDrawLines()
        {
            linesController.delAllLines();
            foreach (var pair in familyService.familyData.relationships)
            {
                Member from = pair.From;
                Member to = pair.To;
                Relationship relationship = pair.Relationship;

                if (!(coordinates.ContainsKey(from) && coordinates.ContainsKey(to))) continue;

                if (relationship == Relationship.ToChild) {
                    if (familyService.hasHalf(from))
                    {
                        Member half = familyService.GetSelectedHalf(from);
                        linesController.DrawMergedLine(from.gameObject, half.gameObject, to.gameObject);
                    }
                    if (!familyService.hasHalf(from))
                    {
                        linesController.DrawPolyLine(from.gameObject, to.gameObject);
                    }
                }

                
                // �������� ���� � ��� ���������� ����� � ����, ���� ���� ���� ��� �� ���������.
                if (relationship == Relationship.ToHalf && familyService.GetChildMembers(from).Count == 0)
                {
                    linesController.DrawLineToHalf(from.gameObject, to.gameObject);
                }
            }
        }
        /// <summary> �������� �������� ���� ���������� � ����� ��. </summary>
        private void FadeCards() {
            List<GameObject> personCards = familyService.familyData.GetAllPersonCards();
            foreach (var card in personCards)
            {
                card.SetActive(false);
            }
                
        }
        /// <summary> ����� ��� ����� � ������������ ������ ��� root. </summary>
        private void FadeSelectedCardBound(Member root) {
            List<GameObject> personCards = familyService.familyData.GetAllPersonCards();
            foreach (GameObject card in personCards)
            {
                Member memeber = card.GetComponent<Member>();
                CardView cardView = card.gameObject.GetComponent<CardView>();

                if (memeber == root)
                {
                    cardView.ActiveBoundImage.SetActive(true);
                    cardView.DefaultBoundImage.SetActive(false);
                }
                if (memeber != root)
                {
                    cardView.ActiveBoundImage.SetActive(false);
                    cardView.DefaultBoundImage.SetActive(true);
                }
            }
                
        }

        /// <summary> ��������� ������� � RectTransform ������� ����� �����. </summary>
        public void hardRepositionCards() {
            foreach (Member member in coordinates.Keys)
            {
                var memberGO = member.gameObject;
                
                // ����������� ��������(��� ��� ������� ����� ������������ DFS � �������� ��� ���������� ���������)
                memberGO.SetActive(true);

                if (memberGO.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    rectTransform.anchoredPosition = coordinates[member];
                }
            }

            // ��������� ������� ������ ��������, ��������� �� ������ �� Member.cs:
            foreach (Member member in coordinates.Keys)
            {
                // ��������� ����������� �� ��������
                Transform faceSpriteTransform = member.transform.Find("Environment/Image (Face Sprite)");
                Image faceSpriteImage = faceSpriteTransform.GetComponent<Image>();
                faceSpriteImage.sprite = member.ProfilePicture;

                // � ����� ������ ��� �� ������� ������� ��������
                CardView cardView = member.GetComponent<CardView>();
                string FIO = member.LastName + " " + member.FirstName + " " + member.MiddleName;
                if (FIO == "  ")
                    cardView.FIO.text = "���";
                else
                    cardView.FIO.text = FIO;
            }
        }
    }
}
