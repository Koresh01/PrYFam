using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PrYFam
{
    /// <summary>
    /// Класс позволяет получить доступ к основным элементам префаба карточки члена семьи,
    /// которая лежит в vertical Scroll view.
    /// </summary>
    class Elements : MonoBehaviour
    {
        [Header("Кнопка перехода к конкретному человеку:")]
        [SerializeField] Button goTo;

        [Header("ФИО + аватарка:")]
        [Tooltip("Контейнер для фамилии.")]
        public TextMeshProUGUI lastName;

        [Tooltip("Контейнер для имени.")]
        public TextMeshProUGUI name;

        [Tooltip("Контейнер для отчества.")]
        public TextMeshProUGUI middleName;

        [Tooltip("Контейнер для аватарки.")]
        public Image ProfilePicture;

        [Tooltip("Ссылка непосредственно на саму карточку:")]
        public Member member;

        [Tooltip("Ссылка на панель расширенного поиска.")]
        public PersonFinder personFinder;

        private void Awake()
        {
            // Префабам нельзя прокидывать сущьности со сцены.
            personFinder = GameObject.FindAnyObjectByType<PersonFinder>();
        }

        void OnEnable()
        {
            goTo.onClick.AddListener(() => {
                CardView cardView = member.GetComponent<CardView>();
                cardView.HandleTreeRedrawWithCameraMove();

                personFinder.gameObject.SetActive(false);
            });    
        }

        void OnDisable()
        {
            goTo.onClick.RemoveAllListeners();
        }
    }
}
