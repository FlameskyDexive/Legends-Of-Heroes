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
            // 多态配置: 吐球参数(消耗HP/子弹HP/飞行距离)从 ActionEventParams_BallSpit 取
            ActionEventParams_BallSpit p = actionEvent.ActionEventConfig.Params as ActionEventParams_BallSpit;
            if (p == null)
            {
                return;
            }
            owner.SpitBall(p.CostHp, p.BulletHp, p.Range);
        }
    }
}
