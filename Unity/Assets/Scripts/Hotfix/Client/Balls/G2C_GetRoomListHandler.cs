namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_GetRoomListHandler : MessageHandler<Scene, G2C_GetRoomList>
    {
        protected override async ETTask Run(Scene scene, G2C_GetRoomList message)
        {
            if (message.Error == ErrorCode.ERR_Success)
            {
                Log.Debug($"Get room list success: {message.RoomList.Count} rooms");
                await EventSystem.Instance.PublishAsync(scene, new GetRoomListSuccess()
                {
                    RoomList = message.RoomList
                });
            }
            else
            {
                Log.Error($"Get room list failed: {message.Message}");
                await EventSystem.Instance.PublishAsync(scene, new GetRoomListFailed()
                {
                    Error = message.Error,
                    Message = message.Message
                });
            }
        }
    }
}