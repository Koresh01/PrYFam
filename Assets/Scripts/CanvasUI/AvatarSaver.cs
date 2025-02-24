using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

// using NativeFilePickerNamespace;
using SFB;


namespace PrYFam
{
    /// <summary>
    /// Скрипт для сохранения текущего изображения-аватара в галерею(Android)/проводник(PC).
    /// </summary>
    public class AvatarSaver : MonoBehaviour
    {
        [Tooltip("Кнопка, для скачивания аватара в галерею.")]
        [SerializeField] private Button saveButton;

        [Tooltip("Кнопка, с которой берем изображение.")]
        [SerializeField] private Button imgButton;

        private void OnEnable()
        {
            saveButton.onClick.AddListener(SaveImage);
        }

        private void OnDisable()
        {
            saveButton.onClick.RemoveListener(SaveImage);
        }

        private void SaveImage()
        {
            if (imgButton == null || imgButton.image == null || imgButton.image.sprite == null)
            {
                Debug.LogError("Кнопка или изображение не назначены!");
                return;
            }

            Texture2D texture = GetTextureFromSprite(imgButton.image.sprite);
            byte[] pngData = texture.EncodeToPNG();
            if (pngData == null)
            {
                Debug.LogError("Ошибка конвертации изображения!");
                return;
            }

#if UNITY_ANDROID
            SaveImageToGallery(pngData);
#elif UNITY_STANDALONE
            SaveImageToPC(pngData);
#else
            Debug.LogError("Функция сохранения не реализована для этой платформы.");
#endif
        }

        private Texture2D GetTextureFromSprite(Sprite sprite)
        {
            Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);

            RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height);
            Graphics.Blit(sprite.texture, rt);

            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return texture;
        }
        private void SaveImageToGallery(byte[] imageData)
        {
            string fileName = "avatar_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            string path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(path, imageData);

            NativeFilePicker.ExportFile(path, (success) =>
            {
                if (success)
                    Debug.Log("Изображение сохранено в галерее: " + path);
                else
                    Debug.LogError("Не удалось сохранить изображение!");
            });
        }
        private void SaveImageToPC(byte[] imageData)
        {
            string fileName = "avatar_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            var paths = StandaloneFileBrowser.SaveFilePanel("Сохранить изображение", "", fileName, "png");

            if (!string.IsNullOrEmpty(paths))
            {
                File.WriteAllBytes(paths, imageData);
                Debug.Log("Изображение сохранено на ПК: " + paths);
            }
            else
            {
                Debug.Log("Сохранение отменено.");
            }
        }
    }
}