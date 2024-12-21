using System;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace PrYFam
{
    public class LinesController : MonoBehaviour
    {
        [SerializeField] Transform placeHolder;
        public List<GameObject> lines;
        public float offset = 2.4f;

        public LinesController()
        {
            lines = new List<GameObject>();
        }
        /// <summary>
        /// Рисует прямую линию от одного объекта к другому, например, для отображения связи "родитель-ребенок".
        /// </summary>
        /// <param name="from">Объект, от которого начинается линия.</param>
        /// <param name="to">Объект, к которому направлена линия.</param>
        public void DrawDirectLine(GameObject from, GameObject to)
        {
            // Создаем объект линии из префаба
            GameObject line = Instantiate(Resources.Load<GameObject>("Prefabs/LineHolder"), from.transform.position, Quaternion.identity, placeHolder);

            // Получаем компонент LineRenderer, чтобы настроить линии
            LineRenderer lr = line.GetComponent<LineRenderer>();

            // Получаем RectTransform для работы с позициями UI-объектов
            RectTransform fromRectTransform = from.GetComponent<RectTransform>();
            RectTransform toRectTransform = to.GetComponent<RectTransform>();

            // Устанавливаем ширину линии и цвет
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material.color = Color.white;

            // Линия будет состоять из 4 точек
            lr.positionCount = 4;

            // Устанавливаем точки линии
            lr.SetPosition(0, new Vector3(fromRectTransform.position.x, fromRectTransform.position.y, 0));
            lr.SetPosition(1, new Vector3(fromRectTransform.position.x, toRectTransform.position.y + offset, 0));
            lr.SetPosition(2, new Vector3(toRectTransform.position.x, toRectTransform.position.y + offset, 0));
            lr.SetPosition(3, new Vector3(toRectTransform.position.x, toRectTransform.position.y, 0));

            // Добавляем линию в список для дальнейшего управления
            lines.Add(line);
        }

        /// <summary>
        /// Рисует линию, соединяющую два исходных объекта с одним целевым объектом, например, для отображения связи между двумя родителями и ребенком.
        /// </summary>
        /// <param name="from1">Первый исходный объект (например, первый родитель).</param>
        /// <param name="from2">Второй исходный объект (например, второй родитель).</param>
        /// <param name="to">Целевой объект, к которому направлена линия (например, ребенок).</param>
        public void DrawMergedLine(GameObject from1, GameObject from2, GameObject to)
        {
            // Вычисляем среднюю точку между двумя исходными объектами
            Vector3 middle = (from1.GetComponent<RectTransform>().position + from2.GetComponent<RectTransform>().position) / 2;

            // Создаем объект линии из префаба
            GameObject line = Instantiate(Resources.Load<GameObject>("Prefabs/LineHolder"), from1.transform.position, Quaternion.identity, placeHolder);

            // Получаем компонент LineRenderer, чтобы настроить линии
            LineRenderer lr = line.GetComponent<LineRenderer>();

            // Получаем RectTransform для работы с позициями UI-объектов
            RectTransform fromRectTransform = from1.GetComponent<RectTransform>();
            RectTransform toRectTransform = to.GetComponent<RectTransform>();

            // Устанавливаем ширину линии и цвет
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material.color = Color.white;

            // Линия будет состоять из 6 точек
            lr.positionCount = 6;

            // Устанавливаем точки линии
            lr.SetPosition(0, new Vector3(fromRectTransform.position.x, fromRectTransform.position.y, 0));
            lr.SetPosition(1, new Vector3(fromRectTransform.position.x, fromRectTransform.position.y - offset, 0));
            lr.SetPosition(2, new Vector3(middle.x, middle.y - offset, 0));
            lr.SetPosition(3, new Vector3(middle.x, toRectTransform.position.y + offset, 0));
            lr.SetPosition(4, new Vector3(toRectTransform.position.x, toRectTransform.position.y + offset, 0));
            lr.SetPosition(5, new Vector3(toRectTransform.position.x, toRectTransform.position.y, 0));

            // Добавляем линию в список для дальнейшего управления
            lines.Add(line);
        }

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
