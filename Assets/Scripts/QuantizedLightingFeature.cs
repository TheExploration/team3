using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class QuantizedLightingFeature : ScriptableRendererFeature
{
    class QuantizedLightingPass : ScriptableRenderPass
    {
        private Material material;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;

        public QuantizedLightingPass(Material material)
        {
            this.material = material;
            tempTexture.Init("_TempTexture");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Quantized Lighting");

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTexture.id, opaqueDesc);
            Blit(cmd, source, tempTexture.Identifier(), material, 0);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material material;
        [Range(2, 10)]
        public int quantizationLevels = 3;
    }

    public Settings settings = new Settings();
    QuantizedLightingPass pass;

    public override void Create()
    {
        pass = new QuantizedLightingPass(settings.material);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) return;

        settings.material.SetFloat("_QuantizationLevels", settings.quantizationLevels);
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
