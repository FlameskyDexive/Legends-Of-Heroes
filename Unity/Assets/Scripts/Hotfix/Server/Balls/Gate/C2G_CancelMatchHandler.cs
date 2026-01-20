namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_CancelMatchHandler : MessageSessionHandler<C2G_CancelMatch, G2C_CancelMatch>
    {
        protected override async ETTask Run(Session session, C2G_CancelMatch request, G2C_CancelMatch response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            if (player == null)
            {
                response.Error = ErrorCode.ERR_PlayerNotFound;
                response.Message = "Player not found";
                return;
            }

            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;

            G2Match_CancelMatch g2MatchCancelMatch = G2Match_CancelMatch.Create();
            g2MatchCancelMatch.PlayerId = player.Id;

            Match2G_CancelMatch match2GCancelMatch = await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId, g2MatchCancelMatch) as Match2G_CancelMatch;

            response.Error = match2GCancelMatch.Error;
            response.Message = match2GCancelMatch.Message;
        }
    }
}