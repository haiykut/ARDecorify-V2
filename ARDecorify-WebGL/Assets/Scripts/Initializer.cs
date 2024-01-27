using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
public class Initializer : MonoBehaviour
{
    internal string json;
    public static Initializer instance;
    [Header("Set Your BaseUrl that not contains id. \nExample: \"localhost:8080/api/unity/webgl\" ")]
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
        StartCoroutine(GetData(url));   
    }
    IEnumerator GetData(string url)
    {
        string appUrl = Application.absoluteURL;
        int lastIndex = appUrl.LastIndexOf('/');
        string dataUrl = url + appUrl.Substring(lastIndex);
        var request = new UnityWebRequest(dataUrl, "GET");    
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if(request.responseCode == 200)
        {
            json = request.downloadHandler.text;
            SceneManager.LoadScene(1);
        }
    }
}
