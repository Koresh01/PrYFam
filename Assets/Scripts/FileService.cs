using UnityEngine;

namespace PrYFam.Assets.Scripts
{
    /// <summary>
    /// ������ ��� ������ � �������, ��������� � ����������� � ��������� ������ ��������� �����.
    /// ��������� ������������� ������ FamilyData � ���� � ��������� ��� �� �����.
    /// </summary>
    public class FileService : MonoBehaviour
    {
        public static void Save(string fileName, FamilyData data)
        {
            /*
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(Application.persistentDataPath + "/" + fileName, json);
            */
            Debug.LogFormat("���������� ����� � ���� {0}.json", fileName);
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
            Debug.LogFormat("��������� ����� �� ����� {0}", filePath);
        }
    }
}
