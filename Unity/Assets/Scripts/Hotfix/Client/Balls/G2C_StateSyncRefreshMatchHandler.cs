namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class G2C_StateSyncRefreshMatchHandler : MessageHandler<Scene, G2C_StateSyncRefreshMatch>
    {
        protected override async ETTask Run(Scene root, G2C_StateSyncRefreshMatch message)
        {
            //收到房间消息，通知刷新UI
            await EventSystem.Instance.PublishAsync(root, new StateSyncRefreshMatch{ RoomInfo = message.RoomInfo});
        }
    }
}