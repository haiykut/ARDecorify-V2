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
using UnityEngine;
using UnityEngine.UI;

namespace AllSamplesLauncher
{
    public class MainScene : MonoBehaviour
    {
        public List<Button> Buttons = new List<Button>();
        public GameObject RightRoot;

        private static SceneRecorder sceneRecorder;

        private void Start()
        {
            if (sceneRecorder == null)
            {
                var go = new GameObject("SceneRecorder");
                DontDestroyOnLoad(go);
                sceneRecorder = go.AddComponent<SceneRecorder>();
                sceneRecorder.Record(Buttons[0], this);
            }
            else
            {
                sceneRecorder.Recover(this);
            }

            foreach (var item in Buttons)
            {
                item.onClick.AddListener(() =>
                {
                    sceneRecorder.Record(item, this);
                });
            }

            for (var i = 0; i < RightRoot.transform.childCount; ++i)
            {
                int count = 0;
                var catgory = RightRoot.transform.GetChild(i).gameObject;
                for (var j = 0; j < catgory.transform.childCount; ++j)
                {
                    var sample = catgory.transform.GetChild(j).gameObject;
                    if (Application.CanStreamedLevelBeLoaded(sample.name))
                    {
                        var button = sample.GetComponent<Button>();
                        button.onClick.AddListener(() =>
                        {
#if EASYAR_ARFOUNDATION_ENABLE
                            if (sample.name.Contains("ARFoundation") || sample.name.Contains("Fusion"))
                            {
                                UnityEngine.XR.ARFoundation.LoaderUtility.Initialize();
                            }
#endif
                            UnityEngine.SceneManagement.SceneManager.LoadScene(sample.name);
                        });
                    }
                    else
                    {
                        sample.SetActive(false);
                        count++;
                    }
                }
                if (count == catgory.transform.childCount)
                {
                    Buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
