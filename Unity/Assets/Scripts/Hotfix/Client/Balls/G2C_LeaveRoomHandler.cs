namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_LeaveRoomHandler : MessageHandler<Scene, G2C_LeaveRoom>
    {
        protected override async ETTask Run(Scene scene, G2C_LeaveRoom message)
        {
            if (message.Error == ErrorCode.ERR_Success)
            {
                Log.Debug("Leave room success");
                await EventSystem.Instance.PublishAsync(scene, new LeaveRoomSuccess());
            }
            else
            {
                Log.Error($"Leave room failed: {message.Message}");
                await EventSystem.Instance.PublishAsync(scene, new LeaveRoomFailed()
                {
                    Error = message.Error,
                    Message = message.Message
                });
            }
        }
    }
}