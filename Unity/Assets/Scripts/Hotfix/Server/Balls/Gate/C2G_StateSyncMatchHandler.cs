namespace ET.Server
{
	[MessageSessionHandler(SceneType.Gate)]
	public class C2G_StateSyncMatchHandler : MessageSessionHandler<C2G_StateSyncMatch, G2C_StateSyncMatch>
	{
		protected override async ETTask Run(Session session, C2G_StateSyncMatch request, G2C_StateSyncMatch response)
		{
			Player player = session.GetComponent<SessionPlayerComponent>().Player;

			StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;

            G2Match_StateSyncMatch g2MatchMatch = G2Match_StateSyncMatch.Create();
            g2MatchMatch.Id = player.Id;
            await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId, g2MatchMatch);
        }
	}
}