namespace ET
{
    // 行为事件类型。原为 Luban 生成枚举（BattleDefine 内同名定义被注释）。迁移为普通枚举。
    public enum EActionEventType
    {
        RangeDamage = 1,
        Bullet = 2,
        AddBuff = 3,
        RemoveBuff = 4,
        Stealth = 5,
        Invincible = 6,
        ChangeNumeric = 7,
        PlayAnimation = 100,
        ShakeCamera = 101,
        PlayEffect = 102,
        PlaySound = 103,
        HideWeapon = 104,
        PlayMaterial = 105,
        RecoverMaterial = 106,
    }
}
