using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgRedDot))]
    [EnableMethod]
    public class DlgRedDotViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public RectTransform EGBackGroundRectTransform
        {
            get
            {
                if (this.uiTransform == null) { Log.Error("uiTransform is null."); return null; }
                if (this.m_EGBackGroundRectTransform == null)
                {
                    this.m_EGBackGroundRectTransform = UIFindHelper.FindDeepChild<RectTransform>(this.uiTransform.gameObject, "EGBackGround");
                }
                return this.m_EGBackGroundRectTransform;
            }
        }

        public Button EButton_rootButton => this.GetChild<Button>(ref this.m_EButton_rootButton, "EButton_root");
        public Image EButton_rootImage => this.GetChild<Image>(ref this.m_EButton_rootImage, "EButton_root");
        public Button EButton_BagButton => this.GetChild<Button>(ref this.m_EButton_BagButton, "EButton_Bag");
        public Image EButton_BagImage => this.GetChild<Image>(ref this.m_EButton_BagImage, "EButton_Bag");
        public Button EButton_BagNode1Button => this.GetChild<Button>(ref this.m_EButton_BagNode1Button, "EButton_BagNode1");
        public Image EButton_BagNode1Image => this.GetChild<Image>(ref this.m_EButton_BagNode1Image, "EButton_BagNode1");
        public Button EButton_BagNode2Button => this.GetChild<Button>(ref this.m_EButton_BagNode2Button, "EButton_BagNode2");
        public Image EButton_BagNode2Image => this.GetChild<Image>(ref this.m_EButton_BagNode2Image, "EButton_BagNode2");
        public Button EButton_MailButton => this.GetChild<Button>(ref this.m_EButton_MailButton, "EButton_Mail");
        public Image EButton_MailImage => this.GetChild<Image>(ref this.m_EButton_MailImage, "EButton_Mail");
        public Button EButton_MailNode1Button => this.GetChild<Button>(ref this.m_EButton_MailNode1Button, "EButton_MailNode1");
        public Image EButton_MailNode1Image => this.GetChild<Image>(ref this.m_EButton_MailNode1Image, "EButton_MailNode1");
        public Button EButton_MailNode2Button => this.GetChild<Button>(ref this.m_EButton_MailNode2Button, "EButton_MailNode2");
        public Image EButton_MailNode2Image => this.GetChild<Image>(ref this.m_EButton_MailNode2Image, "EButton_MailNode2");

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
            this.m_EButton_rootButton = null;
            this.m_EButton_rootImage = null;
            this.m_EButton_BagButton = null;
            this.m_EButton_BagImage = null;
            this.m_EButton_BagNode1Button = null;
            this.m_EButton_BagNode1Image = null;
            this.m_EButton_BagNode2Button = null;
            this.m_EButton_BagNode2Image = null;
            this.m_EButton_MailButton = null;
            this.m_EButton_MailImage = null;
            this.m_EButton_MailNode1Button = null;
            this.m_EButton_MailNode1Image = null;
            this.m_EButton_MailNode2Button = null;
            this.m_EButton_MailNode2Image = null;
            this.uiTransform = null;
        }

        private RectTransform m_EGBackGroundRectTransform;
        private Button m_EButton_rootButton;
        private Image m_EButton_rootImage;
        private Button m_EButton_BagButton;
        private Image m_EButton_BagImage;
        private Button m_EButton_BagNode1Button;
        private Image m_EButton_BagNode1Image;
        private Button m_EButton_BagNode2Button;
        private Image m_EButton_BagNode2Image;
        private Button m_EButton_MailButton;
        private Image m_EButton_MailImage;
        private Button m_EButton_MailNode1Button;
        private Image m_EButton_MailNode1Image;
        private Button m_EButton_MailNode2Button;
        private Image m_EButton_MailNode2Image;
    }
}
