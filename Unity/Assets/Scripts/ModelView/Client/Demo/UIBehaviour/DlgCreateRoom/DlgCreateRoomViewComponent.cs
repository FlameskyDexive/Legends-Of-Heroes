using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgCreateRoom))]
    [EnableMethod]
    public class DlgCreateRoomViewComponent : Entity, IAwake, IDestroy
    {
        public UnityEngine.RectTransform EGBackGroundRectTransform
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_EGBackGroundRectTransform == null)
                {
                    this.m_EGBackGroundRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject, "EGBackGround");
                }
                return this.m_EGBackGroundRectTransform;
            }
        }

        public UnityEngine.UI.InputField ERoomNameInputField
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ERoomNameInputField == null)
                {
                    this.m_ERoomNameInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject, "EGBackGround/ERoomName");
                }
                return this.m_ERoomNameInputField;
            }
        }

        public UnityEngine.UI.Dropdown EModeDropdown
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_EModeDropdown == null)
                {
                    this.m_EModeDropdown = UIFindHelper.FindDeepChild<UnityEngine.UI.Dropdown>(this.uiTransform.gameObject, "EGBackGround/EMode");
                }
                return this.m_EModeDropdown;
            }
        }

        public UnityEngine.UI.InputField EMaxPlayersInputField
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_EMaxPlayersInputField == null)
                {
                    this.m_EMaxPlayersInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject, "EGBackGround/EMaxPlayers");
                }
                return this.m_EMaxPlayersInputField;
            }
        }

        public UnityEngine.UI.InputField EPasswordInputField
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_EPasswordInputField == null)
                {
                    this.m_EPasswordInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject, "EGBackGround/EPassword");
                }
                return this.m_EPasswordInputField;
            }
        }

        public UnityEngine.UI.Button ECreateButton
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ECreateButton == null)
                {
                    this.m_ECreateButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject, "EGBackGround/ECreate");
                }
                return this.m_ECreateButton;
            }
        }

        public UnityEngine.UI.Button ECancelButton
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ECancelButton == null)
                {
                    this.m_ECancelButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject, "EGBackGround/ECancel");
                }
                return this.m_ECancelButton;
            }
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

        private UnityEngine.RectTransform m_EGBackGroundRectTransform = null;
        private UnityEngine.UI.InputField m_ERoomNameInputField = null;
        private UnityEngine.UI.Dropdown m_EModeDropdown = null;
        private UnityEngine.UI.InputField m_EMaxPlayersInputField = null;
        private UnityEngine.UI.InputField m_EPasswordInputField = null;
        private UnityEngine.UI.Button m_ECreateButton = null;
        private UnityEngine.UI.Button m_ECancelButton = null;
        public Transform uiTransform = null;
    }
}