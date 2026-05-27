using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgLobby))]
    [EnableMethod]
    public class DlgLobbyViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public RectTransform EGBackGroundRectTransform => this.GetChild<RectTransform>(ref this.m_EGBackGroundRectTransform, "EGBackGround");
        public Button EEnterMapButton => this.GetChild<Button>(ref this.m_EEnterMapButton, "EGBackGround/EEnterMap");
        public Image EEnterMapImage => this.GetChild<Image>(ref this.m_EEnterMapImage, "EGBackGround/EEnterMap");
        public EUIButton EAvatarEUIButton => this.GetChild<EUIButton>(ref this.m_EAvatarEUIButton, "EGBackGround/Top/PlayerInfo/EAvatar");
        public EUIImage EAvatarEUIImage => this.GetChild<EUIImage>(ref this.m_EAvatarEUIImage, "EGBackGround/Top/PlayerInfo/EAvatar");
        public Text EPlayerNameText => this.GetChild<Text>(ref this.m_EPlayerNameText, "EGBackGround/Top/PlayerInfo/EPlayerName");
        public Button EMatchButton => this.GetChild<Button>(ref this.m_EMatchButton, "EGBackGround/EMatch");
        public Image EMatchImage => this.GetChild<Image>(ref this.m_EMatchImage, "EGBackGround/EMatch");
        public Button ECreateRoomButton => this.GetChild<Button>(ref this.m_ECreateRoomButton, "EGBackGround/ECreateRoom");
        public Button ERoomListButton => this.GetChild<Button>(ref this.m_ERoomListButton, "EGBackGround/ERoomList");
        public Button EBackToLoginButton => this.GetChild<Button>(ref this.m_EBackToLoginButton, "EGBackGround/EBackToLogin");
        public Image EBackToLoginImage => this.GetChild<Image>(ref this.m_EBackToLoginImage, "EGBackGround/EBackToLogin");

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
            this.m_EGBackGroundRectTransform = null;
            this.m_EEnterMapButton = null;
            this.m_EEnterMapImage = null;
            this.m_EAvatarEUIButton = null;
            this.m_EAvatarEUIImage = null;
            this.m_EPlayerNameText = null;
            this.m_EMatchButton = null;
            this.m_EMatchImage = null;
            this.m_ECreateRoomButton = null;
            this.m_ERoomListButton = null;
            this.m_EBackToLoginButton = null;
            this.m_EBackToLoginImage = null;
            this.uiTransform = null;
        }

        private RectTransform m_EGBackGroundRectTransform;
        private Button m_EEnterMapButton;
        private Image m_EEnterMapImage;
        private EUIButton m_EAvatarEUIButton;
        private EUIImage m_EAvatarEUIImage;
        private Text m_EPlayerNameText;
        private Button m_EMatchButton;
        private Image m_EMatchImage;
        private Button m_ECreateRoomButton;
        private Button m_ERoomListButton;
        private Button m_EBackToLoginButton;
        private Image m_EBackToLoginImage;
    }
}
