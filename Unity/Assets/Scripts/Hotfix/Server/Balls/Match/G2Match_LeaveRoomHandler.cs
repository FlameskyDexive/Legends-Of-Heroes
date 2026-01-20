namespace ET.Server
{
    [MessageHandler(SceneType.Match)]
    public class G2Match_LeaveRoomHandler : MessageHandler<Scene, G2Match_LeaveRoom, Match2G_LeaveRoom>
    {
        protected override async ETTask Run(Scene scene, G2Match_LeaveRoom request, Match2G_LeaveRoom response)
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

            if (!roomManagerComponent.PlayerToRoom.ContainsKey(playerId))
            {
                response.Error = ErrorCode.ERR_NotInRoom;
                response.Message = "Player is not in a room";
                return;
            }

            long roomId = roomManagerComponent.GetPlayerRoomId(playerId);
            bool success = roomManagerComponent.LeaveRoom(playerId);

            if (!success)
            {
                response.Error = ErrorCode.ERR_LeaveRoomFailed;
                response.Message = "Failed to leave room";
                return;
            }

            matchComponent.waitMatchStateSyncPlayers.Remove(playerId);

            response.Error = ErrorCode.ERR_Success;
            response.Message = "Leave room success";
        }
    }
}