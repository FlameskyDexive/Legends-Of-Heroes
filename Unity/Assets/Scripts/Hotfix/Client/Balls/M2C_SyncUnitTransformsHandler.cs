namespace ET.Client
{
	[MessageHandler(SceneType.Demo)]
	public class M2C_SyncUnitTransformsHandler : MessageHandler<Scene, M2C_SyncUnitTransforms>
	{
		protected override async ETTask Run(Scene root, M2C_SyncUnitTransforms message)
        {
            Scene currentScene = root.CurrentScene();
            if (message?.TransformInfos?.Count > 0)
            {
                foreach (TransformInfo info in message.TransformInfos)
                {
                    Unit unit = currentScene.GetComponent<UnitComponent>().Get(info.UnitId);
                    if (unit != null)
                    {
                        unit.Forward = info.Forward;
                        unit.Position = info.Position;
                    }
                }
                

            }
			await ETTask.CompletedTask;
		}
	}
}
