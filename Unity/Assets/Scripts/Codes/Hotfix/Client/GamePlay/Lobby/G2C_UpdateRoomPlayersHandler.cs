namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class G2C_UpdateRoomPlayersHandler : AMHandler<G2C_UpdateRoomPlayers>
    {
        protected override async ETTask Run(Session session, G2C_UpdateRoomPlayers message)
        {
            Log.Info($"receive broadcast room players update, {message?.playerInfoRoom?.Count}");
            await ETTask.CompletedTask;
        }
    }
}