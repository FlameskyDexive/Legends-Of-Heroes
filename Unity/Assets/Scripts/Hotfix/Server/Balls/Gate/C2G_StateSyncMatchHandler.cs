namespace ET.Server
{
	[MessageSessionHandler(SceneType.Gate)]
	public class C2G_StateSyncMatchHandler : MessageSessionHandler<C2G_StateSyncMatch, G2C_StateSyncMatch>
	{
		protected override async ETTask Run(Session session, C2G_StateSyncMatch request, G2C_StateSyncMatch response)
		{
			Player player = session.GetComponent<SessionPlayerComponent>().Player;

			StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;

			await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId,
				new G2Match_StateSyncMatch() { Id = player.Id });
		}
	}
}