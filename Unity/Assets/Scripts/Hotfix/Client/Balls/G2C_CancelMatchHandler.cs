namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_CancelMatchHandler : MessageHandler<Scene, G2C_CancelMatch>
    {
        protected override async ETTask Run(Scene scene, G2C_CancelMatch message)
        {
            if (message.Error == ErrorCode.ERR_Success)
            {
                Log.Debug("Cancel match success");
                await EventSystem.Instance.PublishAsync(scene, new CancelMatchSuccess());
            }
            else
            {
                Log.Error($"Cancel match failed: {message.Message}");
                await EventSystem.Instance.PublishAsync(scene, new CancelMatchFailed()
                {
                    Error = message.Error,
                    Message = message.Message
                });
            }
        }
    }
}