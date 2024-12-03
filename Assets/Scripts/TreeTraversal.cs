using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// ������������� ���� � ���������� ����������. ��� ������� ������ ���������� �� ���������.
    /// </summary>
    public class TreeTraversal : MonoBehaviour
    {
        [Header("�����������:")]
        [SerializeField] private FamilyService familyService;

        [Header("�������:")]
        [SerializeField] float VerticalSpacing;
        [SerializeField] float HorizontalSpacing;
        [SerializeField, Range(1f, 2f)] float GlobalTreeCorrectionKoefficient;

        [Header("����� �� ������� �������� ������ � ���� �������:")]
        [SerializeField] float duration;

        [Header("������ ���������� ������ ��������:")]
        [SerializeField] Dictionary<Member, Vector2> coordinates;

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
                HorizontalSpacing,
                VerticalSpacing,
                GlobalTreeCorrectionKoefficient
            );

            // 3.
            hardRepositionCards();  // - ������
            //StartCoroutine(SmoothRepositionCards()); // - �����
            
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
                
                cardView.BoundImage.SetActive(false);

                if (memeber == root)
                    cardView.BoundImage.SetActive(true);
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

        /// <summary ������ ������ ������� ���� ��������. </summary>
        private IEnumerator SmoothRepositionCards()
        {
            // ��� ������ �������� ������ ������ �������
            foreach (var member in coordinates.Keys)
            {
                var memberGO = member.gameObject;

                // ����������� ��������(��� ��� ������� ����� ������������ DFS � �������� ��� ���������� ���������)
                memberGO.SetActive(true);

                if (memberGO.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    Vector2 targetPosition = coordinates[member];
                    yield return StartCoroutine(SmoothMove(rectTransform, targetPosition, duration)); // ������� ��������, 0.5 ������ ��� ��������
                }
            }
        }
        private IEnumerator SmoothMove(RectTransform rectTransform, Vector2 targetPosition, float duration)
        {
            Vector2 startPosition = rectTransform.anchoredPosition;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // �������� ������������ ����� ��������� � ������� ��������
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // ��������, ��� �������� ������� ����������� �����
            rectTransform.anchoredPosition = targetPosition;
        }
    }
}
