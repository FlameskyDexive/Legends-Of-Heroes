namespace ET
{
    // 输入/操作枚举。原在 Ori 的 Battle/BattleDefine, 因属于"操控"语义(OperaComponent 使用),
    // 迁移时下沉到 mapplay(OperaComponent 所在包), 避免战斗包(cn.etetet.skill, 更高层)被反向依赖。

    /// <summary>
    /// 输入操作类型
    /// </summary>
    public enum EInputType : byte
    {
        Key,
        KeyDown,
        KeyUp,
    }

    public enum EOperateStatus : byte
    {
        Success = 0,
        Error = 1,
    }

    public enum EOperateType : byte
    {
        Move = 0,
        Jump = 1,
        Attack = 2, // 普攻
        Skill1,
        Skill2,
        Skill3,
        Skill4,
    }

    // 操作触发技能释放的事件。用于解耦: 操作处理器(mapplay, 低层)发布此事件,
    // 战斗包(cn.etetet.skill, 高层)订阅并真正释放技能, 从而 mapplay 不需反向依赖 skill。
    // Index: 技能槽位(Skill1=0, Skill2=1 ...), 对应 SkillComponent.SpellSkill 的 index 参数。
    public struct OperateSkillCast
    {
        public EntityRef<Unit> Unit { get; set; }
        public int Index { get; set; }
    }
}
