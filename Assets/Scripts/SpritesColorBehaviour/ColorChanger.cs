using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Подключаем пространство имен для работы с UI

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Скрипт для плавного изменения цвета UI-компонента Image с использованием градиента.
    /// Цвет изменяется по заданному градиенту с регулируемой скоростью.
    /// </summary>
    public class ColorChanger : MonoBehaviour
    {
        [Header("Компонент Image, который должен пульсировать цветом.")]
        [SerializeField]
        Image _image; // Ссылка на компонент Image

        [Header("Градиент")]
        [SerializeField]
        Gradient _gradient; // Градиент для изменения цвета

        [Header("Скорость изменения цвета")]
        [SerializeField]
        float speed = 1.0f; // Скорость пульсации (1.0f = стандартная скорость)

        void Update()
        {
            if (_image != null)
            {
                // Умножаем Time.time на speed, чтобы регулировать скорость изменения цвета
                _image.color = _gradient.Evaluate(Mathf.PingPong(Time.time * speed, 1f));
            }
        }
    }
}