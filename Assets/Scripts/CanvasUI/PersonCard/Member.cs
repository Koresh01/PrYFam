using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// Представляет члена семейного древа.
    /// Каждый объект этого класса содержит ссылки на своих родителей, детей и возможных сводных родственников.
    /// </summary>
    public class Member : MonoBehaviour
    {
        [Header("Основная информация")]
        [Tooltip("Имя человека")]
        public string FirstName;

        [Tooltip("Фамилия человека")]
        public string LastName;

        [Tooltip("Отчество человека")]
        public string MiddleName;

        [Space]

        [Tooltip("Фотография человека")]
        public Sprite ProfilePicture;

        [Header("Дата и место рождения")]
        [Tooltip("Дата рождения")]
        public DateTime DateOfBirth;

        [Tooltip("Место рождения")]
        public string PlaceOfBirth;

        [Space]
        
        [Header("Дополнительная информация")]
        [Tooltip("Дата смерти (оставьте пустым, если жив)")]
        public DateTime? DateOfDeath;

        [Tooltip("Место смерти (если применимо)")]
        public string PlaceOfDeath;

        [Tooltip("Краткая биография или описание")]
        [TextArea]
        public string Biography;
    }
}
