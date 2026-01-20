namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_LeaveRoomHandler : MessageSessionHandler<C2G_LeaveRoom, G2C_LeaveRoom>
    {
        protected override async ETTask Run(Session session, C2G_LeaveRoom request, G2C_LeaveRoom response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            if (player == null)
            {
                response.Error = ErrorCode.ERR_PlayerNotFound;
                response.Message = "Player not found";
                return;
            }

            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;

            G2Match_LeaveRoom g2MatchLeaveRoom = G2Match_LeaveRoom.Create();
            g2MatchLeaveRoom.PlayerId = player.Id;

            Match2G_LeaveRoom match2GLeaveRoom = await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId, g2MatchLeaveRoom) as Match2G_LeaveRoom;

            response.Error = match2GLeaveRoom.Error;
            response.Message = match2GLeaveRoom.Message;
        }
    }
}