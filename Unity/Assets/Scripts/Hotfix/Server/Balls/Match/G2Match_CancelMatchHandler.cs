namespace ET.Server
{
    [MessageHandler(SceneType.Match)]
    public class G2Match_CancelMatchHandler : MessageHandler<Scene, G2Match_CancelMatch, Match2G_CancelMatch>
    {
        protected override async ETTask Run(Scene scene, G2Match_CancelMatch request, Match2G_CancelMatch response)
        {
            MatchComponent matchComponent = scene.GetComponent<MatchComponent>();
            if (matchComponent == null)
            {
                response.Error = ErrorCode.ERR_NotFoundComponent;
                response.Message = "MatchComponent not found";
                return;
            }

            long playerId = request.PlayerId;

            if (!matchComponent.waitMatchStateSyncPlayers.Contains(playerId))
            {
                response.Error = ErrorCode.ERR_NotInMatching;
                response.Message = "Player is not in matching";
                return;
            }

            StateSyncRoomManagerComponent roomManagerComponent = scene.GetComponent<StateSyncRoomManagerComponent>();
            if (roomManagerComponent != null && roomManagerComponent.PlayerToRoom.ContainsKey(playerId))
            {
                long roomId = roomManagerComponent.GetPlayerRoomId(playerId);
                roomManagerComponent.LeaveRoom(playerId);
            }

            matchComponent.waitMatchStateSyncPlayers.Remove(playerId);

            response.Error = ErrorCode.ERR_Success;
            response.Message = "Cancel match success";
        }
    }
}