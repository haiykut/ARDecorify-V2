//=============================================================================================================================
//
// Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
// EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
// and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//=============================================================================================================================

using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ImageTracking_TargetOnTheFly
{
    public class TargetOnTheFlySample : MonoBehaviour
    {
        public ImageTrackerFrameFilter Filter;
        public GameObject Cube;
        public Button BackButton;

        private bool creating;
        private string directory;
        private Dictionary<string, ImageTargetController> imageTargetDic = new Dictionary<string, ImageTargetController>();

        private void Start()
        {
            directory = Path.Combine(Application.persistentDataPath, "TargetOnTheFly");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            LoadLocalTargets();

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

        private void LoadLocalTargets()
        {
            Dictionary<string, string> imagefilesDic = GetImagesWithDir(directory);
            foreach (var obj in imagefilesDic.Where(obj => !imageTargetDic.ContainsKey(obj.Key)))
            {
                CreateImageTarget(obj.Key, obj.Value);
            }
        }

        private Dictionary<string, string> GetImagesWithDir(string path)
        {
            Dictionary<string, string> imagefilesDic = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file) == ".jpg" || Path.GetExtension(file) == ".bmp" || Path.GetExtension(file) == ".png")
                    imagefilesDic.Add(Path.GetFileName(file), file);
            }
            return imagefilesDic;
        }

        private IEnumerator TakePhotoCreateTarget()
        {
            creating = true;

            yield return new WaitForEndOfFrame();

            if (!creating)
            {
                yield break;
            }

            Texture2D photo = new Texture2D(Screen.width / 2, Screen.height / 2, TextureFormat.RGB24, false);
            photo.ReadPixels(new Rect(Screen.width / 5, Screen.height / 5, Screen.width * 3 / 5, Screen.height * 3 / 6), 0, 0, false);
            photo.Apply();

            byte[] data = photo.EncodeToJPG(80);
            Destroy(photo);

            string photoName = "photo" + DateTime.Now.Ticks + ".jpg";
            string photoPath = Path.Combine(directory, photoName);
            File.WriteAllBytes(photoPath, data);
            CreateImageTarget(photoName, photoPath);
            GUIPopup.EnqueueMessage("Image saved to: " + directory + Environment.NewLine +
                "Image target generated: " + photoName, 3);
        }

        private void CreateImageTarget(string targetName, string targetPath)
        {
            GameObject imageTarget = new GameObject(targetName);
            var controller = imageTarget.AddComponent<ImageTargetController>();
            controller.SourceType = ImageTargetController.DataSource.ImageFile;
            controller.ImageFileSource.PathType = PathType.Absolute;
            controller.ImageFileSource.Path = targetPath;
            controller.ImageFileSource.Name = targetName;
            controller.Tracker = Filter;
            imageTargetDic.Add(targetName, controller);
            var cube = Instantiate(Cube);
            cube.transform.parent = imageTarget.transform;
        }

        public void StartCreateTarget()
        {
            StartCoroutine(TakePhotoCreateTarget());
        }

        public void ClearAllTarget()
        {
            creating = false;

            foreach (var obj in imageTargetDic)
                Destroy(obj.Value.gameObject);
            imageTargetDic = new Dictionary<string, ImageTargetController>();

            Dictionary<string, string> imageFileDic = GetImagesWithDir(directory);
            foreach (var path in imageFileDic)
                File.Delete(path.Value);
        }
    }
}
