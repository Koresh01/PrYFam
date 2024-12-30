using UnityEngine;

namespace PrYFam
{
    public class GameManager : MonoBehaviour
    {
        // Необходимые зависимости:
        [SerializeField] private FamilyService familyService;
        [SerializeField] private TreeTraversal treeTraversal;
        void Start()
        {
            CreateFamilyTree();
        }

        public void CreateFamilyTree()
        {
            GameObject go1 = familyService.CreateCard();
            //GameObject go2 = familyService.CreateCard();

            Member from = go1.GetComponent<Member>();
            //Member to = go2.GetComponent<Member>();


            //familyService.AddConnection(from, to, Relationship.ToChild);
            treeTraversal.ReDrawTree(from, new Vector2(0,0));


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
