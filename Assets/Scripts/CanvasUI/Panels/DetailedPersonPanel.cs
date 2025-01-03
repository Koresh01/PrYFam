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
        private Sprite initialSprite;

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

                // Сохраняем текущую фотографию как исходную
                initialSprite = root.ProfilePicture;
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
                root.FirstName = string.IsNullOrEmpty(firstNameInputField.text) ? "И" : firstNameInputField.text;
                root.LastName = string.IsNullOrEmpty(lastNameInputField.text) ? "Ф" : lastNameInputField.text;
                root.MiddleName = string.IsNullOrEmpty(middleNameInputField.text) ? "О" : middleNameInputField.text;


                root.Biography = biographyInputField.text;
                root.DateOfBirth = dateOfBirthInputField.text;
                root.PlaceOfBirth = placeOfBirthInputField.text;

                root.ProfilePicture = image.sprite;


                // Проверяем, изменилась ли фотография
                if (image.sprite != initialSprite)
                {
                    Debug.Log("Фотография была изменена.");
                    root.ProfilePicture = image.sprite;

                    // Обновляем изображение на карточке
                    Transform faceSpriteTransform = root.transform.Find("Environment/Image (Face Sprite)");
                    if (faceSpriteTransform != null)
                    {
                        Image faceSpriteImage = faceSpriteTransform.GetComponent<Image>();
                        faceSpriteImage.sprite = root.ProfilePicture;
                    }
                    else
                    {
                        Debug.LogError("Не удалось установить изображение. Неправильно получаем путь к аватарке на CardPrefab.");
                    }
                }

                // А также меняем ФИО на лицевой стороне карточке
                CardView cardView = root.GetComponent<CardView>();
                cardView.FIO.text = root.LastName + " " + root.FirstName + " " + root.MiddleName;

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
