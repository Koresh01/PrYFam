using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// Хранит всю meta-информацию о конкретном члене семейного древа.
    /// </summary>
    public class Member : MonoBehaviour
    {
        public string UniqueId;
        [Header("Основная информация")]
        [Tooltip("Имя человека")]
        public string FirstName;

        [Tooltip("Фамилия человека")]
        public string LastName;

        [Tooltip("Отчество человека")]
        public string MiddleName;

        [Space]

        [Tooltip("Сохранённая в системе фотография члена семьи.")]
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
