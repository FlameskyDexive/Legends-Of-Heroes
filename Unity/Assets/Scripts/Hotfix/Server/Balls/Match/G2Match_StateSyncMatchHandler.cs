using System;


namespace ET.Server
{
	[MessageHandler(SceneType.Match)]
	public class G2Match_StateSyncMatchHandler : MessageHandler<Scene, G2Match_StateSyncMatch, Match2G_StateSyncMatch>
	{
		protected override async ETTask Run(Scene scene, G2Match_StateSyncMatch request, Match2G_StateSyncMatch response)
		{
            StateSyncMatchComponent matchComponent = scene.GetComponent<StateSyncMatchComponent>();
			matchComponent.Match(request.Id).Coroutine();
			await ETTask.CompletedTask;
		}
	}
}