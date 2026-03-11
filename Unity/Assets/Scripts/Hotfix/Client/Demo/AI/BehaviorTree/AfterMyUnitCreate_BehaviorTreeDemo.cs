namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterMyUnitCreate_BehaviorTreeDemo : AEvent<Scene, AfterMyUnitCreate>
    {
        protected override async ETTask Run(Scene scene, AfterMyUnitCreate args)
        {
            Scene root = scene.Root();
            if (root.SceneType != SceneType.Demo)
            {
                await ETTask.CompletedTask;
                return;
            }

            Unit unit = args.unit;
            if (unit == null || unit.IsDisposed)
            {
                await ETTask.CompletedTask;
                return;
            }

            if (unit.GetComponent<XunLuoPathComponent>() == null)
            {
                unit.AddComponent<XunLuoPathComponent>();
            }

            byte[] behaviorTreeBytes = await BehaviorTreeLoader.Instance.LoadBytesAsync("AITest");
            if (behaviorTreeBytes == null || behaviorTreeBytes.Length == 0)
            {
                await ETTask.CompletedTask;
                return;
            }

            BehaviorTreeComponent behaviorTreeComponent = unit.GetComponent<BehaviorTreeComponent>();
            if (behaviorTreeComponent == null)
            {
                unit.AddComponent<BehaviorTreeComponent, byte[], string>(behaviorTreeBytes, "AITest");
            }
            else
            {
                behaviorTreeComponent.Reload(behaviorTreeBytes, "AITest");
            }

            await ETTask.CompletedTask;
        }
    }
}
