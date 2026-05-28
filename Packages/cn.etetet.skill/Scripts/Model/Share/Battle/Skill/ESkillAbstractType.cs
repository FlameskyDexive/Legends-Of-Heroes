namespace ET
{
    // 技能抽象类型。原为 Luban 生成枚举（BattleDefine 内同名定义被注释）。迁移为普通枚举。
    public enum ESkillAbstractType
    {
        NormalAttack = 1,
        ActiveSkill = 2,
        PassiveSkill = 3,
        WeaponSkill = 4,
        MountSkill = 5,
    }
}
