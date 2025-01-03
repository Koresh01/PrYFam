using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// Скрипт для преобразования Sprite в string и наоборот.
    /// </summary>
    public static class SpriteConverter
    {
        /// <summary>
        /// Преобразует спрайт в строку, закодированную в Base64.
        /// </summary>
        /// <param name="sprite">Спрайт, который нужно преобразовать.</param>
        /// <param name="imageFormat">Формат изображения (по умолчанию PNG).</param>
        /// <returns>Строка, содержащая изображение в формате Base64.</returns>
        public static string SpriteToString(Sprite sprite, string imageFormat = "png")
        {
            // Создаем текстуру без сжатия из спрайта
            Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            texture.SetPixels(sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height));
            texture.Apply();

            // Конвертируем текстуру в массив байтов в зависимости от формата
            byte[] textureBytes;
            if (imageFormat.ToLower() == "jpg" || imageFormat.ToLower() == "jpeg")
            {
                textureBytes = texture.EncodeToJPG();
            }
            else
            {
                textureBytes = texture.EncodeToPNG();
            }

            // Кодируем массив байтов в строку Base64
            return Convert.ToBase64String(textureBytes);
        }

        /// <summary>
        /// Преобразует строку, закодированную в Base64, обратно в спрайт.
        /// </summary>
        /// <param name="base64String">Строка в формате Base64, представляющая изображение.</param>
        /// <param name="pixelsPerUnit">Количество пикселей на единицу для создания спрайта (по умолчанию 100).</param>
        /// <returns>Восстановленный спрайт.</returns>
        public static Sprite StringToSprite(string base64String, float pixelsPerUnit = 100.0f)
        {
            // Декодируем строку Base64 в массив байтов
            byte[] textureBytes = Convert.FromBase64String(base64String);

            // Создаем текстуру из байтов
            Texture2D texture = new Texture2D(2, 2); // Создаем временную текстуру
            texture.LoadImage(textureBytes); // Загружаем изображение в текстуру

            // Создаем и возвращаем спрайт из текстуры
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }
    }
}
