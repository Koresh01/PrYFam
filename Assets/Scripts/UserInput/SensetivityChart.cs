using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrYFam.Assets.Scripts.UserInput
{
    /// <summary>
    /// Скрпит, который хранит зависимость чувствительности от расстояния до карточек.
    /// </summary>
    class SensetivityChart : MonoBehaviour
    {
        [Tooltip("Общие настройки ввода")]
        [SerializeField]
        private CommonInputSettings commonInputSettings;

        [Header("График множителя")]
        [Tooltip("График зависимости множителя x от позиции камеры z")]
        public AnimationCurve zoomMultiplierCurve;

        private void Awake()
        {
            InitGraphic();
        }

        /// <summary>
        /// Создаёт график зависимости множителя свайпа от расстояния камеры ДО карточек.
        /// </summary>
        public void InitGraphic()
        {
            float leftZoomX = commonInputSettings.maxZoom;
            float rightZoomX = commonInputSettings.minZoom;

            // Создаём линейную зависимость для множителя multiplier.

            // Коэффициент 3f определяет насколько сильнее будет умножаться разница между пальцами.
            float maxValue = rightZoomX * 3f;
            float minValue = leftZoomX * 3f;
            zoomMultiplierCurve = CreateLinearZoomCurve(leftZoomX, rightZoomX, minValue, maxValue);
        }

        /// <summary>
        /// Создаёт линейную зависимость для AnimationCurve между двумя точками.
        /// </summary>
        /// <param name="minZoom">Минимальное значение z (например, самое далёкое положение камеры).</param>
        /// <param name="maxZoom">Максимальное значение z (например, самое близкое положение камеры).</param>
        /// <param name="maxValue">Значение функции при максимальном z.</param>
        /// <param name="minValue">Значение функции при минимальном z.</param>
        /// <returns>Линейная AnimationCurve между заданными точками.</returns>
        public AnimationCurve CreateLinearZoomCurve(float leftZoomX, float rightZoomX, float minValue, float maxValue)
        {
            // Создаём новую AnimationCurve
            AnimationCurve curve = new AnimationCurve();

            // Добавляем ключи
            Keyframe firstKey = new Keyframe(leftZoomX, minValue);
            Keyframe secondKey = new Keyframe(rightZoomX, maxValue);

            // Вычисляем тангенсы для линейной зависимости
            float tangent = (maxValue - minValue) / (rightZoomX - leftZoomX);


            firstKey.inTangent = 0;           // Входной тангенс начальной точки
            firstKey.outTangent = tangent;          // Выходной тангенс начальной точки
            secondKey.inTangent = tangent;          // Входной тангенс конечной точки
            secondKey.outTangent = 0;         // Выходной тангенс конечной точки

            // Добавляем ключи в график
            curve.AddKey(firstKey);
            curve.AddKey(secondKey);

            return curve;
        }

        /// <summary>
        /// Получает множитель x(z) на основе Z позиции камеры.
        /// </summary>
        /// <param name="z">Текущее значение z камеры</param>
        /// <returns>Значение множителя</returns>
        public float GetZoomMultiplier(float z)
        {
            return zoomMultiplierCurve.Evaluate(-z);    // т.к. позиция камеры по оси Z у нас отрицательная.
        }
    }
}
