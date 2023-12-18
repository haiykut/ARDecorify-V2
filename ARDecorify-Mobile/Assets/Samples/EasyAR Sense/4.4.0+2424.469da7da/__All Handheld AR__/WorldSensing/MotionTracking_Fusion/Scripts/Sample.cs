//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    public class Sample : MonoBehaviour
    {
        public Text Status;
        public Button BackButton;
        public GameObject SessionPanel;
        public TouchController TouchControl;

        public ARSession Session;

        private ARSession session;
        private GameObject sessionObject;
        private ISample sample;
        private List<Button> sessionButtons;
        private List<ISample> sessionSamples;
        private bool checking;
        private string deviceModel = string.Empty;
        private static Optional<DateTime> trialCounter;

        private void Start()
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
            sessionButtons = SessionPanel.GetComponentsInChildren<Button>().ToList();
            sessionSamples = new List<ISample>();
            for (var i = 0; i < sessionButtons.Count; ++i)
            {
                sessionSamples.Add(null);
            }
            SessionPanel.SetActive(false);

            for (var i = 0; i < sessionButtons.Count; ++i)
            {
                var sampleIdx = i;
                sessionButtons[i].onClick.AddListener(() =>
                {
                    foreach (var button in sessionButtons.Where(s => s.interactable))
                    {
                        button.GetComponentInChildren<Text>().color = Color.black;
                    }
                    sessionButtons[sampleIdx].GetComponentInChildren<Text>().color = new Color32(0, 160, 233, 255);
                    foreach (var sample in sessionSamples.Where(s => s != null))
                    {
                        sample.Stop();
                    }
                    if (sessionObject) { DestroyImmediate(sessionObject); }
                    sessionObject = GameObject.Instantiate(Session.gameObject);
                    session = sessionObject.GetComponent<ARSession>();
                    session.StateChanged += (state) =>
                    {
                        if (state == ARSession.SessionState.Ready)
                        {
                            TouchControl.transform.SetParent(session.Origin.transform, false);
                            if (trialCounter.OnNone)
                            {
                                trialCounter = DateTime.Now;
                            }
                        }
                    };
                    sample = sessionSamples[sampleIdx];

                    // move selected frame source to transform order first before ARSession.Start
                    // you can also deactive other frame sources to achive the same effect
                    if (sample is ARFoundationSample)
                    {
                        session.GetComponentInChildren<ARFoundationFrameSource>().transform.SetSiblingIndex(0);
                    }
                    else if (sample is AREngineSample)
                    {
                        session.GetComponentInChildren<HuaweiAREngineFrameSource>().transform.SetSiblingIndex(0);
                    }
                    else if (sample is MotionTrackingSample)
                    {
                        session.GetComponentInChildren<MotionTrackerFrameSource>().transform.SetSiblingIndex(0);
                    }
                    else if (sample is EasyARARCoreARKitWrapperSample)
                    {
                        session.GetComponentInChildren<ARCoreFrameSource>().transform.SetSiblingIndex(0);
                        session.GetComponentInChildren<ARKitFrameSource>().transform.SetSiblingIndex(1);
                    }

                    session.gameObject.SetActive(true);
                    sample.Start(session, TouchControl);
                });
            }

            checking = true;
            StartCoroutine(CheckFrameSourceAvailability(() =>
            {
                checking = false;
                var started = false;
                for (var i = 0; i < sessionButtons.Count; ++i)
                {
                    if (!sessionButtons[i].interactable) { continue; }
                    sessionButtons[i].onClick.Invoke();
                    started = true;
                    break;
                }
                SessionPanel.SetActive(true);

                if (!started)
                {
                    var message = string.Empty;
                    foreach (var fs in Session.GetComponentsInChildren<FrameSource>())
                    {
                        if (fs is FramePlayer) { continue; }
                        message += $"{fs.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "")} ";
                    }
                    GUIPopup.EnqueueMessage($"Available frame source not found from candidates:\n{message}", 100);
                }
            }));
        }

        private void Update()
        {
            Status.text = $"Device Model: {SystemInfo.deviceModel} {deviceModel}" + Environment.NewLine;

            if (checking)
            {
                Status.text += "Please wait while checking all frame source availability...";
                return;
            }
            if (!session)
            {
                Status.text += "No Frame Source Availabile";
                return;
            }

            Status.text += "Frame Source: " + ((session.Assembly != null && session.Assembly.FrameSource) ? session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "").Replace("FrameSource", "") : "-") + Environment.NewLine +
                "Tracking Status: " + session.TrackingStatus + Environment.NewLine +
                "CenterMode: " + session.CenterMode + Environment.NewLine;

            if (session.State >= ARSession.SessionState.Ready && sample != null)
            {
                Status.text += sample.Info;
            }

            // avoid misunderstanding when using personal edition, not necessary in your own projects
            if (!string.IsNullOrEmpty(Engine.errorMessage()))
            {
                BackButton.GetComponent<Button>().interactable = false;
                trialCounter = DateTime.MinValue;
            }
            if (trialCounter.OnSome)
            {
                if (session.State >= ARSession.SessionState.Ready && (session.Assembly.FrameSource is ARFoundationFrameSource || session.Assembly.FrameSource is HuaweiAREngineFrameSource || session.Assembly.FrameSource is NrealFrameSource || trialCounter.Value == DateTime.MinValue))
                {
                    var time = Math.Max(0, (int)(trialCounter.Value - DateTime.Now).TotalSeconds + 100);
                    Status.text += $"\n\nEasyAR License for {session.Assembly.FrameSource.GetType()} will timeout for current process within {time} seconds. (Personal Edition Only)";
                }
            }
        }

        private void OnDestroy()
        {
            if (sessionObject) { Destroy(sessionObject); }
        }

        public void SwitchCenterMode()
        {
            if (!session || session.AvailableCenterMode.Count == 0) { return; }
            while (true)
            {
                session.CenterMode = (ARSession.ARCenterMode)(((int)session.CenterMode + 1) % Enum.GetValues(typeof(ARSession.ARCenterMode)).Length);
                if (session.AvailableCenterMode.Contains(session.CenterMode) && session.CenterMode != ARSession.ARCenterMode.Camera) { break; }
            }
        }

        private IEnumerator CheckFrameSourceAvailability(Action callback)
        {
            var idx = 0;
            foreach (var fs in Session.GetComponentsInChildren<FrameSource>())
            {
                if (!(fs is ARFoundationFrameSource || fs is HuaweiAREngineFrameSource || fs is MotionTrackerFrameSource || fs is ARCoreFrameSource || fs is ARKitFrameSource)) { continue; }
                if (fs is ARCoreFrameSource && Application.platform != RuntimePlatform.Android || fs is ARKitFrameSource && Application.platform != RuntimePlatform.IPhonePlayer) { continue; }

                var check = fs.CheckAvailability();
                if (check != null)
                {
                    yield return check;
                }
                var name = fs.GetType().ToString().Replace("FrameSource", "");
                if (fs is ARFoundationFrameSource || fs is HuaweiAREngineFrameSource)
                {
                    name = name.Replace("easyar.", "");
                }
                sessionButtons[idx].GetComponentInChildren<Text>().text = name;
                sessionButtons[idx].interactable = fs.IsAvailable.Value;

                if (fs is ARFoundationFrameSource)
                {
                    sessionSamples[idx] = GetComponent<ARFoundationSample>();
                }
                else if (fs is HuaweiAREngineFrameSource)
                {
                    sessionSamples[idx] = GetComponent<AREngineSample>();
                }
                else if (fs is MotionTrackerFrameSource)
                {
                    sessionSamples[idx] = GetComponent<MotionTrackingSample>();
                }
                else if (fs is ARCoreFrameSource || fs is ARKitFrameSource)
                {
                    sessionSamples[idx] = GetComponent<EasyARARCoreARKitWrapperSample>();
                }
                idx++;
            }
            for (var i = idx; i < sessionButtons.Count; ++i)
            {
                sessionButtons[i].interactable = false;
                sessionButtons[i].gameObject.SetActive(false);
            }
            callback?.Invoke();
        }

        public interface ISample
        {
            string Info { get; }
            void Start(ARSession session, TouchController touchControl);
            void Stop();
        }
    }
}
