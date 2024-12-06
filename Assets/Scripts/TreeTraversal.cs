using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.ComponentModel;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// ������������� ���� � ���������� ����������. ��� ������� ������ ���������� �� ���������.
    /// </summary>
    public class TreeTraversal : MonoBehaviour
    {
        [Header("�����������:")]
        [SerializeField] private FamilyService familyService;
        [SerializeField] private LinesController linesController;

        [Header("�������:")]
        public float CardHeight;
        public float CardWidth;
        [Range(0f, 400f)] public float GlobalTreeOffset;    // ���������� ����� 2��� ����������.

        [Header("������ ���������� ������ ��������:")]
        [SerializeField] Dictionary<Member, Vector2> coordinates;


        private void Awake()
        {
            CardHeight = familyService.personCardPrefab.GetComponent<RectTransform>().sizeDelta.x;
            CardWidth = familyService.personCardPrefab.GetComponent<RectTransform>().sizeDelta.y;

            CardWidth += GlobalTreeOffset;
            CardHeight += GlobalTreeOffset;
        }

        public void ReDrawTree(Member root, Vector2 basePosition)
        {
            Debug.Log("����������� ����� �� ������ FamilyData.");
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
                        Member half = familyService.GetRelatedMembers(from, Relationship.ToHalf).FirstOrDefault();
                        linesController.DrawMergedLine(from.gameObject, half.gameObject, to.gameObject);
                    }
                    if (!familyService.hasHalf(from))
                    {
                        linesController.DrawDirectLine(from.gameObject, to.gameObject);
                    }


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

        /// <summary> ��������� ������� � RectTransform ������� ����� </summary>
        private void hardRepositionCards() {
            foreach (var member in coordinates.Keys)
            {
                var memberGO = member.gameObject;
                
                // ����������� ��������(��� ��� ������� ����� ������������ DFS � �������� ��� ���������� ���������)
                memberGO.SetActive(true);

                if (memberGO.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    rectTransform.anchoredPosition = coordinates[member];
                }
            }
        }
    }
}
