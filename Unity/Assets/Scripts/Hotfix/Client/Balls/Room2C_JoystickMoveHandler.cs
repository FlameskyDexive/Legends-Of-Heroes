using ET.Server;

namespace ET.Client
{
	[MessageHandler(SceneType.Demo)]
	public class Room2C_JoystickMoveHandler : MessageHandler<Scene, Room2C_JoystickMove>
	{
		protected override async ETTask Run(Scene root, Room2C_JoystickMove message)
        {
            Scene currentScene = root.CurrentScene();
            if (message?.Id > 0)
            {
                Unit unit = currentScene.GetComponent<UnitComponent>().Get(message.Id);
                if (unit != null)
                {
                    unit.Forward = message.MoveForward;
                    unit.Position = message.Position;
                }

            }
			await ETTask.CompletedTask;
		}
	}
}
