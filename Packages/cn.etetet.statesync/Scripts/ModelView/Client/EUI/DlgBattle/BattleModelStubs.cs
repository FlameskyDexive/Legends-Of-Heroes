namespace ET.Client
{
    // BattleModelStubs：DlgBattle 移植所需的 Skill / SkillComponent / ESkillAbstractType 桩类型。
    // 等 cn.etetet.spell 真正移植技能系统后，请把本文件删除并切换为正式定义。

    public enum ESkillAbstractType
    {
        None = 0,
        ActiveSkill = 1,
        PassiveSkill = 2,
    }

    [ChildOf(typeof(SkillComponent))]
    public class Skill : Entity, IAwake
    {
        public int CurrentCD;
        public int CD;
    }

    [ComponentOf(typeof(Unit))]
    public class SkillComponent : Entity, IAwake
    {
    }
}
