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
using UnityEngine.EventSystems;

namespace Sample
{
    public class ARFoundationSample : MonoBehaviour, Sample.ISample
    {
        public List<GameObject> Objects;
        public GameObject ARSessionOrigin;
        private TouchController touchControl;
        private ARSession session;

        string Sample.ISample.Info
        {
            get
            {
                return Environment.NewLine +
#if EASYAR_ARFOUNDATION_ENABLE
                    "ARFoundation.ARSession Status: " + UnityEngine.XR.ARFoundation.ARSession.state + Environment.NewLine +
                    "Plane Detection: Enabled" + Environment.NewLine +
                    "Plane Count: " + planeManager.trackables.count + Environment.NewLine +
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
        }

        void Sample.ISample.Stop()
        {
            session = null;
            foreach (var o in Objects) { o.SetActive(false); }
        }

#if EASYAR_ARFOUNDATION_ENABLE
        private UnityEngine.XR.ARFoundation.ARRaycastManager raycastManager;
        private UnityEngine.XR.ARFoundation.ARPlaneManager planeManager;

        void Start()
        {
            raycastManager = ARSessionOrigin.GetComponent<UnityEngine.XR.ARFoundation.ARRaycastManager>();
            planeManager = ARSessionOrigin.GetComponent<UnityEngine.XR.ARFoundation.ARPlaneManager>();
        }

        void Update()
        {
            if (!session || session.State < ARSession.SessionState.Ready) { return; }

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.touches[0].phase == TouchPhase.Moved)
            {
                var touch = Input.touches[0];

                var hits = new List<UnityEngine.XR.ARFoundation.ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;
                    touchControl.transform.position = hitPose.position;
                }
            }
        }
#endif
    }
}
