using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgLogin))]
    [EnableMethod]
    public class DlgLoginViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Button ELoginButton
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ELoginButton == null)
                {
                    this.m_ELoginButton = UIFindHelper.FindDeepChild<Button>(this.uiTransform.gameObject, "ELogin");
                }
                return this.m_ELoginButton;
            }
        }

        public InputField EAccountInputField
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_EAccountInputField == null)
                {
                    this.m_EAccountInputField = UIFindHelper.FindDeepChild<InputField>(this.uiTransform.gameObject, "EAccount");
                }
                return this.m_EAccountInputField;
            }
        }

        public InputField EPasswordInputField
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
                    this.m_EPasswordInputField = UIFindHelper.FindDeepChild<InputField>(this.uiTransform.gameObject, "EPassword");
                }
                return this.m_EPasswordInputField;
            }
        }

        public void DestroyWidget()
        {
            this.m_ELoginButton = null;
            this.m_EAccountInputField = null;
            this.m_EPasswordInputField = null;
            this.uiTransform = null;
        }

        private Button m_ELoginButton;
        private InputField m_EAccountInputField;
        private InputField m_EPasswordInputField;
    }
}
