namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_JoinRoomHandler : MessageSessionHandler<C2G_JoinRoom, G2C_JoinRoom>
    {
        protected override async ETTask Run(Session session, C2G_JoinRoom request, G2C_JoinRoom response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            if (player == null)
            {
                response.Error = ErrorCode.ERR_PlayerNotFound;
                response.Message = "Player not found";
                return;
            }

            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;

            G2Match_JoinRoom g2MatchJoinRoom = G2Match_JoinRoom.Create();
            g2MatchJoinRoom.PlayerId = player.Id;
            g2MatchJoinRoom.RoomId = request.RoomId;
            g2MatchJoinRoom.Password = request.Password;

            Match2G_JoinRoom match2GJoinRoom = await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId, g2MatchJoinRoom) as Match2G_JoinRoom;

            response.Error = match2GJoinRoom.Error;
            response.Message = match2GJoinRoom.Message;
            response.RoomInfo = match2GJoinRoom.RoomInfo;
        }
    }
}