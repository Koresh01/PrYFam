using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    public class LinesController : MonoBehaviour
    {
        [SerializeField] Transform placeHolder;
        [Tooltip("Цвет линий")]
        [SerializeField] private Color lineColor = Color.white; // Цвет линии, настраивается в инспекторе
        [Tooltip("Толщина линий (от 0.1 до 1.0)")]
        [SerializeField, Range(0.1f, 1.0f)] private float lineThickness = 0.1f; // Толщина линии
        public List<GameObject> lines;
        public float offset = 2.4f;

        public LinesController()
        {
            lines = new List<GameObject>();
        }

        /// <summary>
        /// Создает и настраивает LineRenderer для линии.
        /// </summary>
        private LineRenderer CreateLine(GameObject from)
        {
            // Создаем объект линии из префаба
            GameObject line = Instantiate(Resources.Load<GameObject>("Prefabs/LineHolder"), from.transform.position, Quaternion.identity, placeHolder);

            // Получаем компонент LineRenderer
            LineRenderer lr = line.GetComponent<LineRenderer>();

            // Настраиваем ширину линии
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;

            // Устанавливаем цвет
            lr.startColor = lineColor;
            lr.endColor = lineColor;

            // Настраиваем порядок отрисовки, чтобы линии были позади
            lr.sortingOrder = -1;

            // Добавляем линию в список для дальнейшего управления
            lines.Add(line);

            return lr;
        }

        /// <summary>
        /// Рисует ломаную линию между двумя объектами.
        /// </summary>
        /// <param name="from">Объект, от которого начинается линия.</param>
        /// <param name="to">Объект, к которому направлена линия.</param>
        public void DrawPolyLine(GameObject from, GameObject to)
        {
            LineRenderer lr = CreateLine(from);

            // Получаем RectTransform для работы с позициями UI-объектов
            RectTransform fromRectTransform = from.GetComponent<RectTransform>();
            RectTransform toRectTransform = to.GetComponent<RectTransform>();

            // Линия будет состоять из 4 точек
            lr.positionCount = 4;

            // Устанавливаем точки линии
            lr.SetPosition(0, new Vector3(fromRectTransform.position.x, fromRectTransform.position.y, 0));
            lr.SetPosition(1, new Vector3(fromRectTransform.position.x, toRectTransform.position.y + offset, 0));
            lr.SetPosition(2, new Vector3(toRectTransform.position.x, toRectTransform.position.y + offset, 0));
            lr.SetPosition(3, new Vector3(toRectTransform.position.x, toRectTransform.position.y, 0));
        }

        /// <summary>
        /// Рисует линию, соединяющую два исходных объекта с одним целевым объектом.
        /// </summary>
        /// <param name="from1">Первый исходный объект.</param>
        /// <param name="from2">Второй исходный объект.</param>
        /// <param name="to">Целевой объект.</param>
        public void DrawMergedLine(GameObject from1, GameObject from2, GameObject to)
        {
            LineRenderer lr = CreateLine(from1);

            // Получаем RectTransform для работы с позициями UI-объектов
            RectTransform fromRectTransform = from1.GetComponent<RectTransform>();
            RectTransform toRectTransform = to.GetComponent<RectTransform>();

            // Вычисляем среднюю точку между двумя исходными объектами
            //Vector3 middle = (from1.GetComponent<RectTransform>().position + from2.GetComponent<RectTransform>().position) / 2;
            Vector3 middle = new Vector3(to.GetComponent<RectTransform>().position.x, from1.GetComponent<RectTransform>().position.y, 0);
            // Линия будет состоять из 6 точек
            lr.positionCount = 6;

            // Устанавливаем точки линии
            lr.SetPosition(0, new Vector3(fromRectTransform.position.x, fromRectTransform.position.y, 0));
            lr.SetPosition(1, new Vector3(fromRectTransform.position.x, fromRectTransform.position.y - offset, 0));
            lr.SetPosition(2, new Vector3(middle.x, middle.y - offset, 0));
            lr.SetPosition(3, new Vector3(middle.x, toRectTransform.position.y + offset, 0));
            lr.SetPosition(4, new Vector3(toRectTransform.position.x, toRectTransform.position.y + offset, 0));
            lr.SetPosition(5, new Vector3(toRectTransform.position.x, toRectTransform.position.y, 0));
        }

        /// <summary>
        /// Рисует линию к half (ломаная дуга через низ карточки).
        /// </summary>
        /// <param name="from">Объект, от которого начинается линия.</param>
        /// <param name="to">Объект, к которому направлена линия.</param>
        public void DrawLineToHalf(GameObject from, GameObject to)
        {
            LineRenderer lr = CreateLine(from);

            // Получаем RectTransform для работы с позициями UI-объектов
            RectTransform fromRectTransform = from.GetComponent<RectTransform>();
            RectTransform toRectTransform = to.GetComponent<RectTransform>();

            // Линия будет состоять из 4 точек
            lr.positionCount = 4;

            // Устанавливаем точки линии
            lr.SetPosition(0, new Vector3(fromRectTransform.position.x, fromRectTransform.position.y, 0));
            lr.SetPosition(1, new Vector3(fromRectTransform.position.x, toRectTransform.position.y - offset, 0));
            lr.SetPosition(2, new Vector3(toRectTransform.position.x, toRectTransform.position.y - offset, 0));
            lr.SetPosition(3, new Vector3(toRectTransform.position.x, toRectTransform.position.y, 0));
        }

        /// <summary>
        /// Удаляет все линии.
        /// </summary>
        public void delAllLines()
        {
            foreach (GameObject line in lines)
            {
                Destroy(line);
            }
            lines.Clear();
        }
    }
}
