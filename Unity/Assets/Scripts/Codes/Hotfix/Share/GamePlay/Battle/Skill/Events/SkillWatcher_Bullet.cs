using System.Collections.Generic;

namespace ET
{
	/// <summary>
	/// 技能发射子弹
	/// </summary>
	[SkillWatcher(ESkillEventType.Bullet)]
	[FriendOf(typeof(SkillEvent))]
	public class SkillWatcher_Bullet : ISkillWatcher
	{
		public void Run(SkillEvent skillEvent, EventType.SkillEventType args)
		{
            Log.Info($"emit a bullet");
            Unit owner = args.owner;
            if (owner == null)
                return;
#if DOTNET
			Scene scene = skillEvent.DomainScene();
            Unit bullet = Server.UnitFactory.CreateBullet(scene, IdGenerater.Instance.GenerateUnitId(skillEvent.DomainZone()), skillEvent.Skill, -1000, skillEvent.EventData);

            // 通知客户端创建子弹Unit
            M2C_CreateUnits m2CCreateUnits = new M2C_CreateUnits(){ Units = new List<UnitInfo>()};
            UnitInfo info = Server.UnitHelper.CreateUnitInfo(bullet);
            m2CCreateUnits.Units.Add(info);
            Server.MessageHelper.SendToClient(owner, m2CCreateUnits);
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
