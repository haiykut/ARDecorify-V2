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

namespace SurfaceTracking
{
    public class UIController : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public TouchController TouchControl;
        public Button BackButton;

        private SurfaceTrackerFrameFilter tracker;
        private string deviceModel = string.Empty;

        private void Awake()
        {
            tracker = Session.GetComponentInChildren<SurfaceTrackerFrameFilter>();
            Session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    TouchControl.TurnOn(TouchControl.transform, Session.Assembly.Camera, false, false, true, true);
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
                "CenterMode: " + Session.CenterMode + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove on Surface: One Finger Move" + Environment.NewLine +
                "\tRotate: Two Finger Horizontal Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                var touch = Input.touches[0];
                if (touch.phase == TouchPhase.Moved)
                {
                    var viewPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                    var coord = Session.ImageCoordinatesFromScreenCoordinates(viewPoint);
                    if (tracker && tracker.Tracker != null && coord.OnSome)
                    {
                        tracker.Tracker.alignTargetToCameraImagePoint(coord.Value.ToEasyARVector());
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
                if (Session.AvailableCenterMode.Contains(Session.CenterMode)) { break; }
            }
        }
    }
}
