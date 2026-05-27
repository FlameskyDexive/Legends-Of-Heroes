using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgHelper))]
    [EnableMethod]
    public class DlgHelperViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Text E_TipText
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_E_TipText == null)
                {
                    this.m_E_TipText = UIFindHelper.FindDeepChild<Text>(this.uiTransform.gameObject, "E_Tip");
                }
                return this.m_E_TipText;
            }
        }

        public void DestroyWidget()
        {
            this.m_E_TipText = null;
            this.uiTransform = null;
        }

        private Text m_E_TipText;
    }
}
