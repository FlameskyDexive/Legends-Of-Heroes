using System;


namespace ET.Server
{
	[MessageHandler(SceneType.Gate)]
	public class C2G_LoginGateHandler : AMRpcHandler<C2G_LoginGate, G2C_LoginGate>
	{
		protected override async ETTask Run(Session session, C2G_LoginGate request, G2C_LoginGate response)
		{
			Scene scene = session.DomainScene();
			string account = scene.GetComponent<GateSessionKeyComponent>().Get(request.Key);
			if (account == null)
			{
				response.Error = ErrorCore.ERR_ConnectGateKeyError;
				response.Message = "Gate key验证失败!";
				return;
			}
			
			session.RemoveComponent<SessionAcceptTimeoutComponent>();
            StartSceneConfig config = StartSceneConfigCategory.Instance.GetBySceneName(scene.Zone, "Lobby");

            PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();
			Player player = playerComponent.AddChild<Player, string>(account);
            player.PlayerName = $"Player_{player.Id}";
            //新用户随机分配一个头像，后续可以自己在个人信息里头修改名字、头像
            player.AvatarIndex = RandomGenerator.RandomNumber(0, 10);

            player.AddComponent<PlayerSessionComponent>().Session = session;
            player.AddComponent<MailBoxComponent, MailboxType>(MailboxType.GateSession);
            await player.AddLocation(LocationType.Player);

            session.AddComponent<SessionPlayerComponent>().Player = player;

            response.PlayerId = player.Id;

            response.AvatarIndex = player.AvatarIndex;
            response.PlayerName = player.PlayerName;
            response.LobbyActorId = config.InstanceId;
			await ETTask.CompletedTask;
		}
	}
}