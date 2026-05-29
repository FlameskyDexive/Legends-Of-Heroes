namespace ET.Server
{
    // 吐球技能动作:由 skill 时间轴在 EActionEventType.BallSpit 事件触发(SkillConfig 1001 → ActionEventConfig 1)。
    [ActionEvent(SceneType.Map, EActionEventType.BallSpit)]
    public class ActionEventBallSpit : IActionEvent
    {
        public void Run(ActionEvent actionEvent, ActionEventData args)
        {
            Unit owner = args.owner;
            if (owner == null || owner.IsDisposed)
            {
                return;
            }
            owner.SpitBall();
        }
    }
}
