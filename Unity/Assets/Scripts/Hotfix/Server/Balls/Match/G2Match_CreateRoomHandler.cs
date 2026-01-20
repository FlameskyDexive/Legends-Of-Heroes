namespace ET.Server
{
    [MessageHandler(SceneType.Match)]
    public class G2Match_CreateRoomHandler : MessageHandler<Scene, G2Match_CreateRoom, Match2G_CreateRoom>
    {
        protected override async ETTask Run(Scene scene, G2Match_CreateRoom request, Match2G_CreateRoom response)
        {
            MatchComponent matchComponent = scene.GetComponent<MatchComponent>();
            if (matchComponent == null)
            {
                response.Error = ErrorCode.ERR_NotFoundComponent;
                response.Message = "MatchComponent not found";
                return;
            }

            long playerId = request.PlayerId;

            if (matchComponent.waitMatchStateSyncPlayers.Contains(playerId))
            {
                response.Error = ErrorCode.ERR_AlreadyInMatching;
                response.Message = "Player is already in matching";
                return;
            }

            if (string.IsNullOrEmpty(request.RoomName))
            {
                request.RoomName = $"Room{playerId}";
            }

            if (request.MaxPlayers <= 0 || request.MaxPlayers > 10)
            {
                request.MaxPlayers = 2;
            }

            StateSyncRoomManagerComponent roomManagerComponent = scene.GetComponent<StateSyncRoomManagerComponent>();
            if (roomManagerComponent == null)
            {
                roomManagerComponent = scene.AddComponent<StateSyncRoomManagerComponent>();
            }

            StateSyncRoom room = roomManagerComponent.CreateRoom(
                request.RoomName,
                request.Mode,
                request.MaxPlayers,
                request.Password,
                playerId
            );

            if (room == null)
            {
                response.Error = ErrorCode.ERR_CreateRoomFailed;
                response.Message = "Failed to create room";
                return;
            }

            response.Error = ErrorCode.ERR_Success;
            response.Message = "Create room success";
            response.RoomInfo = roomManagerComponent.GetRoomInfo(room.RoomId);
        }
    }
}