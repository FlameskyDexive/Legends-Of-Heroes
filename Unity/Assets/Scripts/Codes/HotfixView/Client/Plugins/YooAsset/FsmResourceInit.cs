namespace ET.Client
{
    [FsmNode]
    public class FsmResourceInit: AFsmNodeHandler
    {
        public override async ETTask OnEnter(FsmComponent fsmComponent)
        {
            fsmComponent.Transition(nameof(FsmUpdateStaticVersion));
            await ETTask.CompletedTask;
        }
    }
}