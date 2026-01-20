namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetRoomListHandler : MessageSessionHandler<C2G_GetRoomList, G2C_GetRoomList>
    {
        protected override async ETTask Run(Session session, C2G_GetRoomList request, G2C_GetRoomList response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            if (player == null)
            {
                response.Error = ErrorCode.ERR_PlayerNotFound;
                response.Message = "Player not found";
                return;
            }

            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;

            G2Match_GetRoomList g2MatchGetRoomList = G2Match_GetRoomList.Create();
            g2MatchGetRoomList.Mode = request.Mode;

            Match2G_GetRoomList match2GGetRoomList = await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId, g2MatchGetRoomList) as Match2G_GetRoomList;

            response.Error = match2GGetRoomList.Error;
            response.Message = match2GGetRoomList.Message;
            response.RoomList = match2GGetRoomList.RoomList;
        }
    }
}