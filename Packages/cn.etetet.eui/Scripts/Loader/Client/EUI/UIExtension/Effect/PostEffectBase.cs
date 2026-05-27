using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 屏幕后处理特效基类：挂在 Camera 上，Inspector 拖入 Shader 即可生成对应材质。
    /// 子类一般实现 OnRenderImage 完成具体的后处理操作。
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class PostEffectBase : MonoBehaviour
    {
        public Shader shader = null;
        private Material _material = null;

        public Material _Material
        {
            get
            {
                if (_material == null)
                {
                    _material = GenerateMaterial(shader);
                }
                return _material;
            }
        }

        protected Material GenerateMaterial(Shader targetShader)
        {
            if (targetShader == null)
            {
                return null;
            }
            if (!targetShader.isSupported)
            {
                return null;
            }
            Material mat = new Material(targetShader)
            {
                hideFlags = HideFlags.DontSave,
            };
            return mat;
        }
    }
}
