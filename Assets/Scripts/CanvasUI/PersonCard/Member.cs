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

        [Tooltip("Фотография человека на панели детальной информации.")]
        public Sprite ProfilePicture;

        [Header("Дата и место рождения")]
        [Tooltip("Годы жизни:")]
        public string DateOfBirth;

        [Tooltip("Место рождения")]
        public string PlaceOfBirth;


        [Tooltip("Краткая биография или описание")]
        [TextArea]
        public string Biography;
    }
}
