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
        [Tooltip("Контейнер для фамилии.")]
        public TextMeshProUGUI lastName;

        [Tooltip("Контейнер для имени.")]
        public TextMeshProUGUI name;

        [Tooltip("Контейнер для отчества.")]
        public TextMeshProUGUI middleName;

        [Tooltip("Контейнер для аватарки.")]
        public Image ProfilePicture;

        [Tooltip("Ссылка непосредственно на сам объект:")]
        public Member member;
    }
}
