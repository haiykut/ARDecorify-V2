using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HttpRequestController : MonoBehaviour
{
    [SerializeField] InputField usernameArea;
    [SerializeField] InputField passwordArea;
    [SerializeField] string authUrl;
    [SerializeField] string confirmUrl;
    [SerializeField] string registerUrl;
    [SerializeField] List<string> strings;
    [SerializeField] List<Color> stringColors;
    [SerializeField] Text status;
    [SerializeField] Text waiting;
    internal bool isDoneSession;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        
        //TODO: Dev
        User ud = new User();
        ud.username = "deneme";
        ud.password = "Password";
        UserMap u = new UserMap();
        u.user = ud;
        Debug.Log(JsonUtility.ToJson(u,true));
        
    }
    public void SubmitMethod()
    {
        if (usernameArea.text != "" && passwordArea.text != "")
        {
            JsonGenerator(usernameArea.text, passwordArea.text);
        
        }
        else
        {
            status.text = strings[3];
            status.color = stringColors[3];
            StartCoroutine(ObjectCloser(status.gameObject, 2, false));
        }

    }
    public void Register()
    {
        Application.OpenURL(registerUrl);
    }

    void JsonGenerator(string username, string password)
    {
        User user = new User{ username = username, password = password };
        UserMap userMap = new UserMap { user = user };
        StartCoroutine(AuthPost(authUrl, JsonUtility.ToJson(user)));
    }
    public void JsonGenerator(SceneController.FurnitureMap furnitureMap)
    {
        //make a post request to server.
        StartCoroutine(ConfirmPost(confirmUrl, JsonUtility.ToJson(furnitureMap, true)));
        Debug.Log(JsonUtility.ToJson(furnitureMap, true));
    }
    IEnumerator ConfirmPost(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.responseCode == 200)
        {
            isDoneSession = true;
            SceneController.instance.FinishApp();
        }
        else
        {
            isDoneSession = false;
        }
        request.Dispose();
        request.uploadHandler.Dispose();
        request.downloadHandler.Dispose();
    }
    IEnumerator AuthPost(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        waiting.gameObject.SetActive(true);
        yield return request.SendWebRequest();
        if(request.responseCode==200)
        {
            
            if (request.downloadHandler.text == "true")
            {
                status.text = strings[0];
                status.color = stringColors[0];
                StartCoroutine(ObjectCloser(status.gameObject, 2, false));
                yield return new WaitForSeconds(2);
                SceneManager.LoadScene("SurfaceTracking_ImageTarget");
            }

            else
            {
                status.text = strings[1];
                status.color = stringColors[1];
                StartCoroutine(ObjectCloser(status.gameObject, 2, false));
            }

        }
        else
        {
            status.text = strings[2];
            status.color = stringColors[2];
            StartCoroutine(ObjectCloser(status.gameObject, 2, false));
            Debug.Log("network error");
        }

        request.Dispose();
        request.uploadHandler.Dispose();
        request.downloadHandler.Dispose();


    }

    IEnumerator ObjectCloser(GameObject go, float time, bool isClosing)
    {
        if (isClosing)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }
        else
        {
            waiting.gameObject.SetActive(false);
            go.SetActive(true);
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }
      
    }
}
[System.Serializable]
public struct User
{
    public string username;
    public string password;
    
}
[System.Serializable]
public struct UserMap
{
    public User user;
}
