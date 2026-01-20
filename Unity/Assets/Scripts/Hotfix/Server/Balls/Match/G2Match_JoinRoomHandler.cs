namespace ET.Server
{
    [MessageHandler(SceneType.Match)]
    public class G2Match_JoinRoomHandler : MessageHandler<Scene, G2Match_JoinRoom, Match2G_JoinRoom>
    {
        protected override async ETTask Run(Scene scene, G2Match_JoinRoom request, Match2G_JoinRoom response)
        {
            MatchComponent matchComponent = scene.GetComponent<MatchComponent>();
            if (matchComponent == null)
            {
                response.Error = ErrorCode.ERR_NotFoundComponent;
                response.Message = "MatchComponent not found";
                return;
            }

            StateSyncRoomManagerComponent roomManagerComponent = scene.GetComponent<StateSyncRoomManagerComponent>();
            if (roomManagerComponent == null)
            {
                response.Error = ErrorCode.ERR_RoomManagerNotFound;
                response.Message = "RoomManager not found";
                return;
            }

            long playerId = request.PlayerId;

            if (matchComponent.waitMatchStateSyncPlayers.Contains(playerId))
            {
                response.Error = ErrorCode.ERR_AlreadyInMatching;
                response.Message = "Player is already in matching";
                return;
            }

            if (roomManagerComponent.PlayerToRoom.ContainsKey(playerId))
            {
                response.Error = ErrorCode.ERR_AlreadyInRoom;
                response.Message = "Player is already in a room";
                return;
            }

            StateSyncRoom room = roomManagerComponent.GetRoom(request.RoomId);
            if (room == null)
            {
                response.Error = ErrorCode.ERR_RoomNotFound;
                response.Message = "Room not found";
                return;
            }

            if (!string.IsNullOrEmpty(room.Password) && room.Password != request.Password)
            {
                response.Error = ErrorCode.ERR_RoomPasswordWrong;
                response.Message = "Room password is wrong";
                return;
            }

            bool success = roomManagerComponent.JoinRoom(request.RoomId, playerId);
            if (!success)
            {
                response.Error = ErrorCode.ERR_JoinRoomFailed;
                response.Message = "Failed to join room";
                return;
            }

            matchComponent.waitMatchStateSyncPlayers.Add(playerId);

            response.Error = ErrorCode.ERR_Success;
            response.Message = "Join room success";
            response.RoomInfo = roomManagerComponent.GetRoomInfo(request.RoomId);
        }
    }
}