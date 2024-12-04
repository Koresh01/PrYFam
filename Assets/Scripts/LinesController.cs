using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam.Assets.Scripts
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
        public void DrawLine(GameObject from, GameObject to)
        {
            GameObject line = Instantiate(Resources.Load<GameObject>("Prefabs/LineHolder"), from.transform.position, Quaternion.identity, from.transform);
            LineRenderer lr = line.GetComponent<LineRenderer>();

            //from.GetComponent<RectTransform>().anchoredPosition = new Vector2(2,2);

            RectTransform fromRectTransform = from.GetComponent<RectTransform>();
            RectTransform toRectTransform = to.GetComponent<RectTransform>();

            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material.color = Color.white;


            lr.SetPosition(0, fromRectTransform.position);
            lr.SetPosition(1, toRectTransform.position);


            lines.Add(line);
        }
        public void DrawLine(GameObject from1, GameObject from2, GameObject to)
        {
            Vector3 midle = (from1.GetComponent<RectTransform>().position + from2.GetComponent<RectTransform>().position) / 2;

            GameObject line = Instantiate(Resources.Load<GameObject>("Prefabs/LineHolder"), from1.transform.position, Quaternion.identity, placeHolder);
            LineRenderer lr = line.GetComponent<LineRenderer>();

            RectTransform fromRectTransform = from1.GetComponent<RectTransform>();
            RectTransform toRectTransform = to.GetComponent<RectTransform>();

            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material.color = Color.white;

            lr.positionCount = 4;

            lr.SetPosition(0, midle);
            lr.SetPosition(1, new Vector3(midle.x, toRectTransform.position.y + offset, toRectTransform.position.z));
            lr.SetPosition(2, toRectTransform.position + new Vector3(0, offset, 0));
            lr.SetPosition(3, toRectTransform.position);


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
