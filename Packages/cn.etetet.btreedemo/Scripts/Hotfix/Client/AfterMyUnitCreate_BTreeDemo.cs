namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterMyUnitCreate_BTreeDemo : AEvent<Scene, AfterMyUnitCreate>
    {
        protected override async ETTask Run(Scene scene, AfterMyUnitCreate args)
        {
            Scene root = scene.Root();
            // 仅在 Demo 场景下挂载示例行为树，避免污染正式玩法；默认不会触发（无场景使用 SceneType.Demo）
            if (root.SceneType != SceneType.Demo)
            {
                await ETTask.CompletedTask;
                return;
            }

            Unit unit = args.Unit;
            if (unit == null || unit.IsDisposed)
            {
                await ETTask.CompletedTask;
                return;
            }

            EntityRef<Unit> unitRef = unit;

            byte[] behaviorTreeBytes = await BTreeLoader.Instance.LoadBytesAsync("AITest");
            if (behaviorTreeBytes == null || behaviorTreeBytes.Length == 0)
            {
                await ETTask.CompletedTask;
                return;
            }

            // await 之后必须通过 EntityRef 重新获取 Entity
            unit = unitRef;
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            BTreeComponent behaviorTreeComponent = unit.GetComponent<BTreeComponent>();
            if (behaviorTreeComponent == null)
            {
                unit.AddComponent<BTreeComponent, byte[], string>(behaviorTreeBytes, "AITest");
            }
            else
            {
                behaviorTreeComponent.Reload(behaviorTreeBytes, "AITest");
            }

            await ETTask.CompletedTask;
        }
    }
}
