namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class Room2C_StateSyncEnterMapHandler : MessageHandler<Scene, Room2C_StateSyncStart>
    {
        protected override async ETTask Run(Scene root, Room2C_StateSyncStart message)
        {
            root.GetComponent<ObjectWait>().Notify(new WaitType.Wait_Room2C_StateSyncStart() {Message = message});
            await ETTask.CompletedTask;
        }
    }
}