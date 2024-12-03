/*using UnityEngine;
using UnityEditor;

namespace PrYFam.Assets.Scripts.AdditionalExtensions
{
    /// <summary>
    /// ��������� �������� ��� ���������� TreeTraversal.
    /// ��������� ����������� ������� � ����� ������ (HorizontalSpacing, VerticalSpacing, GlobalTreeOffset)
    /// � ������� ����������� � ����������.
    /// </summary>
    [CustomEditor(typeof(TreeTraversal))]
    public class TreeSpacingEditor : Editor
    {
        /// <summary>
        /// ����� ��� ����������� ����������������� ���������� � ����������.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // �������� ������ �� ������ TreeTraversal, ������� �������������
            TreeTraversal treeSpacing = (TreeTraversal)target;

            // ������ ���� ��� �������������� HorizontalSpacing
            treeSpacing.HorizontalSpacing = EditorGUILayout.FloatField(
                "Horizontal Spacing",               // �������� ����
                treeSpacing.HorizontalSpacing       // ������� ��������
            );

            // ������ ���� ��� �������������� VerticalSpacing
            treeSpacing.VerticalSpacing = EditorGUILayout.FloatField(
                "Vertical Spacing",                 // �������� ����
                treeSpacing.VerticalSpacing         // ������� ��������
            );

            // ������������ ����������� � ������������ �������� ��� GlobalTreeOffset
            float minOffset = 1f * treeSpacing.HorizontalSpacing; // ����������� �������� = HorizontalSpacing
            float maxOffset = 3f * treeSpacing.HorizontalSpacing; // ������������ �������� = 3 * HorizontalSpacing

            // ������ �������� ��� �������������� GlobalTreeOffset
            treeSpacing.GlobalTreeOffset = EditorGUILayout.Slider(
                "Offset",                           // �������� ��������
                treeSpacing.GlobalTreeOffset,       // ������� ��������
                minOffset,                          // ����������� ��������
                maxOffset                           // ������������ ��������
            );

            // ���������, ���� �� ������� ��������� � ����������
            if (GUI.changed)
            {
                // �������� ������ ��� "���������", ����� Unity �������� ���������
                EditorUtility.SetDirty(treeSpacing);
            }
        }
    }
}
*/
