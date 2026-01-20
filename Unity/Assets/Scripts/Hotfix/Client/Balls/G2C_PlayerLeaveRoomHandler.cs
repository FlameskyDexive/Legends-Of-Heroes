namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_PlayerLeaveRoomHandler : MessageHandler<Scene, G2C_PlayerLeaveRoom>
    {
        protected override async ETTask Run(Scene scene, G2C_PlayerLeaveRoom message)
        {
            Log.Debug($"Player leave room: {message.PlayerId}");
            await EventSystem.Instance.PublishAsync(scene, new PlayerLeaveRoom()
            {
                PlayerId = message.PlayerId
            });
        }
    }
}