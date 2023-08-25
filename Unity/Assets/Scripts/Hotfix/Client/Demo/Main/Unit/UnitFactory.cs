using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Client
{
    public static partial class UnitFactory
    {
        public static Unit Create(Scene currentScene, UnitInfo unitInfo)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
	        unitComponent.Add(unit);
	        
	        unit.Position = unitInfo.Position;
	        unit.Forward = unitInfo.Forward;
	        
	        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();

			foreach (var kv in unitInfo.KV)
			{
				numericComponent.Set(kv.Key, kv.Value);
			}
	        
	        unit.AddComponent<MoveComponent>();
	        if (unitInfo.MoveInfo != null)
	        {
		        if (unitInfo.MoveInfo.Points.Count > 0)
				{
					unitInfo.MoveInfo.Points[0] = unit.Position;
					unit.MoveToAsync(unitInfo.MoveInfo.Points).Coroutine();
				}
	        }

	        unit.AddComponent<ObjectWait>();
            if (unitInfo.Skills?.Count > 0)
            {
                using ListComponent<int> list = ListComponent<int>.Create();
                foreach (int skillsKey in unitInfo.Skills.Keys)
                {
                    list.Add(skillsKey);
                }
                unit.AddComponent<BattleUnitComponent, List<int>>(list);
            }
            else
            {
                unit.AddComponent<BattleUnitComponent>();
            }

            // unit.AddComponent<XunLuoPathComponent>();
	        
	        EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreate() {Unit = unit});
            return unit;
        }
    }
}
