namespace ET
{
    // 标记一个 Unit 的球类型(玩家/食物/子弹),用于碰撞时区分处理。
    // 本项目 UnitType 无 Food/Bullet, 故用此组件区分球球玩法语义。
    [ComponentOf(typeof(Unit))]
    public class BallComponent : Entity, IAwake<EBallType>
    {
        public EBallType BallType;

        // 计分(仅玩家球有意义):累计击杀数 / 死亡数。用于结算/排行榜(服务端权威)。
        public int Kills;
        public int Deaths;

        // 子弹球归属:发射该子弹的玩家 UnitId(子弹击杀时反查射手记功;非子弹为 0)。
        public long ShooterId;
    }
}
