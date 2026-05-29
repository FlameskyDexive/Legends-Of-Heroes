namespace ET
{
    // 机器人球的简单 AI 标记(挂在机器人 Unit 上)。服务端权威驱动:
    // 定时思考 → 追最近的食物/更小的球、躲避更大的球、偶尔吐球。
    // 由 BallArenaComponent 在竞技场生成机器人时挂载。
    [ComponentOf(typeof(Unit))]
    public class RobotBallAIComponent : Entity, IAwake, IDestroy
    {
        public long ThinkTimer;
    }
}
