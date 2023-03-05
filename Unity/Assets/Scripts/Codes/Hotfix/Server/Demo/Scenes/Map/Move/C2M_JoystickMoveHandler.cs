
namespace ET.Server
{
	[ActorMessageHandler(SceneType.Map)]
	public class C2M_JoystickMoveHandler : AMActorLocationHandler<Unit, C2M_JoystickMove>
	{
		protected override async ETTask Run(Unit unit, C2M_JoystickMove message)
		{
			//收到移动消息，往前移动，如果有地形，需要判定前方位置是否可以移动。
            long speed = unit.GetComponent<NumericComponent>().GetByKey(NumericType.Speed);
            speed = speed == 0? 3 : speed;
            unit.Position = unit.Forward * speed ;
            M2C_JoystickMove m2CJoystickMove = new M2C_JoystickMove(){Position = unit.Position, Id = unit.Id };

            MessageHelper.Broadcast(unit, m2CJoystickMove);

            await ETTask.CompletedTask;
		}
	}
}