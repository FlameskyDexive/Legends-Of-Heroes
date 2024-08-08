using System.Collections.Generic;
using System.Linq;
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
            if(unit.Type() == EUnitType.Player)
                unit.AddComponent<SkillComponent, List<int>>(unitInfo.SkillInfo.Keys.ToList());

            // unit.AddComponent<XunLuoPathComponent>();

            EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreate() {Unit = unit});
            if (currentScene.Root().GetComponent<PlayerComponent>().MyId == unit.Id)
            {
                Log.Info($"~~~~init my unit, ui battle");
                EventSystem.Instance.Publish(currentScene, new AfterMyUnitCreate() { unit = unit });
            }
            return unit;
        }
    }
}
