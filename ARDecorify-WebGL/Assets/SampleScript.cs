using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SampleScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] string json;
    [SerializeField]public Furnitures jc;
    [SerializeField] Objects objects;
    [SerializeField] Transform zero;
    void Start()
    {
        jc = JsonUtility.FromJson<Furnitures>(json);
        for (int i = 0; i<jc.furnitures.Length; i++)
        {
            int id = jc.furnitures[i].id;
            Transform objectt = Instantiate(objects.objects[id].objectModel).transform;
            objectt.SetParent(zero);
            objectt.position = jc.furnitures[i].transf;
            objectt.localEulerAngles = new Vector3(-90, 0, 0);
            objectt.localPosition += new Vector3(objectt.localPosition.x, 0.5f, objectt.localPosition.z);
        }
        //JsonUtility.FromJsonOverwrite(json, jc);
        //Debug.Log(jc.jcs.Length);
       //string s = JsonUtility.ToJson(jc);
       //jc = JsonUtility.FromJson<Furnitures>(json);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

[System.Serializable]
public class Furniture
{
    public int id;
    public Vector3 transf;
}

[System.Serializable]
public class Furnitures
{
    [SerializeField]public Furniture[] furnitures;
}
