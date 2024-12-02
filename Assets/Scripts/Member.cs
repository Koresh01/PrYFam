using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Представляет члена семейного древа.
    /// Каждый объект этого класса содержит ссылки на своих родителей, детей и возможных сводных родственников.
    /// </summary>
    public class Member : MonoBehaviour
    {
        public string Name;
        public string surname;
        public Sprite sprite;
    }
}
