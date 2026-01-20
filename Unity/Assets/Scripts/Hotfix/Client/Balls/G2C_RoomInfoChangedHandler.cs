namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_RoomInfoChangedHandler : MessageHandler<Scene, G2C_RoomInfoChanged>
    {
        protected override async ETTask Run(Scene scene, G2C_RoomInfoChanged message)
        {
            Log.Debug($"Room info changed: {message.RoomInfo.RoomName}");
            await EventSystem.Instance.PublishAsync(scene, new RoomInfoChanged()
            {
                RoomInfo = message.RoomInfo
            });
        }
    }
}