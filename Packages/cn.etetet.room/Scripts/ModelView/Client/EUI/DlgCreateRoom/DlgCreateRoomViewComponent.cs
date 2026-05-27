using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgCreateRoom))]
    [EnableMethod]
    public class DlgCreateRoomViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public RectTransform EGBackGroundRectTransform => this.GetChild<RectTransform>(ref this.m_EGBackGroundRectTransform, "EGBackGround");
        public InputField ERoomNameInputField => this.GetChild<InputField>(ref this.m_ERoomNameInputField, "EGBackGround/ERoomName");
        public Dropdown EModeDropdown => this.GetChild<Dropdown>(ref this.m_EModeDropdown, "EGBackGround/EMode");
        public InputField EMaxPlayersInputField => this.GetChild<InputField>(ref this.m_EMaxPlayersInputField, "EGBackGround/EMaxPlayers");
        public InputField EPasswordInputField => this.GetChild<InputField>(ref this.m_EPasswordInputField, "EGBackGround/EPassword");
        public Button ECreateButton => this.GetChild<Button>(ref this.m_ECreateButton, "EGBackGround/ECreate");
        public Button ECancelButton => this.GetChild<Button>(ref this.m_ECancelButton, "EGBackGround/ECancel");

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
            this.m_ERoomNameInputField = null;
            this.m_EModeDropdown = null;
            this.m_EMaxPlayersInputField = null;
            this.m_EPasswordInputField = null;
            this.m_ECreateButton = null;
            this.m_ECancelButton = null;
            this.uiTransform = null;
        }

        private RectTransform m_EGBackGroundRectTransform;
        private InputField m_ERoomNameInputField;
        private Dropdown m_EModeDropdown;
        private InputField m_EMaxPlayersInputField;
        private InputField m_EPasswordInputField;
        private Button m_ECreateButton;
        private Button m_ECancelButton;
    }
}
