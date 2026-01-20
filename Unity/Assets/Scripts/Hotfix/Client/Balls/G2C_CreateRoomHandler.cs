namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_CreateRoomHandler : MessageHandler<Scene, G2C_CreateRoom>
    {
        protected override async ETTask Run(Scene scene, G2C_CreateRoom message)
        {
            if (message.Error == ErrorCode.ERR_Success)
            {
                Log.Debug($"Create room success: {message.RoomInfo.RoomName}");
                await EventSystem.Instance.PublishAsync(scene, new CreateRoomSuccess()
                {
                    RoomInfo = message.RoomInfo
                });
            }
            else
            {
                Log.Error($"Create room failed: {message.Message}");
                await EventSystem.Instance.PublishAsync(scene, new CreateRoomFailed()
                {
                    Error = message.Error,
                    Message = message.Message
                });
            }
        }
    }
}