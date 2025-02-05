using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Вращает изображение на кнопке по часовой стрелке.
/// </summary>
public class RotateImage : MonoBehaviour
{
    [Tooltip("Кнопка, изображение на которой будет вращаться.")]
    [SerializeField] private Button targetButton;

    /// <summary>
    /// Метод для поворота изображения.
    /// </summary>
    public void RotateClockwise()
    {
        if (targetButton == null || targetButton.image == null || targetButton.image.sprite == null)
        {
            Debug.LogError("Нет изображения для вращения!");
            return;
        }

        Sprite currentSprite = targetButton.image.sprite;
        Texture2D currentTexture = currentSprite.texture;

        // Создаем повернутую текстуру
        Texture2D rotatedTexture = RotateTextureClockwise(currentTexture);

        // Создаем новый спрайт из повернутой текстуры
        Sprite newSprite = Sprite.Create(rotatedTexture, new Rect(0, 0, rotatedTexture.width, rotatedTexture.height), Vector2.one * 0.5f);

        // Устанавливаем новый спрайт на кнопку
        targetButton.image.sprite = newSprite;
    }

    /// <summary>
    /// Вращает текстуру на 90 градусов по часовой стрелке.
    /// </summary>
    private Texture2D RotateTextureClockwise(Texture2D originalTexture)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;
        Texture2D rotatedTexture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                rotatedTexture.SetPixel(y, width - x - 1, originalTexture.GetPixel(x, y));
            }
        }

        rotatedTexture.Apply();
        return rotatedTexture;
    }
}
