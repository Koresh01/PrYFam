﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using SFB;

namespace PrYFam
{
    /// <summary>
    /// Скрипт висит на кнопке и при нажатии на неё вызывает файловый проводник(WIN + Android) 
    /// и предлагает выбрать изображение, которое устанавливается этой кнопке.
    /// </summary>
    public class ImagePicker : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Кнопка, на которую будет устанавливаться изображение.")]
        [SerializeField] private Button targetButton;

        [Tooltip("UI элемент для отображения ошибки.")]
        [SerializeField] private TMP_Text errorMessage;

        /// <summary>
        /// Запускает выбор изображения в зависимости от платформы.
        /// </summary>
        public void PickImage()
        {
#if UNITY_STANDALONE_WIN
            PickImageWindows();  // Для Windows используем StandaloneFileBrowser
#elif UNITY_ANDROID
            PickImageAndroid();  // Для Android используем NativeGallery
#else
            ShowError("Платформа не поддерживается.");
#endif
        }

        /// <summary>
        /// Обрабатывает выбор изображения на Windows.
        /// </summary>
        private void PickImageWindows()
        {
            var extensions = new [] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
            };
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Выберите изображение", "", extensions, false);

            if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                string path = paths[0];
                StartCoroutine(LoadAndSetImage(path));
            }
            else
            {
                ShowError("Файл не выбран.");
            }
        }

        /// <summary>
        /// Обрабатывает выбор изображения на Android.
        /// </summary>
        private void PickImageAndroid()
        {
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                if (!string.IsNullOrEmpty(path))
                {
                    StartCoroutine(LoadAndSetImage(path));
                }
            }, "Выберите изображение", "image/*");

            if (permission != NativeGallery.Permission.Granted)
            {
                ShowError("Доступ к галерее запрещён!");
            }
        }

        /// <summary>
        /// Загружает изображение из указанного пути и устанавливает его на кнопку.
        /// </summary>
        private IEnumerator LoadAndSetImage(string path)
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                targetButton.image.sprite = sprite;
                targetButton.image.color = Color.white;
            }
            else
            {
                ShowError("Не удалось загрузить изображение.");
            }

            yield return null;
        }

        /// <summary>
        /// Отображает сообщение об ошибке.
        /// </summary>
        private void ShowError(string message)
        {
            if (errorMessage != null)
            {
                errorMessage.text = message;
                errorMessage.gameObject.SetActive(true);
            }
            Debug.LogError(message);
        }
    }
}