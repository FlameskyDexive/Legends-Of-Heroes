using ET.EventType;

namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class G2C_StartEnterMapHandler : AMHandler<G2C_StartEnterMap>
    {
        protected override async ETTask Run(Session session, G2C_StartEnterMap message)
        {
            Log.Info($"receive broadcast StartEnterMap");
            await EnterMapHelper.EnterMapAsync(session.ClientScene());

            await ETTask.CompletedTask;
        }
    }
}