//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using UnityEngine;
#if EASYAR_HWARENGINE_ENABLE && UNITY_ANDROID
using easyar;
using HuaweiARUnitySDK;
using System;
using System.Collections;
using HW = HuaweiARUnitySDK;
#endif

namespace Sample
{
    // NOTE: AR Engine do not handle update order, but everything should go after ARSession.Update.
    //       Use the undocumented DefaultExecutionOrder is a simple solution for package distribution, you can use your own way.
    [DefaultExecutionOrder(int.MinValue)]
    public class AREngineSampleSession : MonoBehaviour
    {
#if EASYAR_HWARENGINE_ENABLE && UNITY_ANDROID
        public ARConfigBase Config;

        private bool isSessionCreated;
        private bool? installed;
        private Vector2Int resolution;
        private ScreenOrientation orientation;

        private void Start()
        {
            if (Application.platform != RuntimePlatform.Android) { return; }
            StartCoroutine(CreateSession());
        }

        public void Update()
        {
            if (!isSessionCreated) { return; }
            SetDisplayGeometry();

            AsyncTask.Update();
            HW.ARSession.Update();
        }

        public void OnDestroy()
        {
            if (!isSessionCreated) { return; }
            HW.ARSession.Stop();
        }

        public void OnApplicationPause(bool isPaused)
        {
            if (!installed.HasValue && !isPaused)
            {
                CheckService();
            }
            if (!isSessionCreated) { return; }

            if (isPaused)
            {
                HW.ARSession.Pause();
            }
            else
            {
                HW.ARSession.Resume();
            }
        }

        private IEnumerator InstallRequired()
        {
            CheckService();
            while (!installed.HasValue)
            {
                yield return null;
            }
        }

        private IEnumerator CreateSession()
        {
            yield return InstallRequired();
            if (CheckService())
            {
                CameraDevice.requestPermissions(EasyARController.Scheduler, (Action<PermissionStatus, string>)((status, msg) =>
                {
                    if (status != PermissionStatus.Granted)
                    {
                        throw new UIPopupException("Camera permission not granted");
                    }

                    try
                    {
                        HW.ARSession.CreateSession();
                        HW.ARSession.Config(Config);
                        HW.ARSession.Resume();
                        HW.ARSession.SetCameraTextureNameAuto();
                        SetDisplayGeometry();
                        isSessionCreated = true;
                    }
#if false // you can turn on this code path if you change the class to public
                    catch (ARCameraPermissionDeniedException)
                    {
                        throw new UIPopupException("Camera permission not granted");
                    }
                    catch (ARUnavailableServiceApkTooOldException)
                    {
                        throw new UIPopupException("This AR Engine is too old, please update");
                    }
                    catch (ARUnSupportedConfigurationException)
                    {
                        throw new UIPopupException("This config is not supported on this device");
                    }
#endif
                    catch (ARUnavailableDeviceNotCompatibleException)
                    {
                        throw new UIPopupException("This device does not support AR Engine");
                    }
                    catch (ARUnavailableServiceNotInstalledException)
                    {
                        throw new UIPopupException("This app depend on AREngine.apk, please install it");
                    }
                    catch (SystemException e)
                    {
                        throw new UIPopupException($"{e.Message} Please restart the app");
                    }
                    catch (ApplicationException e)
                    {
                        throw new UIPopupException(e.ToString());
                    }
                }));
            }
        }

        private bool CheckService()
        {
            if (installed.HasValue) { return installed.Value; }
            if (Application.platform != RuntimePlatform.Android)
            {
                installed = false;
                return false;
            }

            installed = AREnginesApk.Instance.IsAREngineApkReady();
            return installed.Value;
        }

        private void SetDisplayGeometry()
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                resolution = new Vector2Int(Screen.width, Screen.height);
                HW.ARSession.SetDisplayGeometry(resolution.x, resolution.y);
            }
            if (orientation != Screen.orientation)
            {
                orientation = Screen.orientation;
                HW.ARSession.SetDisplayGeometry(resolution.x, resolution.y);
            }
        }
#endif
	}
}
