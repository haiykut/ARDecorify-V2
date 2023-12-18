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

namespace MotionTracking
{
    public class UIController : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public Button UnlockPlaneButton;
        public GameObject Plane;
        public TouchController TouchControl;
        public Button BackButton;
        private string deviceModel = string.Empty;

        private void Awake()
        {
            Session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    if (Session.Assembly.FrameSource is MotionTrackerFrameSource)
                    {
                        TouchControl.TurnOn(TouchControl.gameObject.transform, Session.Assembly.Camera, false, false, true, true);
                    }
                    else
                    {
                        TouchControl.TurnOn(TouchControl.gameObject.transform, Session.Assembly.Camera, true, true, true, true);
                        UnlockPlaneButton.gameObject.SetActive(false);
                    }
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
            Status.text = "Try MotionTracking_Fusion Sample For advanced fusion usages with AR Foundation and Huawei AR Engine" + Environment.NewLine +
                Environment.NewLine +
                $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine +
                "Frame Source: " + ((Session.Assembly != null && Session.Assembly.FrameSource) ? Session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
                "Tracking Status: " + Session.TrackingStatus + Environment.NewLine +
                "CenterMode: " + Session.CenterMode + Environment.NewLine;

            if (Session.Assembly != null && Session.Assembly.FrameSource)
            {
                if (Session.Assembly.FrameSource is MotionTrackerFrameSource)
                {
                    Status.text += Environment.NewLine +
                    "Gesture Instruction" + Environment.NewLine +
                    "\tMove on Detected Plane: One Finger Move" + Environment.NewLine +
                    "\tRotate: Two Finger Horizontal Move" + Environment.NewLine +
                    "\tScale: Two Finger Pinch";
                }
                else
                {
                    Status.text += Environment.NewLine +
                    "Gesture Instruction" + Environment.NewLine +
                    "\tMove in View: One Finger Move" + Environment.NewLine +
                    "\tMove Near/Far: Two Finger Vertical Move" + Environment.NewLine +
                    "\tRotate: Two Finger Horizontal Move" + Environment.NewLine +
                    "\tScale: Two Finger Pinch";
                }
            }

            if (Session.Assembly != null && Session.Assembly.Camera && Session.Assembly.FrameSource && Session.Assembly.FrameSource is MotionTrackerFrameSource motionTracker)
            {
                if (!UnlockPlaneButton.interactable)
                {
                    var viewPoint = new Vector2(0.5f, 0.333f);
                    var points = motionTracker.HitTestAgainstHorizontalPlane(viewPoint);
                    if (points.Count > 0)
                    {
                        var viewportPoint = Session.Assembly.Camera.WorldToViewportPoint(Plane.transform.position);
                        if (!Plane.activeSelf || viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1 || Mathf.Abs(Plane.transform.position.y - points[0].y) > 0.15)
                        {
                            Plane.SetActive(true);
                            Plane.transform.position = points[0];
                            Plane.transform.localScale = Vector3.one * (Session.Assembly.Camera.transform.position - points[0]).magnitude;
                        }
                    }
                }

                if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    var touch = Input.touches[0];
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Ray ray = Session.Assembly.Camera.ScreenPointToRay(touch.position);
                        RaycastHit hitInfo;
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            TouchControl.transform.position = hitInfo.point;
                            UnlockPlaneButton.interactable = true;
                        }
                    }
                }
            }
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

        public void UnlockPlane()
        {
            UnlockPlaneButton.interactable = false;
        }
    }
}
