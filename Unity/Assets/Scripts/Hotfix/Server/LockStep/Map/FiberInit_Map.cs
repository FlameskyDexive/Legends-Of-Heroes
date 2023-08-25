using System.Net;

namespace ET.Server
{
    [Invoke((long)SceneType.Map)]
    public class FiberInit_Map: AInvokeHandler<FiberInit, ETTask>
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
            root.AddComponent<CollisionListenerComponent>();
            root.AddComponent<CollisionWorldComponent>();
            root.AddComponent<AOIManagerComponent>();
            root.AddComponent<RoomManagerComponent>();
            root.AddComponent<LocationProxyComponent>();
            root.AddComponent<MessageLocationSenderComponent>();

            await ETTask.CompletedTask;
        }
    }
}