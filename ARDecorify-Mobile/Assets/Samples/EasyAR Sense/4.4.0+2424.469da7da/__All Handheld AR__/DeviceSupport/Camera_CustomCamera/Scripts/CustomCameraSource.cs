//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
using AOT;
using System.Runtime.InteropServices;
#endif
using System;
using UnityEngine;
using UnityEngine.UI;

namespace easyar
{
    public class CustomCameraSource : FrameSource
    {
        private bool willOpen = false;

        public override Optional<bool> IsAvailable { get => Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer; }

        protected override void OnEnable()
        {
            base.OnEnable();
#if !UNITY_EDITOR && UNITY_ANDROID
            if (externalCamera != null)
                externalCamera.Call<bool>("start", cameraCallback);
#elif !UNITY_EDITOR && UNITY_IOS
            if (externalCamera != null)
                externalCamera.start(cameraCallback);
#endif
        }

        protected override void OnDisable()
        {
            base.OnDisable();
#if !UNITY_EDITOR && UNITY_ANDROID
            if (externalCamera != null)
                externalCamera.Call<bool>("stop");
#elif !UNITY_EDITOR && UNITY_IOS
            if (externalCamera != null)
                externalCamera.stop();
#endif
        }

        protected virtual void OnDestroy()
        {
            Close();
        }

        public override void OnAssemble(ARSession session)
        {
            base.OnAssemble(session);
            Open();
        }

        public void Open()
        {
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                throw new UIPopupException(typeof(CustomCameraSource) + " not available under " + Application.platform);
            }
            willOpen = true;
            CameraDevice.requestPermissions(EasyARController.Scheduler, (Action<PermissionStatus, string>)((status, msg) =>
            {
                if (!willOpen)
                {
                    return;
                }
#if !UNITY_EDITOR && UNITY_ANDROID
                 externalCamera = new AndroidJavaObject("com.example.externalcamera.ExternalCameraSample");
                 externalCamera.Call<bool>("open");

                 cameraCallback = new CameraCallback(dataWrapper =>
                 {
                     if (sink == null)
                     {
                         return;
                     }

                     var byteArray = dataWrapper.Get<AndroidJavaObject>("Buffer");
                     var jniByteArray = byteArray.GetRawObject();

                     var buffer = JniUtility.wrapByteArray(jniByteArray, true, () => { byteArray.Dispose(); });
                     var format = (PixelFormat)externalCamera.Call<int>("getPixelFormat");
                     var param = dataWrapper.Get<AndroidJavaObject>("camParams");
                     int orientation = param.Call<int>("getOrientation");
                     int cameraType = param.Call<int>("getCameraType");
                     double timestamp = param.Call<long>("getTimestamp") * 1e-9;
                     var imageWidth = param.Call<int>("getWidth");
                     var imageHeight = param.Call<int>("getHeight");
                     var imageSize = new Vector2(imageWidth, imageHeight);
                     HandleSink(buffer, format, imageSize, orientation, cameraType, timestamp);
                 });

#elif !UNITY_EDITOR && UNITY_IOS
                externalCamera = new ExternalCamera();
                externalCamera.open();

                cameraCallback = (data, len) =>
                {
                    var buffer = Buffer.wrap(data, len, () => { });
                    var format = PixelFormat.YUV_NV12;
                    var orientation = externalCamera.getOrientation();
                    var cameraType = externalCamera.getCameraType();
                    var imageWidth = externalCamera.getImageWidth();
                    var imageHeight = externalCamera.getImageHeight();
                    var timestamp = externalCamera.getTimestamp();
                    var imageSize = new Vector2(imageWidth, imageHeight);
                    HandleSink(buffer, format, imageSize, orientation, cameraType, timestamp);
                };
#endif
                if (enabled)
                {
                    OnEnable();
                }
            }));
        }

        public void Close()
        {
            willOpen = false;
            OnDisable();
#if !UNITY_STANDALONE && !UNITY_EDITOR
            if (externalCamera != null)
                externalCamera.Dispose();
#endif
        }

        private void HandleSink(Buffer imageBuffer, PixelFormat format, Vector2 imageSize, int orientation, int cameraType, double timestamp)
        {
            using (var cameraParams = CameraParameters.createWithDefaultIntrinsics(new Vec2I((int)imageSize.x, (int)imageSize.y), (CameraDeviceType)cameraType, orientation))
            using (var image = new Image(imageBuffer, format, (int)imageSize.x, (int)imageSize.y))
            using (var frame = InputFrame.createWithImageAndCameraParametersAndTemporal(image, cameraParams, timestamp))
            {
                if (sink != null)
                    sink.handle(frame);
            }
            imageBuffer.Dispose();
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        private AndroidJavaObject externalCamera;
        private CameraCallback cameraCallback;

        private class CameraCallback : AndroidJavaProxy
        {
            private Action<AndroidJavaObject> onPreviewFrameCallback;

            public CameraCallback(Action<AndroidJavaObject> onPreviewFrameCallback) : base("com.example.externalcamera.ExternalCameraSample$Callback")
            {
                this.onPreviewFrameCallback = onPreviewFrameCallback;
            }

            public void onPreviewFrame(AndroidJavaObject dataWrapper)
            {
                onPreviewFrameCallback(dataWrapper);
            }
        }
#elif !UNITY_EDITOR && UNITY_IOS
        private ExternalCamera externalCamera;
        private Action<IntPtr, int> cameraCallback;

        private static class ExternalCameraDetail
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct FunctorOfVoidFromRawPointerOfVoidAndInt
            {
                [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void FunctionDelegate(IntPtr state, IntPtr arg0, int arg1);
                [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void DestroyDelegate(IntPtr _state);

                public IntPtr _state;
                public FunctionDelegate _func;
                public DestroyDelegate _destroy;
            }
            [MonoPInvokeCallback(typeof(FunctorOfVoidFromRawPointerOfVoidAndInt.FunctionDelegate))]
            public static void FunctorOfVoidFromRawPointerOfVoidAndInt_func(IntPtr state, IntPtr arg0, int arg1)
            {
                var f = (Action<IntPtr, int>)((GCHandle)(state)).Target;
                f(arg0, arg1);
            }
            [MonoPInvokeCallback(typeof(FunctorOfVoidFromRawPointerOfVoidAndInt.DestroyDelegate))]
            public static void FunctorOfVoidFromRawPointerOfVoidAndInt_destroy(IntPtr _state)
            {
                ((GCHandle)(_state)).Free();
            }
            public static FunctorOfVoidFromRawPointerOfVoidAndInt FunctorOfVoidFromRawPointerOfVoidAndInt_to_c(Action<IntPtr, int> f)
            {
                if (f == null)
                {
                    return new FunctorOfVoidFromRawPointerOfVoidAndInt { _state = IntPtr.Zero, _func = null, _destroy = null };
                }
                var s = GCHandle.Alloc(f, GCHandleType.Normal);
                return new FunctorOfVoidFromRawPointerOfVoidAndInt { _state = (IntPtr)(s), _func = FunctorOfVoidFromRawPointerOfVoidAndInt_func, _destroy = FunctorOfVoidFromRawPointerOfVoidAndInt_destroy };
            }

            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool openCamera(out IntPtr This);
            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern void closeCamera(IntPtr This);
            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern int getImageWidth(IntPtr This);
            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern int getImageHeight(IntPtr This);
            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern int getCameraType(IntPtr This);
            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern int getOrientation(IntPtr This);
            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern double getTimestamp(IntPtr This);
            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool startCamera(IntPtr This, FunctorOfVoidFromRawPointerOfVoidAndInt update);
            [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
            public static extern void stopCamera(IntPtr This);
        }

        private class ExternalCamera : IDisposable
        {
            private IntPtr inner;

            public bool open()
            {
                return ExternalCameraDetail.openCamera(out inner);
            }

            public void close()
            {
                if (inner == null) { return; }
                ExternalCameraDetail.closeCamera(inner);
            }

            public int getImageWidth()
            {
                if (inner == null) { throw new InvalidOperationException(); }
                return ExternalCameraDetail.getImageWidth(inner);
            }

            public int getImageHeight()
            {
                if (inner == null) { throw new InvalidOperationException(); }
                return ExternalCameraDetail.getImageHeight(inner);
            }

            public int getCameraType()
            {
                if (inner == null) { throw new InvalidOperationException(); }
                return ExternalCameraDetail.getCameraType(inner);
            }

            public int getOrientation()
            {
                if (inner == null) { throw new InvalidOperationException(); }
                return ExternalCameraDetail.getOrientation(inner);
            }

            public double getTimestamp()
            {
                if (inner == null) { throw new InvalidOperationException(); }
                return ExternalCameraDetail.getTimestamp(inner);
            }

            public bool start(Action<IntPtr, int> update)
            {
                if (inner == null) { throw new InvalidOperationException(); }
                return ExternalCameraDetail.startCamera(inner, ExternalCameraDetail.FunctorOfVoidFromRawPointerOfVoidAndInt_to_c(update));
            }

            public void stop()
            {
                if (inner == null) { return; }
                ExternalCameraDetail.stopCamera(inner);
            }

            public void Dispose()
            {
                close();
                GC.SuppressFinalize(this);
            }

            ~ExternalCamera()
            {
                close();
            }
        }
#endif
    }
}
