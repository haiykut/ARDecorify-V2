//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================


using UnityEngine;
using UnityEngine.EventSystems;
using easyar;
using UnityEngine.UI;
using System;
using Common;

namespace SpatialMap_Sparse_ImageTarget
{
    public class SparseSpatailMapSample : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public TouchController TouchControl;
        public ImageTargetController ImageTarget;
        public Button BackButton;

        private SparseSpatialMapWorkerFrameFilter sparse;
        private bool onSparse;
        private bool found;
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
            sparse = Session.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();
            Session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    TouchControl.TurnOn(TouchControl.gameObject.transform, Session.Assembly.Camera, false, false, true, false);
                }
            };
            ImageTarget.TargetFound += () => { found = true; };
            ImageTarget.TargetLost += () => { found = false; };

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

        private void Update()
        {
            Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine +
                "Frame Source: " + ((Session.Assembly != null && Session.Assembly.FrameSource) ? Session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
                "Tracking Status: " + Session.TrackingStatus + Environment.NewLine +
                "Sparse Point Cloud Count: " + (sparse.LocalizedMap == null ? "-" : sparse.LocalizedMap.PointCloud.Count.ToString()) + Environment.NewLine +
                "Cube Location: " + (onSparse ? "On Sparse Spatial Map" : (Session.TrackingStatus.OnSome && Session.TrackingStatus != MotionTrackingStatus.NotTracking ? "Air" : "-")) + (found ? " + On " + ImageTarget.name : "") + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove to Sparse Spatial Map Point: One Finger Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                var touch = Input.touches[0];
                if (touch.phase == TouchPhase.Moved)
                {
                    var viewPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                    if (sparse && sparse.LocalizedMap)
                    {
                        var points = sparse.LocalizedMap.HitTest(viewPoint);
                        foreach (var point in points)
                        {
                            onSparse = true;
                            TouchControl.transform.position = sparse.LocalizedMap.transform.TransformPoint(point);
                            break;
                        }
                    }
                }
            }
        }
    }
}
