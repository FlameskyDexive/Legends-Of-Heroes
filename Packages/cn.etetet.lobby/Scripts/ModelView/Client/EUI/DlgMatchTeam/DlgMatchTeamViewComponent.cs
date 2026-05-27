using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgMatchTeam))]
    [EnableMethod]
    public class DlgMatchTeamViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Button E_ConfirmButton => this.GetChild<Button>(ref this.m_E_ConfirmButton, "BackGround/E_Confirm");
        public Image E_ConfirmImage => this.GetChild<Image>(ref this.m_E_ConfirmImage, "BackGround/E_Confirm");
        public LoopHorizontalScrollRect ELoopScrollList_RolesLoopHorizontalScrollRect => this.GetChild<LoopHorizontalScrollRect>(ref this.m_ELoopScroll, "BackGround/ELoopScrollList_Roles");
        public Button E_CancelButton => this.GetChild<Button>(ref this.m_E_CancelButton, "BackGround/E_Cancel");
        public Image E_CancelImage => this.GetChild<Image>(ref this.m_E_CancelImage, "BackGround/E_Cancel");
        public Text ECountDownText => this.GetChild<Text>(ref this.m_ECountDownText, "BackGround/ECountDown");
        public Text ETitleText => this.GetChild<Text>(ref this.m_ETitleText, "BackGround/ETitle");

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
            this.m_E_ConfirmButton = null;
            this.m_E_ConfirmImage = null;
            this.m_ELoopScroll = null;
            this.m_E_CancelButton = null;
            this.m_E_CancelImage = null;
            this.m_ECountDownText = null;
            this.m_ETitleText = null;
            this.uiTransform = null;
        }

        private Button m_E_ConfirmButton;
        private Image m_E_ConfirmImage;
        private LoopHorizontalScrollRect m_ELoopScroll;
        private Button m_E_CancelButton;
        private Image m_E_CancelImage;
        private Text m_ECountDownText;
        private Text m_ETitleText;
    }
}
