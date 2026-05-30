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
	[ActionEvent(SceneType.Map, EActionEventType.RangeDamage)]
	public class ActionEventRangeDamage : IActionEvent
	{
		public void Run(ActionEvent actionEvent, ActionEventData args)
		{
            Unit owner = args.owner;
            Log.Info($"enter range damage");
            if (owner == null)
                return;
            // 多态配置: 范围伤害参数(半径,毫米)从 ActionEventParams_RangeDamage 取
            ActionEventParams_RangeDamage rangeParams = actionEvent.ActionEventConfig.Params as ActionEventParams_RangeDamage;
            if (rangeParams == null)
                return;
            float radius = rangeParams.Radius / 1000f;
            ListComponent<Entity> units = ListComponent<Entity>.Create();
            units.AddRange(actionEvent.Root().GetComponent<UnitComponent>().Children.Values.ToList());
            for (int i = 0; i < units.Count; i++)
            {
                Unit unit = units[i] as Unit;
                if(unit == null || (unit.UnitType != UnitType.Player && unit.UnitType != UnitType.Monster) || unit.GetComponent<SkillComponent>().IsDead())
                    continue;
                float dis = math.distance(owner.Position, unit.Position);
                //满足范围伤害，则进行命中伤害结算
                if (dis <= radius)
                {
                    BattleHelper.HitSettle(owner, unit);
                }
            }
            
            units.Dispose();
        }
	}
}
