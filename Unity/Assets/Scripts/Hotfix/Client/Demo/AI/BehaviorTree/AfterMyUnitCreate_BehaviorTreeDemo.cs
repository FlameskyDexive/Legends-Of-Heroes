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

            if (unit.GetComponent<BehaviorTreeComponent>() == null)
            {
                unit.AddComponent<BehaviorTreeComponent, byte[], string>(BehaviorTreeClientDemoFactory.CreateRobotPatrolBytes(), "RobotPatrol");
            }

            await ETTask.CompletedTask;
        }
    }
}
