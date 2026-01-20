namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_JoinRoomHandler : MessageHandler<Scene, G2C_JoinRoom>
    {
        protected override async ETTask Run(Scene scene, G2C_JoinRoom message)
        {
            if (message.Error == ErrorCode.ERR_Success)
            {
                Log.Debug($"Join room success: {message.RoomInfo.RoomName}");
                await EventSystem.Instance.PublishAsync(scene, new JoinRoomSuccess()
                {
                    RoomInfo = message.RoomInfo
                });
            }
            else
            {
                Log.Error($"Join room failed: {message.Message}");
                await EventSystem.Instance.PublishAsync(scene, new JoinRoomFailed()
                {
                    Error = message.Error,
                    Message = message.Message
                });
            }
        }
    }
}