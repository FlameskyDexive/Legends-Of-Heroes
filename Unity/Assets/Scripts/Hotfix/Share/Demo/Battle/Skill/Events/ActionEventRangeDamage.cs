using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;

namespace ET
{
	/// <summary>
	/// 执行范围伤害技能事件
	/// </summary>
	[FriendOf(typeof(ActionEvent))]
	[ActionEvent(EActionEventType.RangeDamage)]
	public class ActionEventRangeDamage : IActionEvent
	{
		public void Run(ActionEvent actionEvent, EventType.ActionEventData args)
		{
            Unit owner = args.owner;
            Log.Info($"enter range damage");
            if (owner == null)
                return;
            ListComponent<Entity> units = ListComponent<Entity>.Create();
            units.AddRange(actionEvent.Root().GetComponent<UnitComponent>().Children.Values.ToList());
            for (int i = 0; i < units.Count; i++)
            {
                Unit unit = units[i] as Unit;
                if(unit == null || (unit.Type() != EUnitType.Player && unit.Type() != EUnitType.Monster) || unit.GetComponent<SkillComponent>().IsDead())
                    continue;
                float dis = math.distance(owner.Position, unit.Position);
                //满足范围伤害，则进行命中伤害结算
                if (dis <= actionEvent.ActionEventConfig.Params[1] / 1000f)
                {
                    BattleHelper.HitSettle(owner, unit);
                }
            }
            
            units.Dispose();
        }
	}
}
