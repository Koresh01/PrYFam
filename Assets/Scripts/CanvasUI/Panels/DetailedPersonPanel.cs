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
        [Tooltip("Основной объект управления CanvasView.")]
        [SerializeField] private CanvasView canvasView;



        [Header("Кнопки:")]
        [Tooltip("Кнопка для закрытия панели детальной информации.")]
        [SerializeField] private Button closeButton;
        [Tooltip("Кнопка для сохранения информации о конкретном члене семьи.")]
        [SerializeField] private Button saveButton;




        [Header("Текстовые поля")]
        [Tooltip("Поле ввода имени.")]
        [SerializeField] private TMP_InputField firstNameInputField;

        [Tooltip("Поле ввода фамилии.")]
        [SerializeField] private TMP_InputField lastNameInputField;

        [Tooltip("Поле ввода отчества.")]
        [SerializeField] private TMP_InputField middleNameInputField;




        [Header("Изображения:")]
        [Tooltip("Аватар члена семьи.")]
        [SerializeField] Image image;

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
                // Сохраняем новые значения
                root.FirstName = firstNameInputField.text;
                root.LastName = lastNameInputField.text;
                root.MiddleName = middleNameInputField.text;


                root.ProfilePicture = image.sprite;


                // Находим карточку и обновляем спрайт
                Transform faceSpriteTransform = root.transform.Find("Environment").Find("Image (Face Sprite)"); // вот тут надо что то вроде FindChildByName
                Image faceSpriteImage = faceSpriteTransform.GetComponent<Image>();
                faceSpriteImage.sprite = root.ProfilePicture;

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
