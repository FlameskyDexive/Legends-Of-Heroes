using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace ET.Server
{
    public static class UnitFactory
    {
        public static Unit Create(Scene scene, long id, UnitType unitType, int config = 0)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            switch (unitType)
            {
                case UnitType.Player:
                {
                    Unit unit = unitComponent.AddChildWithId<Unit, int>(id, config);
                    unit.AddComponent<MoveComponent>();
                    unit.Position = new float3(0, 0, 0);
			
                    NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
                    numericComponent.Set(NumericType.Speed, 3f); // 速度是3米每秒
                    numericComponent.Set(NumericType.AOI, 15000); // 视野15米
                    
                    // unitComponent.Add(unit);
                    // 加入aoi
                    unit.AddComponent<AOIEntity, int, float3>(9 * 1000, unit.Position);
                    return unit;
                }
                default:
                    throw new Exception($"not such unit type: {unitType}");
            }
        }
        public static Unit CreateBullet(Scene scene, long id, Skill ownerSkill, int config, int[] bulletData)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(id, config);
            unit.AddComponent<MoveComponent>();
            unit.Position = new float3(0, 0, 0);
			
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            numericComponent.Set(NumericType.Speed, 8f); // 速度是3米每秒
            numericComponent.Set(NumericType.AOI, 15000); // 视野15米
            
            return unit;
        }
    }
}