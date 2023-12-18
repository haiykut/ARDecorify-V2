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
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    public class Sample : MonoBehaviour
    {
        public Button BackButton;
        public Text Status;
        public ARSession Session;
        public List<GameObject> ObjectsARFoundation;
        public Camera MainCamera;

        private string deviceModel = string.Empty;
        private ImageTrackerFrameFilter tracker;
        private static Optional<DateTime> trialCounter;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void ImportSampleStreamingAssets()
        {
            FileUtil.ImportSampleStreamingAssets();
        }
#endif

        private void Awake()
        {
            var launcher = "AllSamplesLauncher";
            if (Application.CanStreamedLevelBeLoaded(launcher))
            {
                var button = BackButton.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(launcher);
#if EASYAR_ARFOUNDATION_ENABLE && UNITY_ANDROID
                    // avoid crash on iOS when multithreaded rendering is on, which is also observable on AR Foundation samples
                    UnityEngine.XR.ARFoundation.LoaderUtility.Deinitialize();
#endif
                });
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
            tracker = Session.GetComponentInChildren<ImageTrackerFrameFilter>();

            Session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    if (Session.Assembly.FrameSource is ARFoundationFrameSource)
                    {
                        MainCamera.gameObject.SetActive(false);
                        foreach (var o in ObjectsARFoundation)
                        {
                            o.SetActive(true);
                        }
                    }
                    if (trialCounter.OnNone)
                    {
                        trialCounter = DateTime.Now;
                    }
                }
            };
        }

        private void Update()
        {
            Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine +
                "Frame Source: " + ((Session.Assembly != null && Session.Assembly.FrameSource) ? Session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
                "Tracking Status: " + Session.TrackingStatus + Environment.NewLine;

            if (Session.State == ARSession.SessionState.Assembling)
            {
                Status.text += "Please wait while checking all frame source availability...";
            }

            if (Session.State >= ARSession.SessionState.Ready)
            {
                if (Session.Assembly.FrameSource is CameraDeviceFrameSource)
                {
                    Status.text += Environment.NewLine +
                        "Motion tracking capability not available on this device." + Environment.NewLine +
                        "Fallback to image tracking." + Environment.NewLine;
                }
                else
                {
                    Status.text += Environment.NewLine +
                        "Motion Fusion: " + tracker.EnableMotionFusion + Environment.NewLine +
                        (tracker.EnableMotionFusion ? "Image must NOT move in real world." : "Image is free to move in real world.") + Environment.NewLine +
                    Environment.NewLine +
                    "    Image target scale must be set to physical image width." + Environment.NewLine +
                    "    Scale is preset to match long edge of A4 paper." + Environment.NewLine +
                    "    Suggest to print out images for test.";

                    if (Session.Assembly.FrameSource is ARFoundationFrameSource && !tracker.EnableMotionFusion)
                    {
                        Status.text += Environment.NewLine + Environment.NewLine +
                            "Warning: not suggest to turn off motion fusion when using AR Foundation. There will be lags when image move.";
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
                if (Session.State >= ARSession.SessionState.Ready && (Session.Assembly.FrameSource is ARFoundationFrameSource || Session.Assembly.FrameSource is HuaweiAREngineFrameSource || Session.Assembly.FrameSource is NrealFrameSource || trialCounter.Value == DateTime.MinValue))
                {
                    var time = Math.Max(0, (int)(trialCounter.Value - DateTime.Now).TotalSeconds + 100);
                    Status.text += $"\n\nEasyAR License for {Session.Assembly.FrameSource.GetType()} will timeout for current process within {time} seconds. (Personal Edition Only)";
                }
            }
        }
    }
}
