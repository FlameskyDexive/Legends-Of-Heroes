namespace ET
{
    // 球球竞技场(挂 Map 场景):装配 2D 碰撞世界 + 驱动食物刷新。
    // 把它加到一张 ball-battle 的 Map 场景上, 即引导起整套吞噬/碰撞/食物系统。
    // 下列字段默认值仅作缺表回退;BallArenaComponentSystem.Awake 会从 Luban 单例表 BallConfig(Id=1) 覆盖。
    [ComponentOf(typeof(Scene))]
    public class BallArenaComponent : Entity, IAwake, IDestroy
    {
        public int FoodConfigId = 1102; // 食物球 UnitConfig(Unit.xlsx 中 1102 BallFood;0 表示不刷)
        public int MaxFoodCount = 50;  // 场上食物上限(BallConfig.MaxFoodCount)
        public int FoodHp = 50;        // 每个食物的 HP(质量)(BallConfig.FoodHp)
        public float MapMin = -50f;    // 地图边界(X/Z 平面)(BallConfig.MapMin)
        public float MapMax = 50f;     // (BallConfig.MapMax)
        public long SpawnTimerId;
    }
}
