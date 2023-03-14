
using Unity.Mathematics;

namespace ET.Server
{
	[ActorMessageHandler(SceneType.Lobby)]
	public class C2L_JoinOrCreateRoomHandler : AMActorLocationHandler<Scene, C2L_JoinOrCreateRoom>
	{
		protected override async ETTask Run(Scene scene, C2L_JoinOrCreateRoom message)
		{
			Log.Error($"actor msg, join room:{message.PlayerId}");

            L2C_JoinOrCreateRoom joinOrCreateRoom = new L2C_JoinOrCreateRoom();

            // MessageHelper.Broadcast(unit, joinOrCreateRoom);

            await ETTask.CompletedTask;
		}
	}
}