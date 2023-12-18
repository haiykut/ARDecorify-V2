//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using UnityEngine;
using UnityEngine.Rendering;
#if EASYAR_URP_ENABLE
using UnityEngine.Rendering.Universal;
#else
using ScriptableRendererFeature = UnityEngine.ScriptableObject;
#endif

namespace SpatialMap_SparseSpatialMap
{
    public class SampleOneShotRendererFeature : ScriptableRendererFeature
    {
#if EASYAR_URP_ENABLE
        CameraImageRenderPass renderPass;

        public override void Create()
        {
            renderPass = new CameraImageRenderPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Camera camera = renderingData.cameraData.camera;
            if (!camera) { return; }
            var oneshot = camera.GetComponent<OneShot>();
            if (!oneshot) { return; }

            renderPass.Setup(oneshot, renderer.cameraColorTarget);
            renderer.EnqueuePass(renderPass);
        }

        class CameraImageRenderPass : ScriptableRenderPass
        {
            OneShot oneshot;
            RenderTargetIdentifier colorTarget;

            public CameraImageRenderPass()
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            }

            public void Setup(OneShot shot, RenderTargetIdentifier color)
            {
                oneshot = shot;
                colorTarget = color;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var destTexture = new RenderTexture(Screen.width, Screen.height, 0);
                var cmd = CommandBufferPool.Get();
                if (oneshot.mirror)
                {
                    var material = Instantiate(Resources.Load<Material>("Sample_MirrorTexture"));
                    cmd.Blit(colorTarget, destTexture, material);
                }
                else
                {
                    cmd.Blit(colorTarget, destTexture);
                }
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
                context.Submit();

                RenderTexture.active = destTexture;
                var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();
                RenderTexture.active = null;
                Destroy(destTexture);

                oneshot.callback(texture);
                Destroy(oneshot);
            }

            public override void FrameCleanup(CommandBuffer commandBuffer)
            {
            }
        }
#endif
    }
}
