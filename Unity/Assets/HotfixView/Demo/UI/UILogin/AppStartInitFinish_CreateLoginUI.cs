

namespace ET
{
	public class AppStartInitFinish_RemoveLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await UIComponent.Instance.ShowWindowAsync(WindowID.WindowID_Login);
			//await UIHelper.Create(args.ZoneScene, UIType.UILogin);
		}
	}
}
