using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace ET.Client
{
    public static class UnitFactory
    {
        public static Unit Create(Scene currentScene, UnitInfo unitInfo)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            UnitType unitType = (UnitType)unitInfo.Type;
            /*switch (unitType)
            {
				case UnitType.Player:
				case UnitType.Bullet:
				case UnitType.Monster:
                    
                    break;
                
            }*/
            
	        unit.Position = unitInfo.Position;
	        unit.Forward = unitInfo.Forward;
	        
	        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
	        if (unitInfo.KV != null)
	        {
		        foreach (var kv in unitInfo.KV)
		        {
			        numericComponent.Set(kv.Key, kv.Value);
		        }
	        }
	        //客户端的角色位移来自服务端定期同步之后做插值
	        /*unit.AddComponent<MoveComponent>();
	        if (unitInfo.MoveInfo != null)
	        {
		        if (unitInfo.MoveInfo.Points.Count > 0)
				{
					unitInfo.MoveInfo.Points[0] = unit.Position;
					unit.MoveToAsync(unitInfo.MoveInfo.Points).Coroutine();
				}
	        }*/

	        unit.AddComponent<ObjectWait>();
            if (unitInfo.Skills?.Count > 0)
            {
                unit.AddComponent<BattleUnitComponent, List<int>>(unitInfo.Skills.Keys.ToList());
            }
            else
            {
                unit.AddComponent<BattleUnitComponent>();
            }
	        // unit.AddComponent<XunLuoPathComponent>();
	        
	        EventSystem.Instance.Publish(unit.DomainScene(), new EventType.AfterUnitCreate() {Unit = unit});
            return unit;
        }
    }
}
