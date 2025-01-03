using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrYFam
{
    /// <summary>
    /// Скрипт для панели детальной информации о члене семьи.
    /// Реализует логику работы кнопок и заполнение текстовых полей данными.
    /// Вызывает методы из CanvasView для управления отображением панели.
    /// </summary>
    public class DetailedPersonPanel : MonoBehaviour
    {
        [Header("Ссылки на внешние компоненты")]
        [Tooltip("Основной объект управления CanvasView.")] [SerializeField] private CanvasView canvasView;
        [SerializeField] private TreeTraversal treeTraversal;



        [Header("Кнопки:")]
        [Tooltip("Кнопка для закрытия панели детальной информации.")]
        [SerializeField] private Button closeButton;
        [Tooltip("Кнопка для сохранения информации о конкретном члене семьи.")]
        [SerializeField] private Button saveButton;




        [Header("Текстовые поля ввода:")]

        [Tooltip("Поле ввода имени.")]
        [SerializeField] private TMP_InputField firstNameInputField;

        [Tooltip("Поле ввода фамилии.")]
        [SerializeField] private TMP_InputField lastNameInputField;

        [Tooltip("Поле ввода отчества.")]
        [SerializeField] private TMP_InputField middleNameInputField;

        [Tooltip("Биография.")]
        [SerializeField] private TMP_InputField biographyInputField;

        [Tooltip("Годы жизни.")]
        [SerializeField] private TMP_InputField dateOfBirthInputField;

        [Tooltip("Место рождения.")]
        [SerializeField] private TMP_InputField placeOfBirthInputField;



        [Header("Изображения:")]
        [Tooltip("Фотография члена семьи, на панели детальной информации.")] [SerializeField] Image image;
        /*Defaul-ная аватрка в панели детальной информации не соответствует 
         Defaul-ноая аватрке на самой CardPrefab. Так что прийдётся отслеживать,
        произошло ли изменение фотографии, прежде чем будем сохранять.*/
        // Переменная для хранения исходного спрайта
        [Tooltip("Дефолтный аватар")] public Sprite initialSprite;


        private void OnEnable()
        {
            // Инициализация панели при включении
            InitializePanel();

            // Добавление обработчика на кнопку закрытия
            closeButton.onClick.AddListener(OnCloseButtonClick);

            // Добавление обработчика на кнопку сохранения
            saveButton.onClick.AddListener(OnSaveButtonClick);
        }

        private void OnDisable()
        {
            // Удаление всех обработчиков кнопки закрытия при отключении
            closeButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        /// <summary>
        /// Инициализирует текстовые поля данными текущего пользователя.
        /// </summary>
        private void InitializePanel()
        {
            Member root = Algorithms.Singleton.root;

            if (root != null)
            {
                // Устанавливаем сохранённые значения
                firstNameInputField.text = root.FirstName;
                lastNameInputField.text = root.LastName;
                middleNameInputField.text = root.MiddleName;


                biographyInputField.text = root.Biography;
                dateOfBirthInputField.text = root.DateOfBirth;
                placeOfBirthInputField.text = root.PlaceOfBirth;

                image.sprite = root.ProfilePicture;
            }
            else
            {
                Debug.LogWarning("Данные о члене семьи не найдены!");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки закрытия панели.
        /// </summary>
        private void OnCloseButtonClick()
        {
            if (canvasView != null)
            {
                canvasView.HideDetailedPersonPanel();

                // Переводим статус кругового меню в закрытое состояние
                Member root = Algorithms.Singleton.root;
                RoundMenuController roundMenuController = root.transform.GetComponent<RoundMenuController>();
                roundMenuController.CardStatus = RoundMenuStatus.FirstPressed;
            }
            else
            {
                Debug.LogError("CanvasView не назначен в инспекторе!");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки сохранения данных панели.
        /// </summary>
        private void OnSaveButtonClick()
        {
            Member root = Algorithms.Singleton.root;

            if (root != null)
            {
                root.FirstName = firstNameInputField.text;
                root.LastName = lastNameInputField.text;
                root.MiddleName = middleNameInputField.text;


                root.Biography = biographyInputField.text;
                root.DateOfBirth = dateOfBirthInputField.text;
                root.PlaceOfBirth = placeOfBirthInputField.text;

                root.ProfilePicture = image.sprite;

                // Отрисуем древо, чтоб увидеть как изменились карточки.
                treeTraversal.hardRepositionCards();

                // Закрываем панель
                OnCloseButtonClick();
            }
            else
            {
                Debug.LogWarning("Данные о члене семьи не найдены!");
            }
        }
    }
}
