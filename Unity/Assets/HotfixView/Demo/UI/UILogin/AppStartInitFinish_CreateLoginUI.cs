

namespace ET
{
	public class AppStartInitFinish_RemoveLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await ETTask.CompletedTask;
			 UIComponent.Instance.ShowWindow(WindowID.WindowID_Test);
			 //await UIHelper.Create(args.ZoneScene, UIType.UILogin);
		}
	}
}
