using System;


namespace ET.Server
{
	[MessageHandler(SceneType.Gate)]
	public class Match2G_StateSyncNotifyMatchSuccessHandler : MessageHandler<Player, Match2G_StateSyncNotifyMatchSuccess>
	{
		protected override async ETTask Run(Player player, Match2G_StateSyncNotifyMatchSuccess message)
		{
			player.AddComponent<PlayerRoomComponent>().RoomActorId = message.ActorId;
			
			player.GetComponent<PlayerSessionComponent>().Session.Send(message);
			await ETTask.CompletedTask;
		}
	}
}