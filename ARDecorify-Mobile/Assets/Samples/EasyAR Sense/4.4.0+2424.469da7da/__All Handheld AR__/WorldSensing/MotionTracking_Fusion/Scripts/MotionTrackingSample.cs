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
using UnityEngine.UI;

namespace Sample
{
    public class MotionTrackingSample : MonoBehaviour, Sample.ISample
    {
        public List<GameObject> Objects;
        public GameObject Plane;
        public Button UnlockPlaneButton;
        private TouchController touchControl;
        private ARSession session;

        string Sample.ISample.Info
        {
            get
            {
                return Environment.NewLine +
                    "Plane Detection: Enabled" + Environment.NewLine +
                    Environment.NewLine +
                    "Gesture Instruction" + Environment.NewLine +
                    "\tMove on Detected Plane: One Finger Move" + Environment.NewLine +
                    "\tRotate: Two Finger Horizontal Move" + Environment.NewLine +
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
                    touchControl.TurnOn(touchControl.gameObject.transform, session.Assembly.Camera, false, false, true, true);
                }
            };
            UnlockPlaneButton.gameObject.SetActive(true);
            foreach (var o in Objects) { o.SetActive(true); }
        }

        void Sample.ISample.Stop()
        {
            session = null;
            Plane.SetActive(false);
            UnlockPlaneButton.gameObject.SetActive(false);
            foreach (var o in Objects) { o.SetActive(false); }
        }

        void Update()
        {
            if (!session || session.State < ARSession.SessionState.Ready) { return; }

            var motionTracker = session.Assembly.FrameSource as MotionTrackerFrameSource;
            if (!UnlockPlaneButton.interactable)
            {
                var viewPoint = new Vector2(0.5f, 0.333f);
                var points = motionTracker.HitTestAgainstHorizontalPlane(viewPoint);
                if (points.Count > 0)
                {
                    var viewportPoint = session.Assembly.Camera.WorldToViewportPoint(Plane.transform.position);
                    if (!Plane.activeSelf || viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1 || Mathf.Abs(Plane.transform.position.y - points[0].y) > 0.15)
                    {
                        Plane.SetActive(true);
                        Plane.transform.position = points[0];
                        Plane.transform.localScale = Vector3.one * (session.Assembly.Camera.transform.position - points[0]).magnitude;
                    }
                }
            }

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                var touch = Input.touches[0];
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Ray ray = session.Assembly.Camera.ScreenPointToRay(touch.position);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        touchControl.transform.position = hitInfo.point;
                        UnlockPlaneButton.interactable = true;
                    }
                }
            }
        }
    }
}
