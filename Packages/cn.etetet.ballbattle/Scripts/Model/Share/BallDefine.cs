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
        // ball-battle 专属地图的 MapConfig 名(需在 Map.xlsx 配一张同名地图;
        // 进入该地图的单位才会被装配成球,普通 Map 不受影响)
        public const string BallMapName = "BallBattle";

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

        // —— 死亡 / 重生 ——
        public const int RespawnHp = 1000;       // 复活时的 HP(决定初始体型)
        public const int RespawnDelayMs = 2000;  // 死亡到复活的延迟(ms)
    }

    // 碰撞开始事件(由 CollisionListenerComponent.BeginContact 发布)。
    // 原 Ori 在 ET.EventType.OnCollisionContact, 本项目按目录约定(Share→ET)放在 ET。
    public struct OnCollisionContact
    {
        public Contact contact;
        public bool isEnd;
    }

    // 玩家球死亡事件(碰撞裁决在玩家被吞 / HP<=0 时发布)。
    // 服务端 BallPlayerDie_Respawn 订阅 → 延迟后在随机点以最小体型复活(持续可玩)。
    // 放在本包(不依赖 spell 的 UnitDie), 仅球球玩法内部使用。
    public struct BallPlayerDie
    {
        public EntityRef<Unit> Dead;   // 死亡的玩家球
        public EntityRef<Unit> Killer; // 击杀者(可空)
    }
}
