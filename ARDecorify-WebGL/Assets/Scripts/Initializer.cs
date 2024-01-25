using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
public class Initializer : MonoBehaviour
{
    [SerializeField] internal string json;
    public static Initializer instance;
    public int id;
    public string url;
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
        //SceneManager.LoadScene(1);
        StartCoroutine(GetData(url, id));

    }
    IEnumerator GetData(string url, int x)
    {
        string newUrl = url + "/" + x.ToString();
        Debug.Log(newUrl);
        var request = new UnityWebRequest(newUrl, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        
        yield return request.SendWebRequest();
        if(request.responseCode == 200)
        {
            json = request.downloadHandler.text;
            Debug.Log(request.downloadHandler.text);
            SceneManager.LoadScene(1);
        }
    }

}
