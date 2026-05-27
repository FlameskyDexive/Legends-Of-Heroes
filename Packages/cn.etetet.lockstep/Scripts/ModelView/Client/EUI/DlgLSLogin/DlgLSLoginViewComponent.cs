using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgLSLogin))]
    [EnableMethod]
    public class DlgLSLoginViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public InputField EAccountInputField => this.GetChild<InputField>(ref this.m_EAccountInputField, "Panel/EAccount");
        public Image EAccountImage => this.GetChild<Image>(ref this.m_EAccountImage, "Panel/EAccount");
        public InputField EPasswordInputField => this.GetChild<InputField>(ref this.m_EPasswordInputField, "Panel/EPassword");
        public Image EPasswordImage => this.GetChild<Image>(ref this.m_EPasswordImage, "Panel/EPassword");
        public Button ELoginBtnButton => this.GetChild<Button>(ref this.m_ELoginBtnButton, "Panel/ELoginBtn");
        public Image ELoginBtnImage => this.GetChild<Image>(ref this.m_ELoginBtnImage, "Panel/ELoginBtn");

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
            this.m_EAccountInputField = null;
            this.m_EAccountImage = null;
            this.m_EPasswordInputField = null;
            this.m_EPasswordImage = null;
            this.m_ELoginBtnButton = null;
            this.m_ELoginBtnImage = null;
            this.uiTransform = null;
        }

        private InputField m_EAccountInputField;
        private Image m_EAccountImage;
        private InputField m_EPasswordInputField;
        private Image m_EPasswordImage;
        private Button m_ELoginBtnButton;
        private Image m_ELoginBtnImage;
    }
}
