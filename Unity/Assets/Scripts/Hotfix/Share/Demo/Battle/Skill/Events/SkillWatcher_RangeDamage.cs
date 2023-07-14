using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;

namespace ET
{
	/// <summary>
	/// 执行范围伤害技能事件
	/// </summary>
	[FriendOf(typeof(SkillEvent))]
	[SkillWatcher(ESkillEventType.RangeDamage)]
	public class SkillWatcher_RangeDamage : ISkillWatcher
	{
		public void Run(SkillEvent skillEvent, EventType.SkillEventType args)
		{
            Log.Info($"enter range damage");
            Unit owner = args.owner;
            if (owner == null)
                return;
            ListComponent<Entity> units = ListComponent<Entity>.Create();
            units.AddRange(skillEvent.DomainScene().GetComponent<UnitComponent>().Children.Values.ToList());
            for (int i = 0; i < units.Count; i++)
            {
                Unit unit = units[i] as Unit;
                if(unit == null || (unit.Type != UnitType.Player && unit.Type != UnitType.Monster) || unit.GetComponent<BattleUnitComponent>().IsDead())
                    continue;
                float dis = math.distance(owner.Position, unit.Position);
                //满足范围伤害，则进行命中伤害结算
                if (dis <= skillEvent.EventData[1] / 1000f)
                {
                    BattleHelper.HitSettle(owner, unit);
                }
            }
            
            units.Dispose();
        }
	}
}
