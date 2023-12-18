//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FrameRecording
{
    public class Sample : MonoBehaviour
    {
        public GameObject EasyARSession;
        public Text Status;
        public Button BackButton;

        private ImageTargetController controllerNamecard;
        private ImageTargetController controllerIdback;
        private GameObject easyarObject;
        private FramePlayer player;
        private FrameRecorder recorder;
        private string filePath;
        private string deviceModel = string.Empty;
        private ARSession session;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void ImportSampleStreamingAssets()
        {
            FileUtil.ImportSampleStreamingAssets();
        }
#endif

        private void Awake()
        {
            var folder = Application.persistentDataPath + "/FrameRecording/";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            filePath = folder + "recording.eif";

            controllerNamecard = GameObject.Find("ImageTarget-namecard").GetComponent<ImageTargetController>();
            controllerIdback = GameObject.Find("ImageTarget-idback").GetComponent<ImageTargetController>();
            AddTargetControllerEvents(controllerNamecard);
            AddTargetControllerEvents(controllerIdback);

            CreateRecorder();

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
            Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine;
            if (session)
            {
                Status.text +=
                    "Frame Source: " + ((session.Assembly != null && session.Assembly.FrameSource) ? session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
                    "Tracking Status: " + session.TrackingStatus + Environment.NewLine +
                    "CenterMode: " + session.CenterMode + Environment.NewLine + Environment.NewLine;
            }

            if (player)
            {
                Status.text += "Playback Mode";
                Status.text += Environment.NewLine +
                    "Playback from: " + filePath + Environment.NewLine +
                    "Playback: " + player.Time + " / " + player.Length + (player.IsCompleted ? " -- completed" : "");
            }
            else
            {
                Status.text += "Recoding Mode";
                if (recorder.enabled)
                {
                    Status.text += Environment.NewLine +
                        "Recoding to: " + filePath;
                }
            }
        }

        private void OnDestroy()
        {
            DestroySession();
        }

        public void CreateRecorder()
        {
            DestroySession();
            easyarObject = Instantiate(EasyARSession);
            var picker = easyarObject.GetComponentInChildren<ARComponentPicker>();
            picker.FramePlayer = ARComponentPicker.SingleSelection.Disable;
            picker.FrameRecorder = ARComponentPicker.SingleSelection.FirstAvailableActiveChild;
            easyarObject.SetActive(true);
            var imageTracker = easyarObject.GetComponentInChildren<ImageTrackerFrameFilter>();
            controllerNamecard.Tracker = null;
            controllerIdback.Tracker = null;
            controllerNamecard.Tracker = imageTracker;
            controllerIdback.Tracker = imageTracker;

            player = null;
            recorder = easyarObject.GetComponentInChildren<FrameRecorder>();
            recorder.FilePathType = WritablePathType.Absolute;
            recorder.FilePath = filePath;
            recorder.enabled = false;

            session = easyarObject.GetComponentInChildren<ARSession>();
            session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    // at this time point, tracking results (if there are any) are not processed yet
                    if (!(session.Assembly.FrameSource is CameraDeviceFrameSource))
                    {
                        controllerNamecard.ActiveControl = TargetController.ActiveControlStrategy.HideBeforeFirstFound;
                        controllerIdback.ActiveControl = TargetController.ActiveControlStrategy.HideBeforeFirstFound;
                    }
                }
            };
        }

        public void CreatePlayer()
        {
            DestroySession();
            easyarObject = Instantiate(EasyARSession);
            var picker = easyarObject.GetComponentInChildren<ARComponentPicker>();
            picker.FramePlayer = ARComponentPicker.SingleSelection.FirstAvailableActiveChild;
            picker.FrameRecorder = ARComponentPicker.SingleSelection.Disable;
            easyarObject.SetActive(true);
            var imageTracker = easyarObject.GetComponentInChildren<ImageTrackerFrameFilter>();
            controllerNamecard.Tracker = null;
            controllerIdback.Tracker = null;
            controllerNamecard.Tracker = imageTracker;
            controllerIdback.Tracker = imageTracker;

            recorder = null;
            player = easyarObject.GetComponentInChildren<FramePlayer>();
            player.FilePathType = WritablePathType.Absolute;
            player.FilePath = filePath;

            session = easyarObject.GetComponentInChildren<ARSession>();
            session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    // at this time point, tracking results (if there are any) are not processed yet
                    if (!(session.Assembly.FrameSource is CameraDeviceFrameSource))
                    {
                        controllerNamecard.ActiveControl = TargetController.ActiveControlStrategy.HideBeforeFirstFound;
                        controllerIdback.ActiveControl = TargetController.ActiveControlStrategy.HideBeforeFirstFound;
                    }
                }
            };
        }

        public void Record(bool val)
        {
            recorder.enabled = val;
        }

        public void Playback(bool val)
        {
            if (val)
            {
                player.Play();
            }
            else
            {
                player.Stop();
            }
        }

        private void DestroySession()
        {
            if (easyarObject) { DestroyImmediate(easyarObject); }
        }

        private void AddTargetControllerEvents(ImageTargetController controller)
        {
            if (!controller)
            {
                return;
            }

            controller.TargetFound += () =>
            {
                Debug.LogFormat("Found target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), controller.Target.name());
            };
            controller.TargetLost += () =>
            {
                Debug.LogFormat("Lost target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), controller.Target.name());
            };
            controller.TargetLoad += (Target target, bool status) =>
            {
                Debug.LogFormat("Load target {{id = {0}, name = {1}, size = {2}}} into {3} => {4}", target.runtimeID(), target.name(), controller.Size, controller.Tracker.name, status);
            };
            controller.TargetUnload += (Target target, bool status) =>
            {
                Debug.LogFormat("Unload target {{id = {0}, name = {1}}} => {2}", target.runtimeID(), target.name(), status);
            };
        }
    }
}
