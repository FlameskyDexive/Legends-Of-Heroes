
namespace ET
{
    [Event(SceneType.Map)]
    public class OnCollisionContactHandler: AEvent<EventType.OnCollisionContact>
    {
        protected override async ETTask Run(Scene scene, EventType.OnCollisionContact args)
        {
            
            
            await ETTask.CompletedTask;
        }
    }
}