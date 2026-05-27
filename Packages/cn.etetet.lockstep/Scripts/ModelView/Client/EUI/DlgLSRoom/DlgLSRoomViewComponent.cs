using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgLSRoom))]
    [EnableMethod]
    public class DlgLSRoomViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Text EprogressText => this.GetChild<Text>(ref this.m_EprogressText, "Panel/Eprogress");
        public Text EpredictText => this.GetChild<Text>(ref this.m_EpredictText, "Panel/Epredict");
        public Text EframecountText => this.GetChild<Text>(ref this.m_EframecountText, "Panel/Eframecount");
        public RectTransform EGReplayRectTransform => this.GetChild<RectTransform>(ref this.m_EGReplayRectTransform, "Panel/EGReplay");
        public InputField EjumpToCountInputField => this.GetChild<InputField>(ref this.m_EjumpToCountInputField, "Panel/EGReplay/EjumpToCount");
        public Image EjumpToCountImage => this.GetChild<Image>(ref this.m_EjumpToCountImage, "Panel/EGReplay/EjumpToCount");
        public Button EjumpButton => this.GetChild<Button>(ref this.m_EjumpButton, "Panel/EGReplay/Ejump");
        public Image EjumpImage => this.GetChild<Image>(ref this.m_EjumpImage, "Panel/EGReplay/Ejump");
        public Button EspeedButton => this.GetChild<Button>(ref this.m_EspeedButton, "Panel/EGReplay/Espeed");
        public Image EspeedImage => this.GetChild<Image>(ref this.m_EspeedImage, "Panel/EGReplay/Espeed");
        public RectTransform EGPlayRectTransform => this.GetChild<RectTransform>(ref this.m_EGPlayRectTransform, "Panel/EGPlay");
        public InputField E_SaveNameInputField => this.GetChild<InputField>(ref this.m_E_SaveNameInputField, "Panel/EGPlay/E_SaveName");
        public Image E_SaveNameImage => this.GetChild<Image>(ref this.m_E_SaveNameImage, "Panel/EGPlay/E_SaveName");
        public Button E_SaveReplayButton => this.GetChild<Button>(ref this.m_E_SaveReplayButton, "Panel/EGPlay/E_SaveReplay");
        public Image E_SaveReplayImage => this.GetChild<Image>(ref this.m_E_SaveReplayImage, "Panel/EGPlay/E_SaveReplay");

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
            this.m_EprogressText = null;
            this.m_EpredictText = null;
            this.m_EframecountText = null;
            this.m_EGReplayRectTransform = null;
            this.m_EjumpToCountInputField = null;
            this.m_EjumpToCountImage = null;
            this.m_EjumpButton = null;
            this.m_EjumpImage = null;
            this.m_EspeedButton = null;
            this.m_EspeedImage = null;
            this.m_EGPlayRectTransform = null;
            this.m_E_SaveNameInputField = null;
            this.m_E_SaveNameImage = null;
            this.m_E_SaveReplayButton = null;
            this.m_E_SaveReplayImage = null;
            this.uiTransform = null;
        }

        private Text m_EprogressText;
        private Text m_EpredictText;
        private Text m_EframecountText;
        private RectTransform m_EGReplayRectTransform;
        private InputField m_EjumpToCountInputField;
        private Image m_EjumpToCountImage;
        private Button m_EjumpButton;
        private Image m_EjumpImage;
        private Button m_EspeedButton;
        private Image m_EspeedImage;
        private RectTransform m_EGPlayRectTransform;
        private InputField m_E_SaveNameInputField;
        private Image m_E_SaveNameImage;
        private Button m_E_SaveReplayButton;
        private Image m_E_SaveReplayImage;
    }
}
