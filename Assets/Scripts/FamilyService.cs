using System;
using System.Linq;  // для доступа к .Where в List<>
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PrYFam
{
    /// <summary>
    /// Сервис для работы с данными семейного древа.
    /// Предоставляет функциональность для создания, получения и установки семейных данных.
    /// </summary>
    public class FamilyService : MonoBehaviour
    {
        public GameObject personCardPrefab;
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
        public void CreateMemberWithConnection(GameObject From, Relationship relationship)
        {
            Member from = From.GetComponent<Member>();
            if (!CanAddConnection(from, relationship)) return;
            
            GameObject newMemberObj = createMemberGameObject();
            Member to = newMemberObj.GetComponent<Member>();

            AddConnection(from, to, relationship);
        }
        /// <summary>
        /// Создаёт карточку префаба человека.
        /// </summary>
        public GameObject createMemberGameObject()
        {
            var go = Instantiate(personCardPrefab, cardsPlaceHolder);
            go.SetActive(false);    // Отображаться они начнут после отрисовки.
            return go;
        }
        /// <summary>
        /// Добавляет связь между игровыми объектами prefab-ами: [PersonCard] в семейное древо.
        /// </summary>
        public void AddConnection(Member from, Member to, Relationship relationship = Relationship.None)
        {
            // обработка неликвидных ситуаций
            switch (relationship)
            {
                case Relationship.ToHalf:
                    HandleToHalf(from, to);     // внутри и создание карточки происходит
                    break;
                case Relationship.ToParent:
                    HandleToParent(from, to);   // внутри и создание карточки происходит
                    break;
                case Relationship.ToChild:
                    HandleToChild(from, to);    // внутри и создание карточки происходит
                    break;
                default:
                    Debug.LogWarning("Неизвестное отношение: " + relationship);
                    break;
            }


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
        

        /// <summary>
        /// Удаляет все вхождения переданного Member из FamilyData.
        /// </summary>
        /// <param name="person">персона для удаления</param>
        public void DeletePerson(Member person)
        {
            // Удаляем связь ОТ PERSON
            familyData.relationships.RemoveAll(entry => entry.From == person);
            // Удаляем связь К PERSON
            familyData.relationships.RemoveAll(entry => entry.To == person);
        }
        /// <summary>
        /// Удаляет взаимосвязь из FamilyData.
        /// </summary>
        private void RemoveRelationship(Member from, Member to)
        {
            // Удаляем прямую связь
            familyData.relationships.RemoveAll(entry => entry.From == from && entry.To == to);

            // Удаляем обратную связь
            familyData.relationships.RemoveAll(entry => entry.From == to && entry.To == from);
        }
        /// <summary>
        /// Получает обратную связь между членами
        /// </summary>
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

        /// <summary>
        /// Является ли член семьи крайним в древе.
        /// </summary>
        public bool IsLeaf(Member cur) {
            if (hasParents(cur) && !hasChildren(cur) && !hasHalf(cur))
                return true;
            if (!hasParents(cur) && hasChildren(cur) && hasHalf(cur))
                return true;
            if (!hasParents(cur) && hasChildren(cur) && !hasHalf(cur) && GetRelatedMembers(cur, Relationship.ToChild).Count <= 1)
                return true;

            Debug.Log("Невозможно удалить без последствий целостности древа!!!");
            return false;
        }

        public bool hasChildren(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToChild).Count == 0 ? false : true;
        }
        public bool hasParents(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToParent).Count == 0 ? false : true;
        }
        public bool hasAllParents(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToParent).Count == 2 ? true : false;
        }
        public bool hasHalf(Member current)
        {
            return GetRelatedMembers(current, Relationship.ToHalf).Count > 0 ? true : false;
        }


        /// <summary> Проверяет можно ли добавить взаимосвязь. </summary>
        private bool CanAddConnection(Member from, Relationship relationship)
        {
            switch (relationship) {
                case Relationship.ToHalf:
                    if (!CanAddHalf(from)) return false;
                    break;
                case Relationship.ToParent:
                    if (!CanAddParent(from)) return false;
                    break;
                case Relationship.ToChild:
                    break;
            }
            return true;
        }
        /// <summary> Проверяет можно ли добавить жену. </summary>
        private bool CanAddHalf(Member from)
        {
            if (hasHalf(from))
            {
                Debug.Log("Нельзя добавлять больше одной жены.");
                return false;
            }
            return true;
        }
        /// <summary> Проверяет можно ли добавить ещё родителя. </summary>
        private bool CanAddParent(Member from)
        {
            if (GetRelatedMembers(from, Relationship.ToParent).Count >= 2)
            {
                Debug.Log("Нельзя добавлять больше двух родителей.");
                return false;
            }
            return true;
        }


        /// <summary>
        /// Обрабатывает добавление супруга (жены или мужа) для указанного члена семьи. 
        /// Если у члена семьи уже есть супруг, то добавление не выполняется. 
        /// Также обновляет связи между супругом и детьми.
        /// </summary>
        private void HandleToHalf(Member from, Member to)
        {
            AddBidirectionalRelationship(from, to, Relationship.ToHalf);    // прямую связь 100% добавляем

            // нюансы:
            foreach (var child in GetRelatedMembers(from, Relationship.ToChild))
            {
                AddBidirectionalRelationship(child, to, Relationship.ToParent);
            }
        }
        /// <summary>
        /// Обрабатывает добавление родителя для указанного члена семьи. 
        /// Если у члена семьи уже есть два родителя, то добавление не выполняется. 
        /// Также устанавливает связь между новым родителем и уже существующим вторым родителем как "супруги".
        /// </summary>
        private void HandleToParent(Member from, Member to)
        {
            AddBidirectionalRelationship(from, to, Relationship.ToParent);    // прямую связь 100% добавляем

            // нюансы:
            foreach (var parent in GetRelatedMembers(from, Relationship.ToParent))
            {
                if (parent != to)
                {
                    AddBidirectionalRelationship(parent, to, Relationship.ToHalf);
                }
            }
        }
        /// <summary>
        /// Обрабатывает добавление ребёнка для указанного члена семьи. 
        /// Устанавливает связь между добавляемым ребёнком и супругом члена семьи.
        /// </summary>
        private void HandleToChild(Member from, Member to)
        {
            AddBidirectionalRelationship(from, to, Relationship.ToChild);    // прямую связь 100% добавляем

            // нюансы:
            foreach (var half in GetRelatedMembers(from, Relationship.ToHalf))
            {
                AddBidirectionalRelationship(half, to, Relationship.ToChild);
            }
        }


        #endregion
    }
}
