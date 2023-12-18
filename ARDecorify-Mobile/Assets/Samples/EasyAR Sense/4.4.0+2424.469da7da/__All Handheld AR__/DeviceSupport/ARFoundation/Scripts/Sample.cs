//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using UnityEngine;
using UnityEngine.UI;
using System;
#if EASYAR_ARFOUNDATION_ENABLE
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace Sample
{
    public class Sample : MonoBehaviour
    {
        public Button BackButton;
        public Text Status;
        public easyar.ARSession Session;
        public TouchController TouchControl;
        private int moveCubeTo;
        private static Optional<DateTime> trialCounter;

#if EASYAR_ARFOUNDATION_ENABLE
        private SparseSpatialMapWorkerFrameFilter sparse;
        private DenseSpatialMapBuilderFrameFilter dense;
        private ARRaycastManager raycastManager;
        private ARPlaneManager planeManager;
        private string cubeLocation;
        private string deviceModel = string.Empty;
#endif

        private void Awake()
        {
            Session.StateChanged += (state) =>
            {
                if (state == easyar.ARSession.SessionState.Ready)
                {
                    TouchControl.TurnOn(TouchControl.gameObject.transform, Session.Assembly.Camera, false, false, true, false);
                    if (trialCounter.OnNone)
                    {
                        trialCounter = DateTime.Now;
                    }
                }
            };

            var launcher = "AllSamplesLauncher";
            if (Application.CanStreamedLevelBeLoaded(launcher))
            {
                var button = BackButton.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(launcher);
#if EASYAR_ARFOUNDATION_ENABLE && UNITY_ANDROID
                    // avoid crash on iOS when multithreaded rendering is on, which is also observable on AR Foundation samples
                    LoaderUtility.Deinitialize();
#endif
                });
            }
            else
            {
                BackButton.gameObject.SetActive(false);
            }
        }

#if EASYAR_ARFOUNDATION_ENABLE
        void Start()
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
            planeManager = FindObjectOfType<ARPlaneManager>();
            sparse = Session.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();
            dense = Session.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>();
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

        void Update()
        {
            Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine +
                "Frame Source Type: " + ((!Session || Session.Assembly == null || !Session.Assembly.FrameSource) ? "-" : Session.Assembly.FrameSource.GetType().ToString()) + Environment.NewLine +
                "Tracking Status: " + UnityEngine.XR.ARFoundation.ARSession.state + Environment.NewLine +
                "CenterMode: " + Session.CenterMode + Environment.NewLine +
                "Sparse Point Cloud Count: " + (sparse.LocalizedMap == null ? "-" : sparse.LocalizedMap.PointCloud.Count.ToString()) + Environment.NewLine +
                "Dense Mesh Block Count: " + dense.MeshBlocks.Count + Environment.NewLine +
                "Plane Count: " + planeManager.trackables.count + Environment.NewLine +
                "Cube Location: " + (string.IsNullOrEmpty(cubeLocation) ? (UnityEngine.XR.ARFoundation.ARSession.state > ARSessionState.Ready ? "Air" : "-") : cubeLocation) + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove to " + (moveCubeTo == 0 ? "Plane" : (moveCubeTo == 1 ? "Dense Spatial Map Point" : "Sparse Spatial Map Mesh")) + ": One Finger Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";

            if (Session.Assembly != null && Session.Assembly.Camera && Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.touches[0].phase == TouchPhase.Moved)
            {
                var touch = Input.touches[0];

                if (moveCubeTo == 0)
                {
                    var hits = new List<ARRaycastHit>();
                    if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                    {
                        var hitPose = hits[0].pose;
                        cubeLocation = "AR Foundation Plane";
                        TouchControl.transform.position = hitPose.position;
                    }
                }
                else if (moveCubeTo == 1)
                {
                    Ray ray = Session.Assembly.Camera.ScreenPointToRay(touch.position);
                    var results = Physics.RaycastAll(ray);
                    if (results.Length > 0)
                    {
                        var list = results.ToList();
                        list.Sort((a, b) => a.distance.CompareTo(b.distance));
                        foreach (var hit in list)
                        {
                            if (hit.transform.GetComponent<DenseSpatialMapBlockController>())
                            {
                                cubeLocation = "EasyAR Dense Spatial Map";
                                TouchControl.transform.position = hit.point;
                                break;
                            }
                        }
                    }
                }
                else if (moveCubeTo == 2)
                {
                    var viewPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                    if (sparse && sparse.LocalizedMap)
                    {
                        var points = sparse.LocalizedMap.HitTest(viewPoint);
                        foreach (var point in points)
                        {
                            cubeLocation = "EasyAR Sparse Spatial Map";
                            TouchControl.transform.position = sparse.LocalizedMap.transform.TransformPoint(point);
                            break;
                        }
                    }
                }
            }

            // avoid misunderstanding when using personal edition, not necessary in your own projects
            if (!string.IsNullOrEmpty(Engine.errorMessage()))
            {
                BackButton.GetComponent<Button>().interactable = false;
                trialCounter = DateTime.MinValue;
            }
            if (trialCounter.OnSome)
            {
                if (Session.State >= easyar.ARSession.SessionState.Ready && (Session.Assembly.FrameSource is ARFoundationFrameSource || Session.Assembly.FrameSource is HuaweiAREngineFrameSource || Session.Assembly.FrameSource is NrealFrameSource || trialCounter.Value == DateTime.MinValue))
                {
                    var time = Math.Max(0, (int)(trialCounter.Value - DateTime.Now).TotalSeconds + 100);
                    Status.text += $"\n\nEasyAR License for {Session.Assembly.FrameSource.GetType()} will timeout for current process within {time} seconds. (Personal Edition Only)";
                }
            }
        }
#endif

        public void SwitchMoveLocation()
        {
            moveCubeTo++;
            moveCubeTo %= 3;
        }
    }
}
