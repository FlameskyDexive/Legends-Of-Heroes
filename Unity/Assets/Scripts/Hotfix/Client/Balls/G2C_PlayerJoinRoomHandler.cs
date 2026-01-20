namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_PlayerJoinRoomHandler : MessageHandler<Scene, G2C_PlayerJoinRoom>
    {
        protected override async ETTask Run(Scene scene, G2C_PlayerJoinRoom message)
        {
            Log.Debug($"Player join room: {message.PlayerInfo.PlayerId}");
            await EventSystem.Instance.PublishAsync(scene, new PlayerJoinRoom()
            {
                PlayerInfo = message.PlayerInfo
            });
        }
    }
}