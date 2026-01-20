using System;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_CreateRoomHandler : MessageSessionHandler<C2G_CreateRoom, G2C_CreateRoom>
    {
        protected override async ETTask Run(Session session, C2G_CreateRoom request, G2C_CreateRoom response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            if (player == null)
            {
                response.Error = ErrorCode.ERR_PlayerNotFound;
                response.Message = "Player not found";
                return;
            }

            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;

            G2Match_CreateRoom g2MatchCreateRoom = G2Match_CreateRoom.Create();
            g2MatchCreateRoom.PlayerId = player.Id;
            g2MatchCreateRoom.RoomName = request.RoomName;
            g2MatchCreateRoom.Mode = request.Mode;
            g2MatchCreateRoom.MaxPlayers = request.MaxPlayers;
            g2MatchCreateRoom.Password = request.Password;

            Match2G_CreateRoom match2GCreateRoom = await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId, g2MatchCreateRoom) as Match2G_CreateRoom;

            response.Error = match2GCreateRoom.Error;
            response.Message = match2GCreateRoom.Message;
            response.RoomInfo = match2GCreateRoom.RoomInfo;
        }
    }
}