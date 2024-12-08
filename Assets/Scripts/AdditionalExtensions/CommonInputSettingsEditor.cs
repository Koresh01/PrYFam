using UnityEditor;
using UnityEngine;

namespace PrYFam.Assets.Scripts.AdditionalExtensions
{
    [CustomEditor(typeof(CommonInputSettings))]
    public class CommonInputSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Получаем объект ScriptableObject
            CommonInputSettings settings = (CommonInputSettings)target;

            // Отображаем поле для камеры
            settings.mainCamera = (Camera)EditorGUILayout.ObjectField("Камера", settings.mainCamera, typeof(Camera), true);


            // Заголовок
            EditorGUILayout.LabelField("Настройки зума", EditorStyles.boldLabel);

            // Синхронизированные ползунки
            EditorGUI.BeginChangeCheck();

            // Отображаем ползунки
            float maxZoom = EditorGUILayout.Slider("maxZoom (ближе к карточкам)", settings.maxZoom, 2f, 120f);  // Значение позиции камеры по оси Z (ближе к карточкам)
            float minZoom = EditorGUILayout.Slider("minZoom (дальше от карточек)", settings.minZoom, 2f, 120f);  // Значение позиции камеры по оси Z (дальше от карточек)

            // Логика ограничения
            if (minZoom < maxZoom)
            {
                minZoom = maxZoom;
            }

            // Присваиваем значения обратно
            settings.minZoom = minZoom;
            settings.maxZoom = maxZoom;

            // Сохраняем изменения
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
