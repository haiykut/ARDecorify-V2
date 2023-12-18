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

namespace Sample
{
    public class EasyARARCoreARKitWrapperSample : MonoBehaviour, Sample.ISample
    {
        public List<GameObject> Objects;

        string Sample.ISample.Info
        {
            get
            {
                return Environment.NewLine +
                    "Plane Detection: Not support" + Environment.NewLine +
                    Environment.NewLine +
                    "Gesture Instruction" + Environment.NewLine +
                    "\tMove in View: One Finger Move" + Environment.NewLine +
                    "\tMove Near/Far: Two Finger Vertical Move" + Environment.NewLine +
                    "\tRotate: Two Finger Horizontal Move" + Environment.NewLine +
                    "\tScale: Two Finger Pinch";
            }
        }

        void Sample.ISample.Start(ARSession session, TouchController touchControl)
        {
            session.StateChanged += (state) =>
            {
                if (state == ARSession.SessionState.Ready)
                {
                    touchControl.TurnOn(touchControl.gameObject.transform, session.Assembly.Camera, true, true, true, true);
                }
            };
            foreach (var o in Objects) { o.SetActive(true); }
        }

        void Sample.ISample.Stop()
        {
            foreach (var o in Objects) { o.SetActive(false); }
        }
    }
}
