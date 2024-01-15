namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class Match2G_StateSyncRefreshMatchHandler : MessageHandler<Scene, Match2G_StateSyncRefreshMatch>
    {
        protected override async ETTask Run(Scene root, Match2G_StateSyncRefreshMatch message)
        {
            //收到房间消息，通知刷新UI
            await EventSystem.Instance.PublishAsync(root, new StateSyncRefreshMatch{RoomInfo = message.RoomInfo});
        }
    }
}