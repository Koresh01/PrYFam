using System;
using System.Linq;  // для доступа к .Where в List<>
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

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

        
        








        
        #region Создание новой персоны
        /// <summary>
        /// Добавляет нового человека в семью и добавляет SMART-связь к нему.
        /// </summary>
        public void AddNewMember(GameObject From, Relationship relationship)
        {
            Member from = From.GetComponent<Member>();
            if (!CanAddConnection(from, relationship)) return;
            
            GameObject newMemberObj = CreateCard();
            Member to = newMemberObj.GetComponent<Member>();

            AddConnection(from, to, relationship);
        }     
        /// <summary>
        /// Создаёт карточку префаба человека.
        /// </summary>
        public GameObject CreateCard()
        {
            var go = Instantiate(personCardPrefab, cardsPlaceHolder);
            go.SetActive(false);    // Отображаться они начнут после отрисовки.
            return go;
        }
        /// <summary>
        /// Добавляет новую связь в FamilyData.
        /// </summary>
        public void AddConnection(Member from, Member to, Relationship relationship = Relationship.None)
        {
            // обработка неликвидных ситуаций
            switch (relationship)
            {
                case Relationship.ToHalf:
                    Member newHalf = to;
                    AddBidirectionalRelationship(from, newHalf, Relationship.ToHalf);
                    break;



                case Relationship.ToParent:
                    Member newParent = to;
                    AddBidirectionalRelationship(from, newParent, Relationship.ToParent);

                    foreach (var parent in GetParentMembers(from))
                    {
                        if (parent != newParent)
                        {
                            AddBidirectionalRelationship(parent, newParent, Relationship.ToHalf);
                        }
                    }
                    break;



                case Relationship.ToChild:
                    Member child = to;
                    List<Member> mothers = GetHalfMembers(from);
                    Member selectedMum = GetSelectedHalf(from);

                    AddBidirectionalRelationship(from, child, Relationship.ToChild);
                    AddBidirectionalRelationship(selectedMum, child, Relationship.ToChild);
                    break;



                default:
                    Debug.LogWarning("Неизвестное отношение: " + relationship);
                    break;
            }
        }
        #endregion











        #region Дополнительные проверки при создании персоны
        /// <summary>
        /// Проверяет можно ли добавить взаимосвязь.
        /// </summary>
        private bool CanAddConnection(Member from, Relationship relationship)
        {
            switch (relationship)
            {
                case Relationship.ToHalf:
                    if (!CanAddHalf(from)) return false; // теперь можно добавлять сколько угодно half.
                    break;
                case Relationship.ToParent:
                    if (!CanAddParent(from)) return false;
                    break;
                case Relationship.ToChild:
                    if (!CanAddChild(from)) return false;
                    break;
            }
            return true;
        }
        /// <summary>
        /// Проверяет можно ли добавить жену.
        /// </summary>
        private bool CanAddHalf(Member from)
        {
            /*if (hasHalf(from))
            {
                Debug.Log("Вторую жену добавить нельзя!");
                return false;
            }*/
            return true;
        }
        /// <summary>
        /// Проверяет можно ли добавить ещё родителя.
        /// </summary>
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
        /// Проверяет можно ли добавить ребёнка
        /// </summary>
        private bool CanAddChild(Member cur)
        {
            List<Member> mothers = GetHalfMembers(cur);

            if (mothers.Count >= 1)
            {
                return true;
            }
            else
            {
                Debug.LogError("Прежде чем создавать детей, добавьте хоть одну жену.");
                return false;
            } 
        }
        #endregion











        #region Работа с взаимосвязями
        /// <summary>
        /// Добавляет двунаправленную связь между членами.
        /// </summary>
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







        #region Получение прилежащих членов
        /// <summary>
        /// Возвращает список детей.
        /// </summary>
        /// <param name="cur">Член семьи, у которого ищем детей.</param>
        public List<Member> GetChildMembers(Member cur)
        {
            return GetRelatedMembers(cur, Relationship.ToChild);
        }
        /// <summary>
        /// Возвращает список детей, которые принадлежат указанным родителям.
        /// Учитываются только общие дети, игнорируются дети от других партнёров.
        /// </summary>
        public List<Member> GetChildMembers(Member father, Member mother)
        {
            // Получаем списки детей отца и матери
            List<Member> fatherChildren = GetRelatedMembers(father, Relationship.ToChild);
            List<Member> motherChildren = GetRelatedMembers(mother, Relationship.ToChild);

            // Берём только общих детей
            List<Member> children = fatherChildren.Intersect(motherChildren).ToList();

            return children;
        }
        /// <summary>
        /// Возвращает список родителей.
        /// </summary>
        /// <param name="cur">Член семьи, у которого ищем детей.</param>
        public List<Member> GetParentMembers(Member cur)
        {
            return GetRelatedMembers(cur, Relationship.ToParent);
        }
        /// <summary>
        /// Возвращает список партнёров.
        /// </summary>
        /// <param name="cur">Член семьи, у которого ищем детей.</param>
        public List<Member> GetHalfMembers(Member cur)
        {
            return GetRelatedMembers(cur, Relationship.ToHalf);
        }
        /// <summary>
        /// Получаем выбранную на данный момент жену.
        /// </summary>
        public Member GetSelectedHalf(Member cur)
        {
            List<Member> halfs = GetHalfMembers(cur);

            if (halfs.Count == 0)
                return null;

            // Обращаемся к карточке на сцене, чтобы получить индекс выбранной на данный момент жены.
            GameObject personCard = cur.gameObject;
            SelectedWifeController wifeController = personCard.GetComponent<SelectedWifeController>();
            int inx = wifeController.GetInx(halfs);

            return halfs[inx];
        }
        /// <summary>
        /// Работает непосредственно с FamilyData для получения списка необходимых членов семьи.
        /// </summary>
        private List<Member> GetRelatedMembers(Member from, Relationship relationship)
        {
            return familyData.relationships
                .Where(e => e.From == from && e.Relationship == relationship)
                .Select(e => e.To)
                .ToList();
        }
        #endregion









        #region дополнительные проверки
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
            if (!hasParents(cur) && !hasChildren(cur) && hasHalf(cur))
                return true;
            if (!hasParents(cur) && hasChildren(cur) && !hasHalf(cur) && GetChildMembers(cur).Count <= 1)
                return true;

            Debug.LogError("Невозможно удалить без последствий целостности древа!!!");
            return false;
        }
        public bool hasChildren(Member current)
        {
            return GetChildMembers(current).Count == 0 ? false : true;
        }
        public bool hasParents(Member current)
        {
            return GetParentMembers(current).Count == 0 ? false : true;
        }
        public bool hasHalf(Member current)
        {
            return GetHalfMembers(current).Count > 0 ? true : false;
        }
        #endregion
    }
}
