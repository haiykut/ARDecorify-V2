//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace SpatialMap_SparseSpatialMap
{
    [RequireComponent(typeof(MeshRenderer), typeof(UnityEngine.Video.VideoPlayer))]
    public class VideoPlayerAgent : MonoBehaviour
    {
        public string VideoInStreamingAssets;

        private UnityEngine.Video.VideoPlayer player;
        private bool ready;
        private bool playable = true;

        public bool Playable
        {
            get { return playable; }
            set
            {
                playable = value;
                StatusChanged();
            }
        }

        private void OnEnable()
        {
            StatusChanged();
        }

        private void Start()
        {
            player = GetComponent<UnityEngine.Video.VideoPlayer>();
            player.source = VideoSource.Url;

            var path = Application.streamingAssetsPath + "/" + VideoInStreamingAssets;
            if (Application.platform == RuntimePlatform.Android)
            {
                path = Application.persistentDataPath + "/" + VideoInStreamingAssets;
            }

            // Note: Use the Unity VideoPlayer in your own way.
            // We use video file in StreamingAssets in the samples only to keep compatiblity.
            // Some versions of Unity will have strange behaviors if video in resources.
            if (Application.platform == RuntimePlatform.Android && !File.Exists(path) && !path.StartsWith("https://"))
            {
                StartCoroutine(FileUtil.LoadFile(VideoInStreamingAssets, PathType.StreamingAssets, (data) =>
                {
                    StartCoroutine(WriteFile(path, data));
                }));
            }
            else
            {
                player.url = path;
                ready = true;
                StatusChanged();
            }
        }

        private IEnumerator WriteFile(string path, byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                yield break;
            }

            bool finished = false;
            EasyARController.Instance.Worker.Run(() =>
            {
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, data);
                }
                finished = true;
            });

            while (!finished)
            {
                yield return 0;
            }
            player.url = path;
            ready = true;
            StatusChanged();
        }

        private void StatusChanged()
        {
            if (!ready)
            {
                return;
            }
            if (playable)
            {
                player.Play();
            }
            else
            {
                player.Pause();
            }
        }
    }
}
