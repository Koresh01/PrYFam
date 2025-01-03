using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PrYFam
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
            // Возвращаем список всех уникальных игровых объектов, связанных с записями в коллекции relationships
            return relationships // Начинаем с коллекции relationships, которая содержит записи о связях между объектами
                .SelectMany(entry => new[]
                {
                    // Для каждой записи в relationships создаем массив из двух объектов:
                    entry.From.gameObject, // Добавляем игровой объект, связанный с From
                    entry.To.gameObject    // Добавляем игровой объект, связанный с To
                })
                .Distinct() // Удаляем дубликаты из объединенной коллекции объектов
                .ToList();  // Преобразуем результирующую коллекцию в список и возвращаем
        }
    }
}