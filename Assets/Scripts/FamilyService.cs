using System;
using System.Linq;  // для доступа к .Where в List<>
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Сервис для работы с данными семейного древа.
    /// Предоставляет функциональность для создания, получения и установки семейных данных.
    /// </summary>
    public class FamilyService : MonoBehaviour
    {
        [SerializeField] GameObject personCardPrefab;
        [SerializeField] Transform cardsPlaceHolder;
        [Header("Зависимости:")]
        public FamilyData familyData;

        /// <summary>
        /// Возвращает список членов семейного дерева, связанных с указанным членом семьи через заданный тип связи.
        /// </summary>
        /// <param name="from">Член семьи, для которого ищутся связанные участники.</param>
        /// <param name="relationship">Тип связи, например, родитель или ребенок.</param>
        /// <returns>Список связанных членов семьи.</returns>
        public List<Member> GetRelatedMembers(Member from, Relationship relationship)
        {
            return familyData.relationships
                .Where(e => e.From == from && e.Relationship == relationship)
                .Select(e => e.To)
                .ToList();
        }
        #region creating new person
        /// <summary>
        /// Добавляет нового человека в семью и добавляет SMART-связь к нему.
        /// </summary>
        public GameObject CreateMemberWithConnection(GameObject parent, Relationship relationship)
        {
            GameObject newMemberObj = createGameObject();
            AddConnection(parent, newMemberObj, relationship);
            return newMemberObj;
        }
        /// <summary>
        /// Создаёт карточку префаба человека.
        /// </summary>
        private GameObject createGameObject()
        {
            var go = Instantiate(personCardPrefab, cardsPlaceHolder);
            return go;
        }
        /// <summary>
        /// Добавляет связь между игровыми объектами prefab-ами: [PersonCard] в семейное древо.
        /// </summary>
        private void AddConnection(GameObject From, GameObject To, Relationship relationship = Relationship.None)
        {
            Member from = From.GetComponent<Member>();
            Member to = To.GetComponent<Member>();

            // тут где то нужна проверка
            // типо если relationship == ToHalf => пользователь жедает добавить жену.
            // А значит дети Member from - это ещё и дети Member to;
            // to.children = from.children

            AddBidirectionalRelationship(from, to, relationship);
        }
        #endregion
        #region relationships

        /// <summary> Добавляет двунаправленную связь между членами. </summary>
        private void AddBidirectionalRelationship(Member from, Member to, Relationship relationship)
        {
            Debug.LogFormat("Пытаемся от {0} добавить связь к {1}", from.name, to.name);

            // Проверяем, есть ли уже такая связь
            if (!RelationshipExists(from, to))
            {
                if (to != null)
                {
                    // Добавляем прямую связь:
                    familyData.relationships.Add(new MembersConnection
                    {
                        From = from,
                        To = to,
                        Relationship = relationship
                    });
                    // Добавляем обратную связь:
                    familyData.relationships.Add(new MembersConnection
                    {
                        From = to,
                        To = from,
                        Relationship = GetReverseRelationship(relationship)
                    });
                }
            }
        }
        /// <summary> Проверяет, существует ли хотя бы один элемент в списке, удовлетворяющий этому предикату.. </summary>
        /// <returns>bool</returns>
        public bool RelationshipExists(Member from, Member to)
        {
            return familyData.relationships.Exists(entry => entry.From == from && entry.To == to);
        }
        public Relationship? GetRelationship(Member from, Member to)
        {
            // Используем метод Find, чтобы найти первый элемент в списке `relationships`,
            // который соответствует условию: поле From равно from, а поле To равно to.
            // Если элемент найден, он будет сохранён в переменной entry.
            // Если элемент не найден, entry будет равен null.
            var entry = familyData.relationships.Find(e => e.From == from && e.To == to);

            // Используем оператор null-условной обработки (?.) для безопасного доступа.
            // Если entry не равен null, возвращаем поле Relationship из найденного элемента.
            // Если entry равен null, возвращаем null.
            return entry?.Relationship;
        }
        public void RemoveRelationship(Member from, Member to)
        {
            // Удаляем прямую связь
            familyData.relationships.RemoveAll(entry => entry.From == from && entry.To == to);

            // Удаляем обратную связь
            familyData.relationships.RemoveAll(entry => entry.From == to && entry.To == from);
        }
        /// <summary> Получает обратную связь между членами </summary>
        private Relationship GetReverseRelationship(Relationship relationship)
        {
            return relationship switch
            {
                Relationship.ToParent => Relationship.ToChild,
                Relationship.ToHalf => Relationship.ToHalf,
                Relationship.ToChild => Relationship.ToParent,
                _ => throw new ArgumentOutOfRangeException(nameof(relationship), "Неизвестное отношение"),
            };
        }
        #endregion
        #region simplified interaction
        public void DebugRelationships()    // функция отладки
        {
            foreach (var entry in familyData.relationships)
            {
                Debug.Log($"{entry.From?.name} -> {entry.To?.name}: {entry.Relationship}");
            }
        }
        public bool hasNoChildren(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToChild).Count == 0 ? true : false;
        }
        public bool hasNoParents(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToParent).Count == 0 ? true : false;
        }
        public bool hasAllParents(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToParent).Count == 2 ? true : false;
        }
        public bool hasHalf(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToHalf).Count > 0 ? true : false;
        }
        #endregion
    }
}
