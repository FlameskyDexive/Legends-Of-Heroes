using System;


namespace ET.Server
{
	[MessageHandler(SceneType.Gate)]
	public class Match2G_StateSyncRefreshMatchHandler : MessageHandler<Player, Match2G_StateSyncRefreshMatch>
	{
		protected override async ETTask Run(Player player, Match2G_StateSyncRefreshMatch message)
		{
			player.AddComponent<PlayerRoomComponent>().RoomActorId = message.ActorId;
			
			player.GetComponent<PlayerSessionComponent>().Session.Send(message);
			await ETTask.CompletedTask;
		}
	}
}