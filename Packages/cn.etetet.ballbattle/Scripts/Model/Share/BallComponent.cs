namespace ET
{
    // 标记一个 Unit 的球类型(玩家/食物/子弹),用于碰撞时区分处理。
    // 本项目 UnitType 无 Food/Bullet, 故用此组件区分球球玩法语义。
    [ComponentOf(typeof(Unit))]
    public class BallComponent : Entity, IAwake<EBallType>
    {
        public EBallType BallType;
    }
}
