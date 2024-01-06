using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using easyar;
using System.Security.Cryptography;

public class SceneController : MonoBehaviour
{
    [Header("Selective Object")]
    public Lean.Touch.LeanSelectByFinger selectiveObject;
    [Header("Object Setting")]
    public Objects objectSetting;
    [Header("Image Tracking Object And Zero Point")]
    public GameObject imageTracking;
    public Transform zeroPoint;
    bool imageTrackingControl;
    [Header("Button And Text Variables")]
    public Text status;
    public Button createObjButton;
    public Button deleteObjButton;
    public Button deleteAllButton;
    public Button resetButton;
    public Button setButton;
    public Button confirmButton;
    public GameObject furnituresPanel;
    public GameObject furnituresPanelChild;
    string activeStatus = "Hedef Resim Bulundu";
    string deactiveStatus = "Hedef Resim Yok";
    bool generalControl;
    GameObject[] sceneFurnitureArr;
    List<GameObject> sceneFurnitureList;
    public List<Furniture> confirmedFurnituresList = new List<Furniture>();
    public static SceneController instance;
    [SerializeField] HttpRequestController httpReqController;
    [SerializeField] List<GameObject> allUiElements;
    [SerializeField] GameObject arGuide;
    void Awake()
    {
        instance = this;
        zeroPoint.gameObject.SetActive(true);
      
    }

    public void StartApp()
    {
        for (int i = 0; i < allUiElements.Count; i++)
        {
            allUiElements[i].SetActive(true);
        }
        arGuide.SetActive(false);
        /*
        //TODO: Dev
        Furniture f = new Furniture();
        f.id = 3;
        f.transform = transform.position;
        confirmedFurnituresList.Add(f);
        Furnitures confirmedFurnitures = new Furnitures { furnitures = confirmedFurnituresList };
        httpReqController.JsonGenerator(confirmedFurnitures);
        */
    }

    private void Start()
    {
        resetButton.interactable = false;
        httpReqController = (HttpRequestController)FindObjectOfType(typeof(HttpRequestController));
        for (int i = 0; i < furnituresPanelChild.transform.childCount; i++)
        {
            int id = i;
            Debug.Log(id);
            Button button = furnituresPanelChild.transform.GetChild(id).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate
            {
                FurnitureCreate(objectSetting.objects[id].id);
            });
        }
    }

    void Update()
    {
        if (!generalControl)
        {
            if (imageTracking.active == false)
            {
                if (!imageTrackingControl)
                {
                    status.text = deactiveStatus;
                    createObjButton.interactable = false;
                    setButton.interactable = false;
                    imageTrackingControl = true;
                }
            }
            if (imageTracking.active == true)
            {
                if (imageTrackingControl)
                {
                    setButton.interactable = true;
                    status.text = activeStatus;
                    imageTrackingControl = false;
                }
            }
        }

        if (GameObject.FindGameObjectsWithTag("Furniture").Length <= 0)
        {
            deleteAllButton.interactable = false;
            confirmButton.interactable = false;
        }
        else
        {
            deleteAllButton.interactable = true;
            confirmButton.interactable = true;
        }
        if (selectiveObject.Selectables.Count > 0)
        {
            deleteObjButton.interactable = true;
        }
        else
        {
            deleteObjButton.interactable = false;
        }
    }
    public void DestroyAllFurnitures()
    {
        for (int i = 0; i < zeroPoint.childCount; i++)
        {
            Destroy(zeroPoint.GetChild(i).gameObject);
        }
    }
    public void ResetZeroPoint()
    {
        generalControl = false;
        status.gameObject.SetActive(true);
        resetButton.interactable = false;
        createObjButton.interactable = false;
        status.gameObject.SetActive(true);
        if (imageTracking.active)
            setButton.interactable = true;
        else
            setButton.interactable = false;
    }
    public void BackFromFurnituresPanel()
    {
        furnituresPanel.SetActive(false);
    }
    public void SetZeroPoint()
    {
        generalControl = true;
        createObjButton.interactable = true;
        resetButton.interactable = true;
        setButton.interactable = false;
        status.gameObject.SetActive(false);
        zeroPoint.localPosition = new Vector3(imageTracking.transform.position.x, 0, imageTracking.transform.position.z);
        zeroPoint.localRotation = imageTracking.transform.localRotation;
        createObjButton.interactable = true;
        status.gameObject.SetActive(false);
    }
    public void DestroySelectedFurniture()
    {
        Destroy(selectiveObject.Selectables[0].gameObject);
    }

    public GameObject isDoneSess;
    IEnumerator OpenAndCloseGO(GameObject go, float time)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(time);
        go.SetActive(false);
    }
    public void ConfirmScene()
    {
        sceneFurnitureArr = GameObject.FindGameObjectsWithTag("Furniture"); // Sahnedeki tum esyalarin arrayi

        for (int i = 0; i < sceneFurnitureArr.Length; i++)
        {
            Furniture o = new Furniture { id = sceneFurnitureArr[i].GetComponent<FurnitureScript>().id, transform = sceneFurnitureArr[i].transform.position }; // Tekil esyalarin her birisi ayri
                                                                                                                                                               // birer nesne olarak olusturuluyor
                                                                                                                                                               // FurnitueScript bir scriptableobject
            confirmedFurnituresList.Add(o); // Olusturulan her nesne bir listeye ekleniyor
        }
        Furnitures confirmedFurnitures = new Furnitures { furnitures = confirmedFurnituresList }; // Furnitures classinin icerisinde tekil esyalarin bir listesi bulunuyor ve bu listeyle onaylanan
                                                                                             // esyalarin listesi esitleniyor.
                                                                                             // Json formatina cevirebilmek icin class kullanmak zorunlu.

        httpReqController.JsonGenerator(confirmedFurnitures); //Web kismina post istegi ile beraber json formatinda furnitures classi (tekil esyalarin listesinin bulundugu class) gonderiyor.
    }
    public void FinishApp()
    {
        if (httpReqController.isDoneSession)
        {
            StartCoroutine(OpenAndCloseGO(isDoneSess, 4));
            for (int i = 0; i < zeroPoint.childCount; i++)
            {
                Destroy(zeroPoint.GetChild(i).gameObject);
            }
        }
    }
    public void FurnitureCreateButton()
    {
        furnituresPanel.SetActive(true);
    }
    bool asdf;
    public void FurnitureCreate(int index)
    {
        Transform transform = Instantiate(objectSetting.objects[index].objectModel);
        FurnitureScript singleObject = transform.gameObject.AddComponent<FurnitureScript>();
        singleObject.id = objectSetting.objects[index].id;
        transform.SetParent(zeroPoint.transform);
        transform.localPosition = new Vector3(0, 0, 0f);
        transform.localEulerAngles = new Vector3(180, 0, 0f);
        furnituresPanel.SetActive(false);
    }

    [System.Serializable]
    public struct Furniture
    {
        public int id;
        public Vector3 transform;
    }
    [System.Serializable]
    public class Furnitures
    {
        public List<Furniture> furnitures;
    }

}