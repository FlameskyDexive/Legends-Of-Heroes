namespace ET.Client
{
    [FsmNode]
    public class FsmPatchDone: AFsmNodeHandler
    {
        public override async ETTask OnEnter(FsmComponent fsmComponent)
        {
            Scene clientScene = fsmComponent.GetParent<Scene>();
            clientScene.RemoveComponent<FsmComponent>();

            await ETTask.CompletedTask;
        }
    }
}