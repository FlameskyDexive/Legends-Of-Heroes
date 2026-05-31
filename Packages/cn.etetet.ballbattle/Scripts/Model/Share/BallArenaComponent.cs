namespace ET
{
    // 球球竞技场(挂 Map 场景):装配 2D 碰撞世界 + 驱动食物刷新。
    // 把它加到一张 ball-battle 的 Map 场景上, 即引导起整套吞噬/碰撞/食物系统。
    // 下列字段默认值仅作缺表回退;BallArenaComponentSystem.Awake 会从 Luban 单例表 BallConfig(Id=1) 覆盖。
    [ComponentOf(typeof(Scene))]
    public class BallArenaComponent : Entity, IAwake, IDestroy
    {
        public int FoodConfigId = 1102; // 食物球 UnitConfig(Unit.xlsx 中 1102 BallFood;0 表示不刷)
        public int MaxFoodCount = 600; // 场上食物上限(铺满 100x100 边界;按需求密度翻倍 300→600)
        // 食物直径随机区间:每个食物随机一个直径(视觉=碰撞,同由 HP→Radius 驱动),HP 反算自直径(也是吃它的 HP 增量,越大越值)。
        public float FoodMinDiameter = 0.1f;
        public float FoodMaxDiameter = 0.3f;
        // 地图边界(X/Z 平面),取自 BallArena 场景 Map/Plane 的实际占地:Plane 本地缩放(10,1,10)×内置 10x10 网格、父物体 Map 在原点 scale1
        // → 世界 100x100、居中原点 → 边界 ±50。角色移动 clamp 到此范围,食物也在此范围内随机生成。
        public float MapMin = -50f;
        public float MapMax = 50f;
        public long SpawnTimerId;
        public long LeaderboardTimerId; // 排行榜日志定时器(服务端结算)
    }
}
