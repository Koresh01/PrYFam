using UnityEngine;
using UnityEngine.UI;


namespace PrYFam.Assets.Scripts
{
    public class CardView : MonoBehaviour
    {
        [Header("Зависимости:")]
        [SerializeField] private FamilyService familyService;
        [SerializeField] private TreeTraversal treeTraversal;
        [Header("Кнопки:")]
        [SerializeField] Button enter;
        [SerializeField] Button addParent;
        [SerializeField] Button addChild;
        [SerializeField] Button addHalf;
        
        private void Awake() {
            // Префабам нельзя прокидывать сущьности со сцены.
            familyService = GameObject.FindAnyObjectByType<FamilyService>();
            treeTraversal = GameObject.FindAnyObjectByType<TreeTraversal>();
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
        }

        private void OnDisable() {
            enter.onClick.RemoveAllListeners();
            addChild.onClick.RemoveAllListeners();
            addParent.onClick.RemoveAllListeners();
            addHalf.onClick.RemoveAllListeners();
        }
    }
}
