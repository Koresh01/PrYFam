using UnityEditor;
using UnityEngine;

namespace PrYFam.Assets.Scripts.AdditionalExtensions
{
    [CustomEditor(typeof(TreeTraversal))]
    public class TreeSpacingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Получаем ссылку на редактируемый объект
            TreeTraversal treeSpacing = (TreeTraversal)target;

            // Отображаем стандартные поля для horizontalSpacing и verticalSpacing
            treeSpacing.HorizontalSpacing = EditorGUILayout.FloatField("Horizontal Spacing", treeSpacing.HorizontalSpacing);
            treeSpacing.VerticalSpacing = EditorGUILayout.FloatField("Vertical Spacing", treeSpacing.VerticalSpacing);

            // Ползунок для offset с динамическим диапазоном
            float minOffset = 0.5f * treeSpacing.HorizontalSpacing;
            float maxOffset = 2f * treeSpacing.HorizontalSpacing;
            treeSpacing.Offset = EditorGUILayout.Slider("Offset", treeSpacing.Offset, minOffset, maxOffset);

            // Сохраняем изменения
            if (GUI.changed)
            {
                EditorUtility.SetDirty(treeSpacing);
            }
        }
    }
}
