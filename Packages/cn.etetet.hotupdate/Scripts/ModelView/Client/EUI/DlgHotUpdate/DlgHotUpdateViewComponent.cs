using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgHotUpdate))]
    [EnableMethod]
    public class DlgHotUpdateViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Text E_TxtLoadingText => this.GetChild<Text>(ref this.m_E_TxtLoadingText, "E_TxtLoading");
        public Image E_ImgLoadingImage => this.GetChild<Image>(ref this.m_E_ImgLoadingImage, "LoadingBg/E_ImgLoading");

        private T GetChild<T>(ref T cache, string path) where T : Component
        {
            if (this.uiTransform == null) { Log.Error("uiTransform is null."); return null; }
            if (cache == null)
            {
                cache = UIFindHelper.FindDeepChild<T>(this.uiTransform.gameObject, path);
            }
            return cache;
        }

        public void DestroyWidget()
        {
            this.m_E_TxtLoadingText = null;
            this.m_E_ImgLoadingImage = null;
            this.uiTransform = null;
        }

        private Text m_E_TxtLoadingText;
        private Image m_E_ImgLoadingImage;
    }
}
