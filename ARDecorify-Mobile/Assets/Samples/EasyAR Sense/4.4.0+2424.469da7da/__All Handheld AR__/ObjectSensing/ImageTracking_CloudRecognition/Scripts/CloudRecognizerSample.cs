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
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ImageTracking_CloudRecognition
{
    public class CloudRecognizerSample : MonoBehaviour
    {
        public ARSession Session;
        public UnityEngine.UI.Text Status;
        public bool UseOfflineCache = true;
        public string OfflineCachePath;
        public Button BackButton;

        private CloudRecognizerFrameFilter cloudRecognizer;
        private ImageTrackerFrameFilter tracker;
        private List<GameObject> targetObjs = new List<GameObject>();
        private List<string> loadedCloudTargetUids = new List<string>();
        private int cachedTargetCount;
        private ResolveInfo resolveInfo;
        private float autoResolveRate = 1f;
        private bool isTracking;
        private bool resolveOn;

        private void Awake()
        {
            tracker = Session.GetComponentInChildren<ImageTrackerFrameFilter>();
            cloudRecognizer = Session.GetComponentInChildren<CloudRecognizerFrameFilter>();

            if (UseOfflineCache)
            {
                if (string.IsNullOrEmpty(OfflineCachePath))
                {
                    OfflineCachePath = Application.persistentDataPath + "/CloudRecognizerSample";
                }
                if (!Directory.Exists(OfflineCachePath))
                {
                    Directory.CreateDirectory(OfflineCachePath);
                }
                if (Directory.Exists(OfflineCachePath))
                {
                    var targetFiles = Directory.GetFiles(OfflineCachePath);
                    foreach (var file in targetFiles)
                    {
                        if (Path.GetExtension(file) == ".etd")
                        {
                            LoadOfflineTarget(file);
                        }
                    }
                }
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
        }

        private void Start()
        {
            StartAutoResolve(autoResolveRate);
        }

        private void Update()
        {
            Status.text =
            "Resolve: " + ((resolveInfo == null || resolveInfo.Index == 0 || isTracking) ? "OFF" : "ON") + Environment.NewLine +
            "\tIndex: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.Index).ToString()) + Environment.NewLine +
            "\tCostTime: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.CostTime).ToString() + "s") + Environment.NewLine +
            "\tTargetName: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.TargetName).ToString()) + Environment.NewLine +
            "\tCloudStatus: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.CloudStatus).ToString()) + Environment.NewLine +
            "\tErrorMessage: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.UnknownErrorMessage).ToString()) + Environment.NewLine +
            "CachedTargets: " + cachedTargetCount + Environment.NewLine +
            "LoadedTargets: " + loadedCloudTargetUids.Count;

            AutoResolve();
        }

        private void OnDestroy()
        {
            foreach (var obj in targetObjs)
            {
                Destroy(obj);
            }
        }

        public void ClearAll()
        {
            if (Directory.Exists(OfflineCachePath))
            {
                var targetFiles = Directory.GetFiles(OfflineCachePath);
                foreach (var file in targetFiles)
                {
                    if (Path.GetExtension(file) == ".etd")
                    {
                        File.Delete(file);
                    }
                }
            }
            foreach (var obj in targetObjs)
            {
                Destroy(obj);
            }
            targetObjs.Clear();
            loadedCloudTargetUids.Clear();
            cachedTargetCount = 0;
        }

        public void StartAutoResolve(float resolveRate)
        {
            if (Session != null && resolveInfo == null)
            {
                autoResolveRate = resolveRate;
                resolveInfo = new ResolveInfo();
                resolveOn = true;
            }
        }

        public void StopResolve()
        {
            if (Session != null)
            {
                resolveInfo = null;
                resolveOn = false;
            }
        }

        private void AutoResolve()
        {
            var time = Time.time;
            if (!resolveOn || isTracking || resolveInfo.Running || time - resolveInfo.ResolveTime < autoResolveRate)
            {
                return;
            }

            resolveInfo.Running = true;

            cloudRecognizer.Resolve((frame)=>
            {
                resolveInfo.ResolveTime = time;
            },(resultO, error) =>
            {
                if (resolveInfo == null)
                {
                    return;
                }
                resolveInfo.Running = false;
                resolveInfo.TargetName = "-";

                if (resultO.OnNone)
                {
                    resolveInfo.CostTime = 0;
                    resolveInfo.CloudStatus = CloudRecognizationStatus.UnknownError;
                    resolveInfo.UnknownErrorMessage = error;
                    return;
                }
                var result = resultO.Value;

                resolveInfo.Index++;
                resolveInfo.CostTime = Time.time - resolveInfo.ResolveTime;
                resolveInfo.CloudStatus = result.getStatus();
                resolveInfo.UnknownErrorMessage = result.getUnknownErrorMessage();

                var target = result.getTarget();
                if (target.OnSome)
                {
                    using (var targetValue = target.Value)
                    {
                        resolveInfo.TargetName = targetValue.name();

                        if (!loadedCloudTargetUids.Contains(targetValue.uid()))
                        {
                            LoadCloudTarget(targetValue.Clone());
                        }
                    }
                }
            });
        }

        private void LoadCloudTarget(ImageTarget target)
        {
            var uid = target.uid();
            loadedCloudTargetUids.Add(uid);
            var go = new GameObject(uid);
            targetObjs.Add(go);
            var targetController = go.AddComponent<ImageTargetController>();
            targetController.SourceType = ImageTargetController.DataSource.Target;
            targetController.TargetSource = target;
            LoadTargetIntoTracker(targetController);

            targetController.TargetLoad += (loadedTarget, result) =>
            {
                if (!result)
                {
                    Debug.LogErrorFormat("target {0} load failed", uid);
                    return;
                }
                AddCubeOnTarget(targetController);
            };

            if (UseOfflineCache && Directory.Exists(OfflineCachePath))
            {
                if (target.save(OfflineCachePath + "/" + target.uid() + ".etd"))
                {
                    cachedTargetCount++;
                }
            }
        }

        private void LoadOfflineTarget(string file)
        {
            var go = new GameObject(Path.GetFileNameWithoutExtension(file) + "-offline");
            targetObjs.Add(go);
            var targetController = go.AddComponent<ImageTargetController>();
            targetController.SourceType = ImageTargetController.DataSource.TargetDataFile;
            targetController.TargetDataFileSource.PathType = PathType.Absolute;
            targetController.TargetDataFileSource.Path = file;
            LoadTargetIntoTracker(targetController);

            targetController.TargetLoad += (loadedTarget, result) =>
            {
                if (!result)
                {
                    Debug.LogErrorFormat("target data {0} load failed", file);
                    return;
                }
                loadedCloudTargetUids.Add(loadedTarget.uid());
                cachedTargetCount++;
                AddCubeOnTarget(targetController);
            };
        }

        private void LoadTargetIntoTracker(ImageTargetController controller)
        {
            controller.Tracker = tracker;
            controller.TargetFound += () =>
            {
                isTracking = true;
            };
            controller.TargetLost += () =>
            {
                isTracking = false;
            };
        }

        private void AddCubeOnTarget(ImageTargetController controller)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Resources.Load("Materials/EasyAR") as Material;
            cube.transform.parent = controller.transform;
            cube.transform.localPosition = new Vector3(0, 0, -0.1f);
            cube.transform.eulerAngles = new Vector3(0, 0, 180);
            cube.transform.localScale = new Vector3(0.5f, 0.5f / controller.Target.aspectRatio(), 0.2f);
        }

        private class ResolveInfo
        {
            public int Index = 0;
            public bool Running = false;
            public float ResolveTime = 0;
            public float CostTime = 0;
            public string TargetName = "-";
            public Optional<string> UnknownErrorMessage;
            public CloudRecognizationStatus CloudStatus = CloudRecognizationStatus.UnknownError;
        }
    }
}
