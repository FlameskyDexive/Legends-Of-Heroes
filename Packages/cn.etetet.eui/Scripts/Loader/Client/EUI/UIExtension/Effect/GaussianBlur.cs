using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 高斯模糊后处理：基于 <see cref="PostEffectBase"/>，
    /// 用迭代横竖两次模糊 + 降分辨率得到性能可控的高斯模糊。
    /// </summary>
    [ExecuteInEditMode]
    public class GaussianBlur : PostEffectBase
    {
        public float BlurRadius = 1.0f;
        public int downSample = 2;
        public int iteration = 1;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!_Material)
            {
                Graphics.Blit(source, destination);
                return;
            }

            RenderTexture rt1 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);
            RenderTexture rt2 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);

            Graphics.Blit(source, rt1);

            for (int i = 0; i < iteration; i++)
            {
                _Material.SetVector("_offsets", new Vector4(0, BlurRadius, 0, 0));
                Graphics.Blit(rt1, rt2, _Material);
                _Material.SetVector("_offsets", new Vector4(BlurRadius, 0, 0, 0));
                Graphics.Blit(rt2, rt1, _Material);
            }

            Graphics.Blit(rt1, destination);

            RenderTexture.ReleaseTemporary(rt1);
            RenderTexture.ReleaseTemporary(rt2);
        }
    }
}
