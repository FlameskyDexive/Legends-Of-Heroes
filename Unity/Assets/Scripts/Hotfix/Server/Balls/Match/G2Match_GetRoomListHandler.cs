namespace ET.Server
{
    [MessageHandler(SceneType.Match)]
    public class G2Match_GetRoomListHandler : MessageHandler<Scene, G2Match_GetRoomList, Match2G_GetRoomList>
    {
        protected override async ETTask Run(Scene scene, G2Match_GetRoomList request, Match2G_GetRoomList response)
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

            RoomMode? mode = null;
            if (request.Mode >= 0)
            {
                mode = (RoomMode)request.Mode;
            }

            List<RoomInfo> roomList = roomManagerComponent.GetRoomList(mode);

            response.Error = ErrorCode.ERR_Success;
            response.Message = "Get room list success";
            response.RoomList = roomList;
        }
    }
}