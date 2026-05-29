namespace ET
{
    // 球球竞技场(挂 Map 场景):装配 2D 碰撞世界 + 驱动食物刷新。
    // 把它加到一张 ball-battle 的 Map 场景上, 即引导起整套吞噬/碰撞/食物系统。
    // 数值正式应来自配置表 BallConfig, 这里先给默认值。
    [ComponentOf(typeof(Scene))]
    public class BallArenaComponent : Entity, IAwake, IDestroy
    {
        public int FoodConfigId = 0;   // 食物球的 UnitConfigId(需在 UnitConfig 配置;0 表示未配则不刷)
        public int MaxFoodCount = 50;  // 场上食物上限
        public int FoodHp = 50;        // 每个食物的 HP(质量)
        public float MapMin = -50f;    // 地图边界(X/Z 平面)
        public float MapMax = 50f;
        public long SpawnTimerId;
    }
}
