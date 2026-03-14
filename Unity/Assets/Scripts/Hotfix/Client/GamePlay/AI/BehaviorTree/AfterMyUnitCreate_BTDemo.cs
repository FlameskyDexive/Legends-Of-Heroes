namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterMyUnitCreate_BTDemo : AEvent<Scene, AfterMyUnitCreate>
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

            byte[] behaviorTreeBytes = await BTLoader.Instance.LoadBytesAsync("AITest");
            if (behaviorTreeBytes == null || behaviorTreeBytes.Length == 0)
            {
                await ETTask.CompletedTask;
                return;
            }

            BTComponent behaviorTreeComponent = unit.GetComponent<BTComponent>();
            if (behaviorTreeComponent == null)
            {
                unit.AddComponent<BTComponent, byte[], string>(behaviorTreeBytes, "AITest");
            }
            else
            {
                behaviorTreeComponent.Reload(behaviorTreeBytes, "AITest");
            }

            await ETTask.CompletedTask;
        }
    }
}
