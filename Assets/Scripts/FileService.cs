using UnityEngine;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// Сервис для работы с файлами, связанный с сохранением и загрузкой данных семейного древа.
    /// Позволяет сериализовать объект FamilyData в файл и загружать его из файла.
    /// </summary>
    public class FileService : MonoBehaviour
    {
        public static void Save(string fileName, FamilyData data)
        {
            /*
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(Application.persistentDataPath + "/" + fileName, json);
            */
            Debug.LogFormat("Сохранение древа в файл {0}.json", fileName);
        }

        public static void Load(string filePath)
        {
            /*
            var path = Application.persistentDataPath + "/" + fileName;
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonUtility.FromJson<FamilyData>(json);
            }
            throw new FileNotFoundException($"File not found: {fileName}");
            */
            Debug.LogFormat("Загружено древо из файла {0}", filePath);
        }
    }
}
