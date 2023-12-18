//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using Common;
using easyar;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapBuilding_Sparse_Dense
{
    public class MapBuilding_Sparse_DenseSample : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public TouchController TouchControl;
        public Button BackButton;

        private SparseSpatialMapWorkerFrameFilter sparse;
        private DenseSpatialMapBuilderFrameFilter dense;
        private bool onSparse;
        private bool onDense;
        private bool moveOnSparse = true;
        private string deviceModel = string.Empty;

        private void Awake()
        {
            sparse = Session.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();
            dense = Session.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>();
            Session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    TouchControl.TurnOn(TouchControl.gameObject.transform, Session.Assembly.Camera, false, false, true, false);
                }
            };

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
                "CenterMode: " + Session.CenterMode + Environment.NewLine +
                "Sparse Point Cloud Count: " + (sparse.LocalizedMap == null ? "-" : sparse.LocalizedMap.PointCloud.Count.ToString()) + Environment.NewLine +
                "Dense Mesh Block Count: " + dense.MeshBlocks.Count + Environment.NewLine +
                "Cube Location: " + (onSparse ? "On Sparse Spatial Map" : (onDense ? "On Dense Spatial Map" : (Session.TrackingStatus.OnSome && Session.TrackingStatus != MotionTrackingStatus.NotTracking ? "Air" : "-"))) + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove to " + (moveOnSparse ? "Sparse Spatial Map Point" : "Dense Spatial Map Mesh") + ": One Finger Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";

            if (Session.Assembly != null && Session.Assembly.Camera && Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.touches[0].phase == TouchPhase.Moved)
            {
                var touch = Input.touches[0];
                if (moveOnSparse)
                {
                    var viewPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                    if (sparse && sparse.LocalizedMap)
                    {
                        var points = sparse.LocalizedMap.HitTest(viewPoint);
                        foreach (var point in points)
                        {
                            onSparse = true;
                            onDense = false;
                            TouchControl.transform.position = sparse.LocalizedMap.transform.TransformPoint(point);
                            break;
                        }
                    }
                }
                else
                {
                    Ray ray = Session.Assembly.Camera.ScreenPointToRay(Input.touches[0].position);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        onDense = true;
                        onSparse = false;
                        TouchControl.transform.position = hitInfo.point;
                    };
                }
            }
        }

        public void SwitchMoveLocation()
        {
            moveOnSparse = !moveOnSparse;
        }

        public void SwitchCenterMode()
        {
            if (Session.AvailableCenterMode.Count == 0) { return; }
            while (true)
            {
                Session.CenterMode = (ARSession.ARCenterMode)(((int)Session.CenterMode + 1) % Enum.GetValues(typeof(ARSession.ARCenterMode)).Length);
                if (Session.AvailableCenterMode.Contains(Session.CenterMode) && Session.CenterMode != ARSession.ARCenterMode.Camera) { break; }
            }
        }
    }
}
