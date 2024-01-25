
using UnityEngine;

public class SceneController : MonoBehaviour
{
    private string json;
    private FurnitureMap input;
    [SerializeField] private Objects furnituresSettings;
    [SerializeField] private Transform room;
    [SerializeField] private Transform zeroPoint;
    [SerializeField] private Character character;
    [SerializeField] internal Texture2D cursorRotateTexture;
    [SerializeField] internal Texture2D cursorNormalTexture;
    private void Awake()
    {
        json = Initializer.instance.json;
    }
    void Start()
    {
        input = JsonUtility.FromJson<FurnitureMap>(json);
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
        return new Vector3(borderSizeX + 3, 0.1f, borderSizeZ + 3);
    }
    void SetTheScene(Vector3 borderSize)
    {
        room.localScale = borderSize;
        zeroPoint.SetParent(null);
        zeroPoint.localScale = Vector3.one;
        character.transform.SetLocalPositionAndRotation(zeroPoint.localPosition + new Vector3(0,0.75f,0), zeroPoint.localRotation);
    }

    void CreateFurnitures()
    {
        for (int i = 0; i < input.furnitures.Length; i++)
        {
            int id = input.furnitures[i].id;
            Transform furniture = Instantiate(furnituresSettings.objects[id].objectModel).transform;
            furniture.localEulerAngles = new Vector3(-90, 0, 0);
            furniture.localPosition = new Vector3(input.furnitures[i].position.x / 2, 0.05f, input.furnitures[i].position.z / 2);
            furniture.localEulerAngles = new Vector3(-90, input.furnitures[i].rotation.y, input.furnitures[i].rotation.z);
            furniture.SetParent(zeroPoint);
        }
    }
    [System.Serializable]
    public struct Furniture
    {
        public int id;
        public Vector3 position;
        public Vector3 rotation;
    }
    [System.Serializable]
    public struct FurnitureMap
    {
        public Furniture[] furnitures;
    }
}
