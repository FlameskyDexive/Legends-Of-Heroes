

namespace ET
{
	public class LoginFinish_RemoveLoginUI: AEvent<EventType.LoginFinish>
	{
		protected override async ETTask Run(EventType.LoginFinish args)
		{
			UIComponent.Instance?.CloseWindow(WindowID.WindowID_Login);
			await ETTask.CompletedTask;
			//await UIHelper.Remove(args.ZoneScene, UIType.UILogin);
		}
	}
}
