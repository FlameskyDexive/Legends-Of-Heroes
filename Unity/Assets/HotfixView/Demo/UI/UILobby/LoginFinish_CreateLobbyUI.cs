

namespace ET
{
	public class LoginFinish_CreateLobbyUI: AEvent<EventType.LoginFinish>
	{
		protected override async ETTask Run(EventType.LoginFinish args)
		{
			await UIComponent.Instance?.ShowWindowAsync(WindowID.WindowID_Lobby);
			//await UIHelper.Create(args.ZoneScene, UIType.UILobby);
		}
	}
}
