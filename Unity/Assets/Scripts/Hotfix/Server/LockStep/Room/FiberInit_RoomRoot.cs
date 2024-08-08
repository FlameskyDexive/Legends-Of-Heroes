using System.Net;

namespace ET.Server
{
    [Invoke((long)SceneType.RoomRoot)]
    public class FiberInit_RoomRoot: AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();
            root.AddComponent<UnitComponent>();
            root.AddComponent<AOIManagerComponent>();
            root.AddComponent<CollisionListenerComponent>();
            root.AddComponent<CollisionWorldComponent>();
            root.AddComponent<LocationProxyComponent>();
            root.AddComponent<MessageLocationSenderComponent>();

            await ETTask.CompletedTask;
        }
    }
}