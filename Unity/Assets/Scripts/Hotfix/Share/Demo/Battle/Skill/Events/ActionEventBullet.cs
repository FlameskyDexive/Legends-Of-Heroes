using System.Collections.Generic;

namespace ET
{
	/// <summary>
	/// 技能发射子弹
	/// </summary>
	[ActionEvent(EActionEventType.Bullet)]
	[FriendOf(typeof(ActionEvent))]
	public class ActionEventBullet : IActionEvent
	{
		public void Run(ActionEvent actionEvent, EventType.ActionEventData args)
		{
            Unit owner = args.owner;
            Log.Info($"emit a bullet");
            if (owner == null)
                return;
#if DOTNET
			Scene scene = actionEvent.Scene();
            Unit bullet = Server.UnitFactory.CreateBullet(scene, IdGenerater.Instance.GenerateId(), actionEvent.OwnerSkill, -1000, actionEvent.ActionEventConfig.Params);

            // 通知客户端创建子弹Unit
            M2C_CreateUnits m2CCreateUnits = M2C_CreateUnits.Create();
            m2CCreateUnits.Units = new List<UnitInfo>();
            UnitInfo info = Server.UnitHelper.CreateUnitInfo(bullet);
            m2CCreateUnits.Units.Add(info);
           Server.MapMessageHelper.SendToClient(owner, m2CCreateUnits);
            // this.TimeOutDestroyBullet(bullet, scene).Coroutine();
#endif
        }


		// private async ETTask TimeOutDestroyBullet(Unit unit, Scene scene)
		// {
		// 	await TimerComponent.Instance.WaitAsync(1000);
		// 	scene?.GetComponent<UnitComponent>().Remove(unit.Id);
		// }
	}
	
	
}
