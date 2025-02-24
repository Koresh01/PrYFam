using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// Скрипт висит на панели поиска членов семейного древа.
    /// Отображает в scrollView всех членов, у которых встречается слово "word";
    /// </summary>
    class PersonFinder : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Скрипт, который обрабатывает свайпы/touches.")]
        MobileInput mobileInput;

        [SerializeField]
        [Tooltip("Скрипт, который обрабатывает компьютерный ввод.")]
        ComputerInput computerInput;

        [Tooltip("Класс хранящий все карточки.")]
        [SerializeField] private FamilyData familyData;

        [Tooltip("Ключевое слово, по которому ищем.")]
        [SerializeField] private TMP_InputField word;

        [Tooltip("Контейнер элементов вертикального списка.")]
        [SerializeField] private Transform container;

        [Tooltip("Префаб -> FoundedPerson(найденный член семьи).")]
        [SerializeField] private GameObject prefab;

        void OnEnable()
        {
            // выключаем обработку свайпов и тачей
            mobileInput.enabled = false;
            computerInput.enabled = false;

            FindByWord();    
        }

        void OnDisable()
        {
            // включаем обработку свайпов и тачей
            mobileInput.enabled = true;
            computerInput.enabled = true;
        }

        /// <summary>
        /// Находит и выводит нужные карточки.
        /// </summary>
        public void FindByWord()
        {
            Clear(); // Очистка предыдущих результатов
            List<Member> members = GetCardsInfos();

            foreach (Member m in members)
            {
                string context = MemberToString(m);
                if (WordExistsInContext(context))
                {
                    // Создаем новый объект карточки и добавляем в контейнер
                    GameObject newCard = Instantiate(prefab, container);
                    
                    // Можно добавить настройку newCard, например, установить текст
                    Elements elements = newCard.GetComponent<Elements>();
                    
                    elements.name.text = m.FirstName;
                    elements.lastName.text = m.LastName;
                    elements.middleName.text = m.MiddleName;
                    elements.ProfilePicture.sprite = m.ProfilePicture;

                    elements.member = m;
                }
            }
        }

        /// <summary>
        /// Получает все карточки дочерних элементов.
        /// </summary>
        private List<Member> GetCardsInfos()
        {
            return familyData.GetAllMembers();
        }

        /// <summary>
        /// Проверяет содержится ли слово в данном контексте.
        /// </summary>
        /// <param name="context">Весь текст, который встречается у конкретного человека( из скрипта Member)</param>
        private bool WordExistsInContext(string context)
        {
            if (string.IsNullOrEmpty(word.text))    // если пользователь ничего не ввел, то выводим все карточки.
                return true;
            else if (context.ToLower().Contains(word.text.ToLower()))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Переводит всю meta-информацию о человеке в строку.
        /// </summary>
        private string MemberToString(Member m)
        {
            return string.Join(" ", new string[] { m.FirstName, m.LastName, m.MiddleName, m.DateOfBirth, m.PlaceOfBirth, m.Biography });
        }

        /// <summary>
        /// Удаляет карточки из контейнера - vertical scrollView;
        /// </summary>
        public void Clear()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
