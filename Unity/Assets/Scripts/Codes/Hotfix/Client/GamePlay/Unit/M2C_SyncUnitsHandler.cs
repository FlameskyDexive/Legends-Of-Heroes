namespace ET.Client
{
	[MessageHandler(SceneType.Client)]
	public class M2C_SyncUnitsHandler : AMHandler<M2C_SyncUnits>
	{
		protected override async ETTask Run(Session session, M2C_SyncUnits message)
		{
			Scene currentScene = session.DomainScene().CurrentScene();
			UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
			Log.Info($"create units:{message?.Units?.Count}");
			foreach (UnitInfo unitInfo in message.Units)
			{
                Unit unit = unitComponent.Get(unitInfo.UnitId);
				if(unit == null)
                    continue;
                unit.SyncInfo(unitInfo);
            }
			await ETTask.CompletedTask;
		}
	}
}
