using UnityEditor;
using UnityEngine;

namespace PrYFam.Assets.Scripts.AdditionalExtensions
{
    [CustomEditor(typeof(TreeTraversal))]
    public class TreeSpacingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // �������� ������ �� ������������� ������
            TreeTraversal treeSpacing = (TreeTraversal)target;

            // ���������� ����������� ���� ��� horizontalSpacing � verticalSpacing
            treeSpacing.HorizontalSpacing = EditorGUILayout.FloatField("Horizontal Spacing", treeSpacing.HorizontalSpacing);
            treeSpacing.VerticalSpacing = EditorGUILayout.FloatField("Vertical Spacing", treeSpacing.VerticalSpacing);

            // �������� ��� offset � ������������ ����������
            float minOffset = 0.5f * treeSpacing.HorizontalSpacing;
            float maxOffset = 2f * treeSpacing.HorizontalSpacing;
            treeSpacing.Offset = EditorGUILayout.Slider("Offset", treeSpacing.Offset, minOffset, maxOffset);

            // ��������� ���������
            if (GUI.changed)
            {
                EditorUtility.SetDirty(treeSpacing);
            }
        }
    }
}
