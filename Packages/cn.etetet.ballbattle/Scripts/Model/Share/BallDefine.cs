using Box2DSharp.Dynamics.Contacts;

namespace ET
{
    // 碰撞器形状类型(原 Ori Battle/BattleDefine 中被注释的 EColliderType,迁移到此)
    public enum EColliderType : byte
    {
        Circle = 1,
        Box = 2,
    }

    // 球的逻辑类型(本项目 UnitType 无 Bullet/Food, 用此区分碰撞处理)
    public enum EBallType : byte
    {
        None = 0,
        Player = 1, // 玩家球
        Food = 2,   // 食物球(被吃)
        Bullet = 3, // 子弹
    }

    // 球球玩法常量(正式应迁到配置表 BallConfig 供策划调,这里先给可跑的默认值)
    public static class BallDefine
    {
        // Box2D 世界步进的固定时间步(本项目无 IFixedUpdate, 用 IUpdate + 固定 dt 步进)
        public const float FixedDeltaTime = 1f / 20f;

        // Box2D 求解迭代次数(纯做重叠检测, 迭代可较低)
        public const int VelocityIteration = 8;
        public const int PositionIteration = 3;

        // —— 体型/速度公式(质量=HP 驱动) ——
        public const float MinHp = 1f;          // HP 下限,避免 sqrt(0)
        public const float RadiusCoef = 0.05f;  // Radius = RadiusCoef * sqrt(HP) (面积∝HP)
        public const float BaseSpeed = 5f;       // 最小球的基础速度
        public const float SpeedCoef = 10f;      // Speed = clamp(BaseSpeed*SpeedCoef/sqrt(HP), MinSpeed, BaseSpeed)
        public const float MinSpeed = 1.5f;      // 速度下限(越大越慢但不为 0)

        // —— 吞噬 ——
        public const float EatRatio = 1.15f;     // A 半径 > B 半径 * EatRatio → A 吃 B

        // —— 子弹 ——
        public const int BulletDamage = 100;     // 子弹命中扣的 HP(正式应从子弹/技能配置读)
    }

    // 碰撞开始事件(由 CollisionListenerComponent.BeginContact 发布)。
    // 原 Ori 在 ET.EventType.OnCollisionContact, 本项目按目录约定(Share→ET)放在 ET。
    public struct OnCollisionContact
    {
        public Contact contact;
        public bool isEnd;
    }
}
