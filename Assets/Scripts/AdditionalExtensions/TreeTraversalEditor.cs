/*using UnityEngine;
using UnityEditor;

namespace PrYFam.Assets.Scripts.AdditionalExtensions
{
    /// <summary>
    /// Кастомный редактор для компонента TreeTraversal.
    /// Позволяет настраивать отступы и сдвиг дерева (HorizontalSpacing, VerticalSpacing, GlobalTreeOffset)
    /// с удобным интерфейсом в инспекторе.
    /// </summary>
    [CustomEditor(typeof(TreeTraversal))]
    public class TreeSpacingEditor : Editor
    {
        /// <summary>
        /// Метод для отображения пользовательского интерфейса в инспекторе.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Получаем ссылку на объект TreeTraversal, который редактируется
            TreeTraversal treeSpacing = (TreeTraversal)target;

            // Рисуем поле для редактирования HorizontalSpacing
            treeSpacing.HorizontalSpacing = EditorGUILayout.FloatField(
                "Horizontal Spacing",               // Название поля
                treeSpacing.HorizontalSpacing       // Текущее значение
            );

            // Рисуем поле для редактирования VerticalSpacing
            treeSpacing.VerticalSpacing = EditorGUILayout.FloatField(
                "Vertical Spacing",                 // Название поля
                treeSpacing.VerticalSpacing         // Текущее значение
            );

            // Рассчитываем минимальное и максимальное значение для GlobalTreeOffset
            float minOffset = 1f * treeSpacing.HorizontalSpacing; // Минимальное значение = HorizontalSpacing
            float maxOffset = 3f * treeSpacing.HorizontalSpacing; // Максимальное значение = 3 * HorizontalSpacing

            // Рисуем ползунок для редактирования GlobalTreeOffset
            treeSpacing.GlobalTreeOffset = EditorGUILayout.Slider(
                "Offset",                           // Название ползунка
                treeSpacing.GlobalTreeOffset,       // Текущее значение
                minOffset,                          // Минимальное значение
                maxOffset                           // Максимальное значение
            );

            // Проверяем, были ли внесены изменения в инспекторе
            if (GUI.changed)
            {
                // Помечаем объект как "изменённый", чтобы Unity сохранил изменения
                EditorUtility.SetDirty(treeSpacing);
            }
        }
    }
}
*/
