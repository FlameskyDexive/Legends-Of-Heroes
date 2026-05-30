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

    // 球球玩法参数。结构性常量(地图名/Box2D 步进/配置 id/sqrt 下限)为 const;
    // 玩法调参(体型速度公式/吞噬/复活/食物/机器人)迁到 Luban 单例表 BallConfig(固定 Id=1),
    // 经下方静态属性读取——配置未加载/缺表时回退默认值,保证早期初始化与缺表都不崩。
    public static class BallDefine
    {
        // —— 结构性常量(不进配置表) ——
        // ball-battle 专属地图的 MapConfig 名(需在 Map.xlsx 配一张同名地图;
        // 进入该地图的单位才会被装配成球,普通 Map 不受影响)
        public const string BallMapName = "BallBattle";

        // Box2D 世界步进的固定时间步(本项目无 IFixedUpdate, 用 IUpdate + 固定 dt 步进)
        public const float FixedDeltaTime = 1f / 20f;

        // Box2D 求解迭代次数(纯做重叠检测, 迭代可较低)
        public const int VelocityIteration = 8;
        public const int PositionIteration = 3;

        // HP 下限,避免 sqrt(0)(体型/速度公式的结构性下限,不进调参表)
        public const float MinHp = 1f;

        // —— 技能 / 单位 配置 id(不进调参表) ——
        public const int SpitSkillId = 1001;     // 吐球技能 SkillConfig.Id(ActiveSkill 槽位 0)
        public const int SplitSkillId = 1002;    // 分裂技能 SkillConfig.Id(ActiveSkill 槽位 1)
        public const int BulletConfigId = 1102;  // 喷出球的 UnitConfig(复用食物球 Virtual 配置, SetupBall(Bullet) 覆盖其角色)
        public const int RobotConfigId = 1102;   // 机器人球 UnitConfig(复用食物球 Virtual 配置 + SetupBall(Player))

        // 机器人模拟"吐球"时传给 SpitBall 的参数(镜像 skill 配置 1001 → ActionEventParams_BallSpit;仅机器人 AI 用)。
        // 分裂参数(MinSplitHp/SplitRange)已迁入 skill 配置 ActionEventParams_BallSplit,以下保留兼容(当前无引用)。
        public const int SpitCost = 50;
        public const int SpitBulletHp = 30;
        public const float SpitRange = 30f;
        public const int MinSplitHp = 200;
        public const float SplitRange = 25f;

        // —— 玩法调参(常量;值与原 BallConfig 单例表默认完全一致,行为不变) ——
        // 注:Luban 单例表 BallConfig 的"配置驱动"暂停用——ET 禁止在静态上下文用 BallConfigCategory.Instance(ET0039),
        // 且静态可变字段需 [StaticField](ET0015)。如需恢复配置驱动,应在有 Entity/Fiber 上下文处用
        // self.GetSingleton<BallConfigCategory>().Get(1) 读取(参考 BallArenaComponentSystem),而非静态 BallDefine。

        // 体型/速度公式(质量=HP 驱动):Radius=RadiusCoef*sqrt(HP);Speed=clamp(BaseSpeed*SpeedCoef/sqrt(HP),MinSpeed,BaseSpeed)
        public const float RadiusCoef = 0.05f;
        public const float BaseSpeed = 5f;
        public const float SpeedCoef = 10f;
        public const float MinSpeed = 1.5f;

        // 吞噬:A 半径 > B 半径 * EatRatio → A 吃 B
        public const float EatRatio = 1.15f;

        // 子弹命中扣的 HP
        public const int BulletDamage = 100;

        // 死亡 / 重生
        public const int RespawnHp = 1000;
        public const int RespawnDelayMs = 2000;

        // 机器人 AI(服务端竞技场生成,服务端权威驱动)
        public const int RobotCount = 5;
        public const int RobotInitHp = 300;
        public const int RobotThinkMs = 300;
        public const float RobotLookahead = 20f;
        public const float RobotFleeRange = 12f;
        public const float RobotSpitRange = 10f;
        public const float RobotSpitChance = 0.15f;
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
