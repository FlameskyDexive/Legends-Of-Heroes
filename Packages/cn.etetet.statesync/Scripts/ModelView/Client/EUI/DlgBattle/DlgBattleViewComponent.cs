using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgBattle))]
    [EnableMethod]
    public class DlgBattleViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Text E_PlayerNameText => this.GetChild<Text>(ref this.m_E_PlayerNameText, "Top/PlayerRoot/E_PlayerName");
        public Joystick E_JoystickJoystick => this.GetChild<Joystick>(ref this.m_E_JoystickJoystick, "Bottom/Joy/E_Joystick");
        public Image E_JoystickImage => this.GetChild<Image>(ref this.m_E_JoystickImage, "Bottom/Joy/E_Joystick");
        public Button EBtnSkill1Button => this.GetChild<Button>(ref this.m_EBtnSkill1Button, "Bottom/Skill/EBtnSkill1");
        public Image EIconSkill1Image => this.GetChild<Image>(ref this.m_EIconSkill1Image, "Bottom/Skill/EBtnSkill1/EIconSkill1");
        public Text ETextSkill1Text => this.GetChild<Text>(ref this.m_ETextSkill1Text, "Bottom/Skill/EBtnSkill1/ETextSkill1");
        public Image EIMgCD1Image => this.GetChild<Image>(ref this.m_EIMgCD1Image, "Bottom/Skill/EBtnSkill1/EIMgCD1");
        public Image EImgMask1Image => this.GetChild<Image>(ref this.m_EImgMask1Image, "Bottom/Skill/EBtnSkill1/EImgMask1");
        public Button EBtnSkill2Button => this.GetChild<Button>(ref this.m_EBtnSkill2Button, "Bottom/Skill/EBtnSkill2");
        public Image EIconSkill2Image => this.GetChild<Image>(ref this.m_EIconSkill2Image, "Bottom/Skill/EBtnSkill2/EIconSkill2");
        public Text ETextSkill2Text => this.GetChild<Text>(ref this.m_ETextSkill2Text, "Bottom/Skill/EBtnSkill2/ETextSkill2");
        public Image EIMgCD2Image => this.GetChild<Image>(ref this.m_EIMgCD2Image, "Bottom/Skill/EBtnSkill2/EIMgCD2");
        public Image EImgMask2Image => this.GetChild<Image>(ref this.m_EImgMask2Image, "Bottom/Skill/EBtnSkill2/EImgMask2");

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
            this.m_E_PlayerNameText = null;
            this.m_E_JoystickJoystick = null;
            this.m_E_JoystickImage = null;
            this.m_EBtnSkill1Button = null;
            this.m_EIconSkill1Image = null;
            this.m_ETextSkill1Text = null;
            this.m_EIMgCD1Image = null;
            this.m_EImgMask1Image = null;
            this.m_EBtnSkill2Button = null;
            this.m_EIconSkill2Image = null;
            this.m_ETextSkill2Text = null;
            this.m_EIMgCD2Image = null;
            this.m_EImgMask2Image = null;
            this.uiTransform = null;
        }

        private Text m_E_PlayerNameText;
        private Joystick m_E_JoystickJoystick;
        private Image m_E_JoystickImage;
        private Button m_EBtnSkill1Button;
        private Image m_EIconSkill1Image;
        private Text m_ETextSkill1Text;
        private Image m_EIMgCD1Image;
        private Image m_EImgMask1Image;
        private Button m_EBtnSkill2Button;
        private Image m_EIconSkill2Image;
        private Text m_ETextSkill2Text;
        private Image m_EIMgCD2Image;
        private Image m_EImgMask2Image;
    }
}
