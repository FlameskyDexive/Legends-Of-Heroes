using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgLSLobby))]
    [EnableMethod]
    public class DlgLSLobbyViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Button EMatchButton => this.GetChild<Button>(ref this.m_EMatchButton, "Panel/EMatch");
        public Image EMatchImage => this.GetChild<Image>(ref this.m_EMatchImage, "Panel/EMatch");
        public InputField EReplayPathInputField => this.GetChild<InputField>(ref this.m_EReplayPathInputField, "Panel/GameObject/EReplayPath");
        public Image EReplayPathImage => this.GetChild<Image>(ref this.m_EReplayPathImage, "Panel/GameObject/EReplayPath");
        public Button EReplayButton => this.GetChild<Button>(ref this.m_EReplayButton, "Panel/GameObject/EReplay");
        public Image EReplayImage => this.GetChild<Image>(ref this.m_EReplayImage, "Panel/GameObject/EReplay");

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
            this.m_EMatchButton = null;
            this.m_EMatchImage = null;
            this.m_EReplayPathInputField = null;
            this.m_EReplayPathImage = null;
            this.m_EReplayButton = null;
            this.m_EReplayImage = null;
            this.uiTransform = null;
        }

        private Button m_EMatchButton;
        private Image m_EMatchImage;
        private InputField m_EReplayPathInputField;
        private Image m_EReplayPathImage;
        private Button m_EReplayButton;
        private Image m_EReplayImage;
    }
}
