//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager Instance;
        public GameObject EasyARSession;
        public SparseSpatialMapController MapControllerPrefab;
        public MainViewController MainView;
        public CreateViewController CreateView;
        public EditViewController EditView;
        public GameObject PreviewView;
        public Text Status;
        public Toggle EditPointCloudUI;
        public Toggle PreviewPointCloudUI;
        public Text RecycleBinText;
        public Button BackButton;
        public bool MainViewRecycleBinClearMapCacheOnly;

        private GameObject easyarObject;
        private ARSession session;
        private SparseSpatialMapWorkerFrameFilter mapFrameFilter;
        private List<MapMeta> selectedMaps = new List<MapMeta>();
        private MapSession mapSession;
        private string deviceModel = string.Empty;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void ImportSampleStreamingAssets()
        {
            FileUtil.ImportSampleStreamingAssets();
        }
#endif

        private void Awake()
        {
            Instance = this;
            MainView.gameObject.SetActive(false);
            CreateView.gameObject.SetActive(false);
            EditView.gameObject.SetActive(false);
            PreviewView.SetActive(false);
            if (MainViewRecycleBinClearMapCacheOnly)
            {
                RecycleBinText.text = "Delete only maps caches";
            }
            else
            {
                RecycleBinText.text = "Delete all maps and caches";
            }

            var launcher = "AllSamplesLauncher";
            if (Application.CanStreamedLevelBeLoaded(launcher))
            {
                var button = BackButton.GetComponent<Button>();
                button.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(launcher); });
            }
            else
            {
                BackButton.gameObject.SetActive(false);
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    using (var buildClass = new AndroidJavaClass("android.os.Build"))
                    {
                        deviceModel = $"(Device = {buildClass.GetStatic<string>("DEVICE")}, Model = {buildClass.GetStatic<string>("MODEL")})";
                    }
                }
                catch (Exception e) { deviceModel = e.Message; }
            }
#endif
        }

        private void Start()
        {
            LoadMainView();
        }

        private void Update()
        {
            if (session)
            {
                Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine +
                    "VIO Device" + Environment.NewLine +
                    "\tType: " + ((session.Assembly != null && session.Assembly.FrameSource) ? session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
                    "\tTracking Status: " + session.TrackingStatus + Environment.NewLine +
                    "CenterMode: " + session.CenterMode + Environment.NewLine +
                    "Sparse Spatial Map" + Environment.NewLine +
                    "\tWorking Mode: " + mapFrameFilter.WorkingMode + Environment.NewLine +
                    "\tLocalization Mode: " + mapFrameFilter.LocalizerConfig.LocalizationMode + Environment.NewLine +
                    "Localized Map" + Environment.NewLine +
                    "\tName: " + (mapFrameFilter.LocalizedMap == null ? "-" : (mapFrameFilter.LocalizedMap.MapInfo == null ? "-" : mapFrameFilter.LocalizedMap.MapInfo.Name)) + Environment.NewLine +
                    "\tID: " + (mapFrameFilter.LocalizedMap == null ? "-" : (mapFrameFilter.LocalizedMap.MapInfo == null ? "-" : mapFrameFilter.LocalizedMap.MapInfo.ID)) + Environment.NewLine +
                    "\tPoint Cloud Count: " + (mapFrameFilter.LocalizedMap == null ? "-" : mapFrameFilter.LocalizedMap.PointCloud.Count.ToString());

                if (mapFrameFilter.LocalizedMap == null)
                {
                    EditPointCloudUI.gameObject.SetActive(false);
                    PreviewPointCloudUI.gameObject.SetActive(false);
                }
                else
                {
                    EditPointCloudUI.gameObject.SetActive(true);
                    PreviewPointCloudUI.gameObject.SetActive(true);
                }
            }
            else
            {
                Status.text = string.Empty;
            }
        }

        private void OnDestroy()
        {
            DestroySession();
        }

        public void SelectMaps(List<MapMeta> metas)
        {
            selectedMaps = metas;
            MainView.EnablePreview(selectedMaps.Count > 0);
            MainView.EnableEdit(selectedMaps.Count == 1);
        }

        public void LoadMainView()
        {
            DestroySession();
            SelectMaps(new List<MapMeta>());
            MainView.gameObject.SetActive(true);
        }

        public void LoadCreateView()
        {
            CreateSession();
            mapSession.SetupMapBuilder(MapControllerPrefab);
            CreateView.SetMapSession(mapSession);
            CreateView.gameObject.SetActive(true);
        }

        public void LoadEditView()
        {
            CreateSession();
            mapSession.LoadMapMeta(MapControllerPrefab, true);
            EditView.SetMapSession(mapSession);
            EditView.gameObject.SetActive(true);
            EditPointCloudUI.isOn = true;
        }

        public void LoadPreviewView()
        {
            CreateSession();
            mapSession.LoadMapMeta(MapControllerPrefab, false);
            PreviewView.SetActive(true);
            PreviewPointCloudUI.isOn = false;
        }

        public void ShowParticle(bool show)
        {
            if (mapSession == null)
            {
                return;
            }
            foreach (var map in mapSession.Maps)
            {
                if (map.Controller) { map.Controller.ShowPointCloud = show; }
            }
        }

        public void SwitchCenterMode()
        {
            if (session.AvailableCenterMode.Count == 0) { return; }
            while (true)
            {
                session.CenterMode = (ARSession.ARCenterMode)(((int)session.CenterMode + 1) % Enum.GetValues(typeof(ARSession.ARCenterMode)).Length);
                if (session.AvailableCenterMode.Contains(session.CenterMode)) { break; }
            }
        }

        private void CreateSession()
        {
            easyarObject = Instantiate(EasyARSession);
            easyarObject.SetActive(true);
            session = easyarObject.GetComponent<ARSession>();
            mapFrameFilter = easyarObject.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();

            mapSession = new MapSession(session, mapFrameFilter, selectedMaps);
        }

        private void DestroySession()
        {
            if (mapSession != null)
            {
                mapSession.Dispose();
                mapSession = null;
            }
            if (easyarObject) { Destroy(easyarObject); }
        }
    }
}
