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
using UnityEngine.EventSystems;

namespace Sample
{
    public class AREngineSample : MonoBehaviour, Sample.ISample
    {
        public List<GameObject> Objects;
        public GameObject PlanePrefab;
        public AREngineSampleSession SampleSession;
        private TouchController touchControl;
        private ARSession session;

        string Sample.ISample.Info
        {
            get
            {
                return Environment.NewLine +
#if EASYAR_HWARENGINE_ENABLE && UNITY_ANDROID
                    "HuaweiARUnitySDK.ARFrame Status: " + HuaweiARUnitySDK.ARFrame.GetTrackingState() + Environment.NewLine +
                    "Plane Detection: Enabled" + Environment.NewLine +
                    "Plane Count: " + planeAll.Count + Environment.NewLine +
#endif
                    Environment.NewLine +
                    "Gesture Instruction" + Environment.NewLine +
                    "\tMove on Detected Plane: One Finger Move" + Environment.NewLine +
                    "\tScale: Two Finger Pinch";
            }
        }

        void Sample.ISample.Start(ARSession session, TouchController touchControl)
        {
            this.touchControl = touchControl;
            this.session = session;
            session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    touchControl.TurnOn(touchControl.gameObject.transform, session.Assembly.Camera, false, false, true, false);
                }
            };
            foreach (var o in Objects) { o.SetActive(true); }
#if EASYAR_HWARENGINE_ENABLE && UNITY_ANDROID
            SampleSession.OnApplicationPause(false);
#endif
        }

        void Sample.ISample.Stop()
        {
            session = null;
            StopAllCoroutines();
            if (SampleSession.gameObject.activeSelf)
            {
#if EASYAR_HWARENGINE_ENABLE && UNITY_ANDROID
                SampleSession.OnApplicationPause(true);
#endif
            }
            foreach (var o in Objects) { o.SetActive(false); }
        }

#if EASYAR_HWARENGINE_ENABLE && UNITY_ANDROID
        private HuaweiARUnitySDK.ARAnchor anchor;
        private List<AREnginePlaneVisualizer> planeAll = new List<AREnginePlaneVisualizer>();

        void Update()
        {
            if (!session || session.State < ARSession.SessionState.Ready) { return; }

            if (HuaweiARUnitySDK.ARFrame.GetTrackingState() == HuaweiARUnitySDK.ARTrackable.TrackingState.STOPPED) { return; }

            var planes = new List<HuaweiARUnitySDK.ARPlane>();
            HuaweiARUnitySDK.ARFrame.GetTrackables(planes, HuaweiARUnitySDK.ARTrackableQueryFilter.NEW);
            foreach (var plane in planes)
            {
                GameObject planeObject = Instantiate(PlanePrefab, Vector3.zero, Quaternion.identity, transform);
                var p = planeObject.AddComponent<AREnginePlaneVisualizer>();
                p.SetPlane(plane);
                planeAll.Add(p);
            }
            planeAll = planeAll.Where(p => p).ToList();

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.touches[0].phase == TouchPhase.Moved)
            {
                var touch = Input.touches[0];

                HuaweiARUnitySDK.ARHitResult hitResult = null;
                foreach (var result in HuaweiARUnitySDK.ARFrame.HitTest(touch))
                {
                    if (result.GetTrackable() is HuaweiARUnitySDK.ARPlane plane && plane.IsPoseInPolygon(result.HitPose))
                    {
                        hitResult = result;
                        break;
                    }
                }
                if (hitResult != null)
                {
                    if (anchor != null) { anchor.Detach(); }
                    anchor = hitResult.CreateAnchor();
                    StartCoroutine(UpdateAnchor(anchor));
                }
            }
        }

        private IEnumerator UpdateAnchor(HuaweiARUnitySDK.ARAnchor anchor)
        {
            while (true)
            {
                switch (anchor.GetTrackingState())
                {
                    case HuaweiARUnitySDK.ARTrackable.TrackingState.TRACKING:
                        Pose p = anchor.GetPose();
                        touchControl.transform.position = p.position;
                        touchControl.transform.rotation = p.rotation;
                        break;
                    case HuaweiARUnitySDK.ARTrackable.TrackingState.PAUSED:
                    case HuaweiARUnitySDK.ARTrackable.TrackingState.STOPPED:
                    default:
                        yield break;
                }
                yield return null;
            }
        }
#endif
    }
}
