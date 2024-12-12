# Программа для построения генеологических деревьев, для Android, Windows, IOS и т.д. Используем Unity3D.

## Общий вид программы(демо)

![image](https://github.com/user-attachments/assets/001cd7b2-feb5-4943-a976-f0a89bc6b89b)
<p align="center"><i>рис.1 (Пример карточки члена семьи)</i></p>

![image](https://github.com/user-attachments/assets/19a39bdb-0831-4cff-85bf-937dbe7af2f2)
<p align="center"><i>рис.2 (Пример построенного древа)</i></p>


## Идея

Давайте создадим такую программу, с помощью которой любой человек сможет построить своё семейное древо. 

## Требования
Вот что хотелось бы видеть в результате:
- Возможность __строить и редактировать__ древо.
- Добавление/удаление __изображений своих родственников__.
- __Импорт/экспорт__ древа в файл/из файла.
- Кроссплатформенность (чтобы редактирование можно было осушествлять как на ПК, так и на мобильном устройстве)

![image](https://github.com/user-attachments/assets/9ab32acd-2c2d-4d8e-a029-5cc472dcb271)
<p align="center"><i>рис.3 (Пример импорт/экспорт панели)</i></p>

<u>Управление для мобильных устройств:</u> 
- Свайпы, для перемещения
- Масштабирование 2умя пальцами

<u>Управление для ПК:</u>
- Перемещение через кнопки WASD.
- Масштабирование через mouse scroll.

<u>Управление общее для всех устройств:</u>
- масшабирование древа с помощью кнопок + - и слайдера на экране

<p align="center">
  <img src="https://github.com/user-attachments/assets/7778be80-1e99-4ecd-ac03-eb81c8b60eaa" alt="image" />
</p>
<p align="center"><i>рис.4 (Слайдер и кнопки для масштабирования)</i></p>

## Дополнительные референсы
Очевидно, что такие программы уже есть. Проанализировав готовые решения можно понять, что при построении семейных деревьев могут возникать сложные моменты, например:
* Многоженство, и разные дети от разных жён.
* Невозможность отрисовки всего древа целиком.

![image](https://github.com/user-attachments/assets/e589dba6-ec44-49f6-bae3-248aa18c60b9)
<p align="center"><i>рис.5 (Невозможность отрисовать родителей среднего ребёнка без самопересечения древа)</i></p>


Вот так предлагаю пока что реализовать кнопки доабвления (детей, родителей, жён):
![image](https://github.com/user-attachments/assets/384988b8-6718-4e5b-a06e-dbe33d8b10b1)
<p align="center"><i>рис.6 (Невозможность отрисовать родителей среднего ребёнка без самопересечения древа)</i></p>

## От идеи до реализации
Как же мы будем хранить данные о нашем древе в коде? Первое что приходит в голову - это использовать стандартные способы хранения графов:


### Способы хранения графов

Графы можно хранить разными способами в зависимости от их типа (ориентированные или неориентированные) и частоты операций, которые необходимо выполнять. Вот основные способы хранения графов:

---

1. **Списки смежности (Adjacency List)**
Описание: Каждой вершине соответствует список (или коллекция), содержащий её смежные вершины.
Используется: Когда граф разреженный (много вершин, но мало рёбер).

Преимущества:
- Экономит память в случае разреженных графов.
- Эффективен для обхода графа.
- 
Недостатки:
- Поиск наличия рёбер между двумя вершинами может быть медленным.

Пример:

```
Dictionary<int, List<int>> adjacencyList;
```

---

2. **Матрица смежности (Adjacency Matrix)**
Описание: Это двумерный массив (матрица), где элемент на позиции [i, j] равен 1, если существует ребро между вершинами i и j, и 0 в противном случае.
Используется: Когда граф плотный (много рёбер).

Преимущества:
- Быстрый доступ для проверки наличия ребра между двумя вершинами (O(1)).
- Прост в реализации.

Недостатки:
- Потребляет много памяти для разреженных графов (O(n²), где n — количество вершин).
- Неэффективен для графов с малым количеством рёбер.
  
Пример:

```C#
int[,] adjacencyMatrix;
```

3. **Список рёбер (Edge List)**
Описание: Список всех рёбер графа, где каждое ребро представлено как пара вершин (или тройка для взвешенных рёбер).
Используется: В случаях, когда нужно хранить рёбра в графах с небольшим числом рёбер.

Преимущества:
- Простота реализации.
- Хорошо подходит для алгоритмов, которые работают с рёбрами (например, поиск минимального пути).

Недостатки:
- Медленный поиск смежных вершин.
- Неэффективен для частых запросов на поиск смежности.
  
Пример:

```C#
List<Tuple<int, int>> edgeList;
```

---

4. **Список инцидентности (Incident List)**
Описание: Для каждой вершины хранится список рёбер, инцидентных этой вершине. Каждое ребро представлено как пара вершин.
Используется: Когда требуется информация о рёбрах, инцидентных вершинам.

Преимущества:
- Удобен для задач, связанных с анализом рёбер.

Недостатки:
- Неэффективен для часто используемых операций поиска смежности.
  
Пример:

```C#
Dictionary<int, List<Tuple<int, int>>> incidentList;
```

---

5. **Сжатая матрица смежности (Compressed Adjacency Matrix)**
Описание: Это улучшенная версия матрицы смежности, где для хранения рёбер используется структура данных, оптимизированная для разреженных графов (например, с использованием списков или битовых массивов).
Используется: Для разреженных графов, где количество рёбер значительно меньше, чем количество возможных рёбер.

Преимущества:
- Экономит память по сравнению с обычной матрицей смежности.

Недостатки:
- Сложность в реализации и модификации.

---

6. **Представление с помощью объектов и списков (Object-oriented representation)**
Описание: Каждый граф может быть представлен как объект, содержащий вершины и рёбра, где вершины могут быть объектами с собственными атрибутами (например, координаты, метки).
Используется: Когда требуется гибкость в представлении и манипуляции графами.

Преимущества:
- Удобно для специфичных приложений, где вершины и рёбра имеют дополнительные данные.

Недостатки:
- Требует больше времени на реализацию и управление памятью.

---

7. **Блоки для динамических графов (Dynamic Graph Representations)**
Описание: Используются специализированные структуры данных, которые оптимизируют операции для изменяющихся графов, таких как добавление и удаление рёбер и вершин.
Используется: В динамических графах, где часто изменяется структура (например, в графах социальных сетей).

Преимущества:
- Позволяют эффективно управлять изменениями в графе.

Недостатки:
- Обычно сложнее в реализации.


**Резюме:**
Списки смежности хороши для разреженных графов и когда важен быстрый доступ к смежным вершинам.
Матрицы смежности удобны для плотных графов, где важно быстрое определение наличия ребра.
Список рёбер полезен для хранения рёбер, когда важна работа с рёбрами (например, алгоритмы поиска минимального пути).


---

**При реализации этого проекта, я решил, что буду использовать список смежности для хранения нашего древа-графа. НО!**

---



### Какой граф будет у нас?
Графы бывают направленные и не направленные.

![image](https://github.com/user-attachments/assets/7fea6b11-3355-4388-b135-30693821f160)


<p align="center"><i>рис.7 (Направленные и не направленные графы)</i></p>


Очевидно что наш граф - направленный, но я поссчитал что этого не достаточно. Нужно конкретнее описывать связи. Вот как с этим справился Я:

Перечисление enum _Relationship.cs_:
```C#
namespace PrYFam.Assets.Scripts
{
    public enum Relationship
    {
        ToChild,
        ToHalf,
        ToParent,
        None
    }
}
```

Ну и вот как выглядит класс для хранения нешего списка смежности _FamilyData.cs_:
```C#
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
```


А добавлением и удалением элементов в этом списке смежности занимается _FamilyService.cs_:
```C#
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

```
