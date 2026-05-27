using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(DlgBattle))]
    [FriendOf(typeof(DlgBattle))]
    [FriendOf(typeof(SkillComponent))]
    public static partial class DlgBattleSystem
    {
        [EntitySystem]
        public static void Awake(this DlgBattle self)
        {
        }

        [EntitySystem]
        public static void Update(this DlgBattle self)
        {
            if (self.Skill1 != null)
            {
                self.SetSkillCD(self.Skill1, self.View.EBtnSkill1Button, self.View.EIMgCD1Image, self.View.ETextSkill1Text, self.View.EImgMask1Image);
            }
            if (self.Skill2 != null)
            {
                self.SetSkillCD(self.Skill2, self.View.EBtnSkill2Button, self.View.EIMgCD2Image, self.View.ETextSkill2Text, self.View.EImgMask2Image);
            }
        }

        public static void RegisterUIEvent(this DlgBattle self)
        {
            EntityRef<DlgBattle> selfRef = self;
            self.View.EBtnSkill1Button.AddListener(self.Root(), () => OnClickSkill1(selfRef));
            self.View.EBtnSkill2Button.AddListener(self.Root(), () => OnClickSkill2(selfRef));
            self.View.E_JoystickJoystick.OnValueChanged.AddListener(v => OnPressJoystick(selfRef, v));
            self.View.E_JoystickJoystick.OnPointerUp.AddListener(v => OnUpJoystick(selfRef, v));
        }

        public static void ShowWindow(this DlgBattle self, Entity contextData = null)
        {
        }

        public static void RefreshSkillView(this DlgBattle self)
        {
            // TODO: 等 cn.etetet.spell 提供 SkillComponent/Skill/ESkillAbstractType 后启用真实逻辑。
            Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.Scene());
            if (unit == null)
            {
                return;
            }

            SkillComponent skillComp = unit.GetComponent<SkillComponent>();
            if (skillComp == null)
            {
                return;
            }

            if (skillComp.TryGetSkill(ESkillAbstractType.ActiveSkill, 0, out Skill skill1))
            {
                self.Skill1 = skill1;
                self.InitSkill(self.Skill1, self.View.EIconSkill1Image);
            }
            if (skillComp.TryGetSkill(ESkillAbstractType.ActiveSkill, 1, out Skill skill2))
            {
                self.Skill2 = skill2;
                self.InitSkill(self.Skill2, self.View.EIconSkill2Image);
            }
        }

        private static void InitSkill(this DlgBattle self, Skill skill, Image imgIcon)
        {
            if (skill == null)
            {
                return;
            }
            // TODO: 接入真实技能图标 / 资源加载。
        }

        private static void SetSkillCD(this DlgBattle self, Skill skill, Button btn, Image imgCD, Text cdText, Image imgCover)
        {
            bool isInCd = skill.IsInCd();
            imgCD.gameObject.SetActive(isInCd);
            imgCover.gameObject.SetActive(isInCd);
            cdText.gameObject.SetActive(isInCd);
            if (isInCd)
            {
                cdText.text = (skill.CurrentCD / 1000f).ToString("f1");
                imgCD.fillAmount = skill.CD <= 0 ? 0 : skill.CurrentCD / (float)skill.CD;
            }
        }

        private static void OnClickSkill1(EntityRef<DlgBattle> selfRef)
        {
            DlgBattle self = selfRef;
            if (self == null) { return; }
            Skill skill = self.Skill1;
            if (skill == null || skill.IsInCd()) { return; }
            self.Scene().GetComponent<OperaComponent>()?.OnClickSkill1();
        }

        private static void OnClickSkill2(EntityRef<DlgBattle> selfRef)
        {
            DlgBattle self = selfRef;
            if (self == null) { return; }
            Skill skill = self.Skill2;
            if (skill == null || skill.IsInCd()) { return; }
            self.Scene().GetComponent<OperaComponent>()?.OnClickSkill2();
        }

        private static void OnPressJoystick(EntityRef<DlgBattle> selfRef, Vector2 v)
        {
            DlgBattle self = selfRef;
            if (self == null) { return; }
            if (v == Vector2.zero) { return; }
            self.Scene().GetComponent<OperaComponent>()?.OnMove(v);
            self.LastJoyPos = v;
        }

        private static void OnUpJoystick(EntityRef<DlgBattle> selfRef, Vector2 v)
        {
            DlgBattle self = selfRef;
            if (self == null) { return; }
            self.Scene().GetComponent<OperaComponent>()?.StopMove();
        }
    }
}
