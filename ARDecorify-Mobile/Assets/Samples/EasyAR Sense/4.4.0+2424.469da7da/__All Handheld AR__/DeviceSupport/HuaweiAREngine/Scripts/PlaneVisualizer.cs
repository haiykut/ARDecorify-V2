//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

#if EASYAR_HWARENGINE_ENABLE && UNITY_ANDROID

using HuaweiARUnitySDK;
using System.Collections.Generic;
using UnityEngine;

namespace Sample
{
    public class PlaneVisualizer : MonoBehaviour
    {
        private Mesh mesh;
        private MeshRenderer meshRenderer;
        private ARPlane plane;
        private List<Vector3> meshVertices = new List<Vector3>();
        private List<Vector3> meshVertices3D = new List<Vector3>();
        private List<Vector2> meshVertices2D = new List<Vector2>();

        public void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Update()
        {
            if (plane == null)
            {
                return;
            }
            else if (plane.GetSubsumedBy() != null || plane.GetTrackingState() == ARTrackable.TrackingState.STOPPED)
            {
                Destroy(gameObject);
                return;
            }
            else if (plane.GetTrackingState() == ARTrackable.TrackingState.PAUSED)
            {
                meshRenderer.enabled = false;
                return;
            }

            meshRenderer.enabled = true;
            UpdateMeshIfNeeded();
        }

        public void SetPlane(ARPlane plane)
        {
            this.plane = plane;
        }

        private void UpdateMeshIfNeeded()
        {
            meshVertices3D.Clear();
            plane.GetPlanePolygon(meshVertices3D);

            if (Compare(meshVertices, meshVertices3D))
            {
                return;
            }

            var centerPose = plane.GetCenterPose();
            for (int i = 0; i < meshVertices3D.Count; i++)
            {
                meshVertices3D[i] = centerPose.rotation * meshVertices3D[i] + centerPose.position;
            }

            var planeNormal = centerPose.rotation * Vector3.up;
            meshRenderer.material.SetVector("_PlaneNormal", planeNormal);

            meshVertices.Clear();
            meshVertices.AddRange(meshVertices3D);

            meshVertices2D.Clear();
            plane.GetPlanePolygon(ref meshVertices2D);

            var tr = new Triangulator(meshVertices2D);

            mesh.Clear();
            mesh.SetVertices(meshVertices3D);
            mesh.SetIndices(tr.Triangulate(), MeshTopology.Triangles, 0);
        }

        private bool Compare(List<Vector3> firstList, List<Vector3> secondList)
        {
            if (firstList.Count != secondList.Count)
            {
                return false;
            }
            for (int i = 0; i < firstList.Count; i++)
            {
                if (firstList[i] != secondList[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
#endif
