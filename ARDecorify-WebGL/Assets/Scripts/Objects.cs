using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "haiykut/ModelSettings")]
public class Objects : ScriptableObject
{
    public List<Object> objects;
    [System.Serializable]
    public class Object
    {
        public long id;
        public Transform objectModel;
    }
}
