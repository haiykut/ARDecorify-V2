
using UnityEngine;

public class SceneController : MonoBehaviour
{
    private FurnitureMap input;
    [SerializeField] private Objects furnituresSettings;
    [SerializeField] private Transform room;
    [SerializeField] private Transform zeroPoint;
    [SerializeField] private Character character;
    [SerializeField] internal Texture2D cursorRotateTexture;
    [SerializeField] internal Texture2D cursorNormalTexture;
 
    void Start()
    {
        input = JsonUtility.FromJson<FurnitureMap>(Initializer.instance.json);
        SetTheScene(CalculateBorders());
        CreateFurnitures();
    }
    Vector3 CalculateBorders()
    {
        float borderSizeZ = 0;
        float borderSizeX = 0;
        for(int i = 0; i< input.furnitures.Length; i++)
        {

            if (borderSizeZ < Mathf.Abs(input.furnitures[i].position.z))
            {
                borderSizeZ = Mathf.Abs(input.furnitures[i].position.z);
            }
            if (borderSizeX < Mathf.Abs(input.furnitures[i].position.x))
            {
                borderSizeX = Mathf.Abs(input.furnitures[i].position.x);
            }
        }
        return new Vector3(borderSizeX + 10, 0.1f, borderSizeZ + 10);
    }
    void SetTheScene(Vector3 borderSize)
    {
        room.localScale = borderSize;
        zeroPoint.SetParent(null);
        zeroPoint.localScale = Vector3.one;
        character.transform.position = new Vector3(0, zeroPoint.localPosition.y + 0.75f, borderSize.z - 10);
        character.transform.localEulerAngles = new Vector3(0, 180, 0);

    }

    void CreateFurnitures()
    {
        for (int i = 0; i < input.furnitures.Length; i++)
        {
            long furnitureId = input.furnitures[i].id;
            Transform furniture = Instantiate(furnituresSettings.objects.Find(x => x.id == furnitureId).objectModel);
            furniture.localPosition = new Vector3(input.furnitures[i].position.x, 0.05f, input.furnitures[i].position.z);
            furniture.localEulerAngles = new Vector3(input.furnitures[i].rotation.x, input.furnitures[i].rotation.y, input.furnitures[i].rotation.z);
            furniture.SetParent(zeroPoint);
        }
    }
    [System.Serializable]
    public struct Furniture
    {
        public long id;
        public Vector3 position;
        public Vector3 rotation;
    }
    [System.Serializable]
    public struct FurnitureMap
    {
        public Furniture[] furnitures;
    }
}
