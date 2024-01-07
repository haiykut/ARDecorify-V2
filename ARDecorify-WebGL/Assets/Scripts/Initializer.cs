using UnityEngine;
using UnityEngine.SceneManagement;
public class Initializer : MonoBehaviour
{
    [SerializeField] internal TextAsset json;
    public static Initializer instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        else
            instance = this;
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        SceneManager.LoadScene(1);
    }

}
