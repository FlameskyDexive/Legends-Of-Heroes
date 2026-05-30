namespace ET
{
    // 球球竞技场(挂 Map 场景):装配 2D 碰撞世界 + 驱动食物刷新。
    // 把它加到一张 ball-battle 的 Map 场景上, 即引导起整套吞噬/碰撞/食物系统。
    // 下列字段默认值仅作缺表回退;BallArenaComponentSystem.Awake 会从 Luban 单例表 BallConfig(Id=1) 覆盖。
    [ComponentOf(typeof(Scene))]
    public class BallArenaComponent : Entity, IAwake, IDestroy
    {
        public int FoodConfigId = 1102; // 食物球 UnitConfig(Unit.xlsx 中 1102 BallFood;0 表示不刷)
        public int MaxFoodCount = 80;  // 场上食物上限(密度: 80 个铺在 50x50 边界内, 玩家周围可见)
        public int FoodHp = 50;        // 每个食物的 HP(质量)
        public float MapMin = -25f;    // 地图边界(X/Z 平面);边界小一点食物更密、更易看到(玩家约在原点)
        public float MapMax = 25f;
        public long SpawnTimerId;
        public long LeaderboardTimerId; // 排行榜日志定时器(服务端结算)
    }
}
