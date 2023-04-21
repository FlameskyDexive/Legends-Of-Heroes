namespace ET.Server
{
	[MessageHandler(SceneType.Gate)]
	public class C2G_EnterMapHandler : AMRpcHandler<C2G_EnterMap, G2C_EnterMap>
	{
		protected override async ETTask Run(Session session, C2G_EnterMap request, G2C_EnterMap response)
		{
			Player player = session.GetComponent<SessionPlayerComponent>().Player;

			// 在Gate上动态创建一个Map Scene，把Unit从DB中加载放进来，然后传送到真正的Map中，这样登陆跟传送的逻辑就完全一样了
			GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
			gateMapComponent.Scene = await SceneFactory.CreateServerScene(gateMapComponent, player.Id, IdGenerater.Instance.GenerateInstanceId(), gateMapComponent.DomainZone(), "GateMap", SceneType.Map);

			Scene scene = gateMapComponent.Scene;
			
			// 这里可以从DB中加载Unit
			Unit unit = UnitFactory.Create(scene, player.Id, UnitType.Player, player.AvatarIndex + 1000);
			
			StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "Dungeon01");
			response.MyId = player.Id;
            
            //为了方便临时测试的机器人，正常启动服务器要走正常机器人创建逻辑
			// Unit robot = UnitFactory.Create(scene, 1000, UnitType.Player, 1000);
			// TransferHelper.TransferAtFrameFinish(robot, startSceneConfig.InstanceId, startSceneConfig.Name).Coroutine();

			// 等到一帧的最后面再传送，先让G2C_EnterMap返回，否则传送消息可能比G2C_EnterMap还早
			TransferHelper.TransferAtFrameFinish(unit, startSceneConfig.InstanceId, startSceneConfig.Name).Coroutine();
		}
	}
}