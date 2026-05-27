using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// 通用复用 UI Entity：典型用法是把一段 UI 子树（带 EImage_test / ELabel_test 等节点）
    /// 实例化为一个 Entity，绑定 Transform 后可通过 UIFindHelper 自动取得控件。
    /// 实际项目中可基于此扩展更多复用 UI 类型。
    /// </summary>
    [ChildOf]
    [EnableMethod]
    public class ESReuseUI : Entity, IAwake<Transform>, IDestroy
    {
        public Image EImage_testImage
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_EImage_testImage == null)
                {
                    this.m_EImage_testImage = UIFindHelper.FindDeepChild<Image>(this.uiTransform.gameObject, "EImage_test");
                }
                return this.m_EImage_testImage;
            }
        }

        public Text ELabel_testText
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ELabel_testText == null)
                {
                    this.m_ELabel_testText = UIFindHelper.FindDeepChild<Text>(this.uiTransform.gameObject, "ELabel_test");
                }
                return this.m_ELabel_testText;
            }
        }

        public void DestroyWidget()
        {
            this.m_EImage_testImage = null;
            this.m_ELabel_testText = null;
            this.uiTransform = null;
        }

        private Image m_EImage_testImage;
        private Text m_ELabel_testText;
        public Transform uiTransform;
    }
}
