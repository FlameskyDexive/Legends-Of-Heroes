using ET.EventType;

namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class G2C_UpdateRoomPlayersHandler : AMHandler<G2C_UpdateRoomPlayers>
    {
        protected override async ETTask Run(Session session, G2C_UpdateRoomPlayers message)
        {
            Log.Info($"receive broadcast room players update, {message?.roomInfo?.playerInfoRoom?.Count}");
            EventSystem.Instance.Publish(session.DomainScene(), new UpdateRoomPlayers(){roomPlayersProto = message});
            await ETTask.CompletedTask;
        }
    }
}