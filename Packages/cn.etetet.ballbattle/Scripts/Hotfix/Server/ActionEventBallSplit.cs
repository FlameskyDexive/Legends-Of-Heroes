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
            // 多态配置: 分裂参数(分裂下限HP/冲刺距离)从 ActionEventParams_BallSplit 取
            ActionEventParams_BallSplit p = actionEvent.ActionEventConfig.Params as ActionEventParams_BallSplit;
            if (p == null)
            {
                return;
            }
            owner.SplitBall(p.MinHp, p.Range);
        }
    }
}
