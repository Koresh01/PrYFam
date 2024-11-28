using UnityEngine;

namespace PrYFam.Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        // Необходимые зависимости:
        [SerializeField] private FamilyService familyService;
        [SerializeField] private TreeTraversal treeTraversal;
        void Start()
        {
            //CreateFamilyTree();
        }

        public void CreateFamilyTree()
        {
            /*
            GameObject go1 = familyService.newMember();
            GameObject go2 = familyService.newMember();
            familyService.AddConnection(go1, go2, Relationship.ToChild);
            */
            Debug.Log("Family tree was created.");
        }

        /*
        public void SaveFamilyTree(string fileName)
        {
            FamilyData data = familyService.GetFamilyData();
            FileService.Save("name", data);
        }

        public void LoadFamilyTree(string filePath)
        {
            FamilyData data = FileService.Load(filePath);
        }
        */
    }
}
