namespace ET.Server
{
	[MessageSessionHandler(SceneType.Gate)]
	public class C2G_EnterMapHandler : MessageSessionHandler<C2G_EnterMap, G2C_EnterMap>
	{
		// 球球大作战专属地图名(与 ballbattle 包 BallDefine.BallMapName 一致;login 不依赖 ballbattle 故本地常量)
		private const string BallMapName = "BallBattle";

		protected override async ETTask Run(Session session, C2G_EnterMap request, G2C_EnterMap response)
		{
			Player player = session.GetComponent<SessionPlayerComponent>().Player;
			EntityRef<Player> playerRef = player;
			GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
			EntityRef<GateMapComponent> gateMapComponentRef = gateMapComponent;
			await gateMapComponent.Create(player.Id);

			player = playerRef;
			gateMapComponent = gateMapComponentRef;
			response.MyId = player.Id;

			// 目标地图:客户端可指定(如球球大作战传 "BallBattle"),空则默认 Map2
			string mapName = string.IsNullOrEmpty(request.MapName) ? "Map2" : request.MapName;

			// 球球大作战玩家用球单位(UnitConfig 1101 BallPlayer,其 Name=BallPlayer -> 客户端据此加载球预制体);
			// 其它地图用默认人形单位 1001。进图后服务端 AfterUnitCreateServer_SetupBall 仍按地图名把它装配成球。
			int unitConfigId = mapName == BallMapName ? 1101 : 1001;

			// 等到一帧的最后面再进图，先让G2C_EnterMap返回，否则切场景消息可能比G2C_EnterMap还早
			EnterMapAtFrameFinish(player, gateMapComponent, player.GetComponent<PlayerSessionComponent>().GetActorId(), mapName, 0, unitConfigId).Coroutine();
		}

		private static async ETTask EnterMapAtFrameFinish(Player player, GateMapComponent gateMapComponent, ActorId gateActorId, string mapName, int mapId, int unitConfigId)
		{
			EntityRef<Player> playerRef = player;
			EntityRef<GateMapComponent> gateMapComponentRef = gateMapComponent;
			await player.Fiber().WaitFrameFinish();

			player = playerRef;
			gateMapComponent = gateMapComponentRef;

			G2Map_EnterMap g2MapEnterMap = G2Map_EnterMap.Create();
			g2MapEnterMap.PlayerId = player.Id;
			g2MapEnterMap.GateActorId = gateActorId;
			g2MapEnterMap.MapName = mapName;
			g2MapEnterMap.MapId = mapId;
			g2MapEnterMap.UnitConfigId = unitConfigId;

			try
			{
				await player.Root().GetComponent<MessageSender>().Call(gateMapComponent.Fiber.Root.GetActorId(), g2MapEnterMap);
			}
			finally
			{
				player = playerRef;
				gateMapComponent = gateMapComponentRef;
				await player.Fiber().RemoveFiber(gateMapComponent.Fiber.Id);
				player = playerRef;
				player.RemoveComponent<GateMapComponent>();
			}
		}
	}
}
