using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    [SerializeField] string json;
    Furnitures input;
    [SerializeField] Objects furnituresSettings;
    [SerializeField] Transform room;
    [SerializeField] Transform zeroPoint;
    public static SceneManager instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        input = JsonUtility.FromJson<Furnitures>(json);
        SetTheScene(CalculateBorders());
        CreateFurnitures();
        
    }
    Vector3 CalculateBorders()
    {
        float borderSizeZ = 0;
        float borderSizeX = 0;
        for(int i = 0; i< input.furnitures.Length; i++)
        {

            if (borderSizeZ < Mathf.Abs(input.furnitures[i].transf.z))
            {
                borderSizeZ = Mathf.Abs(input.furnitures[i].transf.z);
            }
            if (borderSizeX < Mathf.Abs(input.furnitures[i].transf.x))
            {
                borderSizeX = Mathf.Abs(input.furnitures[i].transf.x);
            }
        }
        Debug.Log(borderSizeX.ToString() + borderSizeZ.ToString());
        return new Vector3(borderSizeX + 2, 0.1f, borderSizeZ + 2);
    }
    void SetTheScene(Vector3 borderSize)
    {
        room.localScale = borderSize;
        zeroPoint.SetParent(null);
        zeroPoint.localScale = Vector3.one;
    }

    void CreateFurnitures()
    {
        for (int i = 0; i < input.furnitures.Length; i++)
        {
            int id = input.furnitures[i].id;
            Transform furniture = Instantiate(furnituresSettings.objects[id].objectModel).transform;
            furniture.localEulerAngles = new Vector3(-90, 0, 0);
            furniture.localPosition = new Vector3(input.furnitures[i].transf.x / 2, 0.05f, input.furnitures[i].transf.z / 2);
        }
    }
    [System.Serializable]
    public struct Furniture
    {
        public int id;
        public Vector3 transf;
    }
    [System.Serializable]
    public struct Furnitures
    {
        public Furniture[] furnitures;
    }
}
