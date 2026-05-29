namespace ET.Server
{
    // 分裂技能动作:由 skill 时间轴在 EActionEventType.BallSplit 事件触发(SkillConfig 1002 → ActionEventConfig 2)。
    [ActionEvent(SceneType.Map, EActionEventType.BallSplit)]
    public class ActionEventBallSplit : IActionEvent
    {
        public void Run(ActionEvent actionEvent, ActionEventData args)
        {
            Unit owner = args.owner;
            if (owner == null || owner.IsDisposed)
            {
                return;
            }
            owner.SplitBall();
        }
    }
}
