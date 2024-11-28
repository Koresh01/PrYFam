using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Представляет связь между двумя членами семьи.
    /// Включает исходный объект, целевой объект и тип отношения между ними.
    /// </summary>
    [System.Serializable]
    public class MembersConnection
    {
        public Member From;
        public Member To;
        public Relationship Relationship;
    }


    /// <summary>
    /// Содержит данные о семейном древе, включая список всех связей между членами семьи.
    /// Предоставляет методы для добавления, удаления и проверки связей.
    /// </summary>
    [System.Serializable]
    public class FamilyData : MonoBehaviour
    {
        // Список для хранения отношений
        public List<MembersConnection> relationships = new List<MembersConnection>();

        /// <summary>
        /// Возвращает список всех игровых объектов, связанных с членами семьи.
        /// </summary>
        /// <returns>Список GameObject, представляющих всех членов семьи.</returns>
        public List<GameObject> GetAllPersonCards()
        {
            return relationships
                .SelectMany(entry => new[] { entry.From.gameObject, entry.To.gameObject })
                .Distinct()
                .ToList();
        }
    }
}