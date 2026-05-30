namespace ET
{
	/// <summary>
	/// 技能发射子弹(通用)。
	/// 注:原实现依赖 Server.UnitFactory.CreateBullet —— 该方法全仓库不存在,且 Share 层禁止依赖 ET.Server(ET0113),
	/// 故此通用事件未实现。球球大作战的"吐球"子弹走 ballbattle 的 ActionEventBallSpit → BallSkillHelper.EjectBall(服务端),
	/// 不经此通用事件。此处保留 [ActionEvent] 注册以使 EActionEventType.Bullet 派发不缺项,Run 为安全空桩。
	/// </summary>
	[ActionEvent(SceneType.Map, EActionEventType.Bullet)]
	[FriendOf(typeof(ActionEvent))]
	public class ActionEventBullet : IActionEvent
	{
		public void Run(ActionEvent actionEvent, ActionEventData args)
		{
			Unit owner = args.owner;
			if (owner == null)
			{
				return;
			}
			Log.Info("ActionEventBullet: 通用发射子弹事件未实现(球球大作战的吐球用 ActionEventBallSpit)");
		}
	}
}
