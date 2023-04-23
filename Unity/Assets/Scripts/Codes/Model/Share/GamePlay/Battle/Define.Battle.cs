namespace ET
{
    /// <summary>
	/// 技能事件类型
	/// </summary>
    public enum ESkillEventType : byte
    {
        /// <summary>
		/// 范围伤害
		/// </summary>
	    RangeDamage = 1,
        /// <summary>
		/// 子弹
		/// </summary>
	    Bullet = 2,
        /// <summary>
		/// 添加buff
		/// </summary>
	    AddBuff = 3,
        /// <summary>
		/// 移除buff
		/// </summary>
	    RemoveBuff = 4,
        /// <summary>
		/// 隐身
		/// </summary>
        Stealth = 5,
    }

    /// <summary>
    /// 技能抽象类型
    /// </summary>
    public enum ESkillAbstractType : byte
    {
        /// <summary>
        /// 普攻
        /// </summary>
        NormalAttack = 1,
        /// <summary>
        /// 主动技能
        /// </summary>
        ActiveSkill = 2,
        /// <summary>
        /// 被动技能
        /// </summary>
        PassiveSkill = 3,
        /// <summary>
        /// 武器技能
        /// </summary>
        WeaponSkill = 4,
        /// <summary>
        /// 坐骑技能
        /// </summary>
        MountSkill = 5,
    }

    /// <summary>
    /// 输入操作类型
    /// </summary>
    public enum EInputType : byte
    {
        Key,
        KeyDown,
        KeyUp,
    }

    public enum EOperateStatus: byte
    {
        Success = 0,
        Error = 1,
    }
    
    public enum EOperateType : byte
    {
        Move = 0,
        Jump = 1,
        Attack = 2,//普攻
        Skill1,
        Skill2,
        Skill3,
        Skill4,
    }

    public enum EColliderType: byte
    {
        Circle,
        Box,
    }


    public enum EHitFromType: byte
    {
        /// <summary>
        /// 普通技能命中（范围伤害等等）
        /// </summary>
        Skill_Normal,
        /// <summary>
        /// 子弹技能命中
        /// </summary>
        Skill_Bullet,
        /// <summary>
        /// buff伤害
        /// </summary>
        Buff,
    }

    public enum EHitResultType: byte
    {
        /// <summary>
        /// 伤害扣血
        /// </summary>
        Damage,
        /// <summary>
        /// 回血
        /// </summary>
        RecoverBlood,
        /// <summary>
        /// 闪避
        /// </summary>
        Doge,
        /// <summary>
        /// 暴击
        /// </summary>
        Crit,
    }

}
