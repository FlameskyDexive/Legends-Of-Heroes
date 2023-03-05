

namespace ET.Client
{
	[MessageHandler(SceneType.Client)]
	public class M2C_JoystickMoveHandler : AMHandler<M2C_JoystickMove>
	{
		protected override async ETTask Run(Session session, M2C_JoystickMove message)
		{
			Unit unit = session.DomainScene().CurrentScene().GetComponent<UnitComponent>().Get(message.Id);

			// float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            unit.Position = message.Position;
            // await unit.GetComponent<MoveComponent>().MoveToAsync(message.Points, speed);
        }
	}
}
