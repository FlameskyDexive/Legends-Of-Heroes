using Unity.Mathematics;

namespace ET.Server
{
    public static partial class UnitFactory
    {
        public static Unit Create(Scene scene, long id, int configId)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();

            Unit unit = unitComponent.AddChildWithId<Unit, int>(id, configId);
            UnitConfig unitConfig = unit.Config();

            unit.UnitType = unitConfig.UnitType;

            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            foreach ((NumericType k, long v) in unitConfig.KV)
            {
                numericComponent.SetNoEvent(k, v);
            }

            if (unit.UnitType != UnitType.Player)
            {
                MapUnitConfig mapUnitConfig = scene.Fiber().GetSingleton<MapUnitConfigCategory>().Get((int)id);
                if (mapUnitConfig != null)
                {
                    foreach ((NumericType k, long v) in mapUnitConfig.KV)
                    {
                        numericComponent.SetNoEvent(k, v);
                    }
                }
            }

            unit.Position = new float3(numericComponent.GetAsFloat(NumericType.X), numericComponent.GetAsFloat(NumericType.Y), numericComponent.GetAsFloat(NumericType.Z));
            unit.Rotation = quaternion.Euler(0, math.radians(numericComponent.Get(NumericType.Yaw)), 0);

            unit.AddComponent<MoveComponent>();
            unit.AddComponent<TurnComponent>();
            unit.AddComponent<AOIEntity>();
            unit.AddComponent<TargetComponent>();
            unit.AddComponent<SpellComponent>();
            unit.AddComponent<BuffComponent>();

            switch (unit.UnitType)
            {
                case UnitType.Player:
                {
                    unit.AddComponent<ItemComponent>();
                    unit.AddComponent<QuestComponent>();
                    break;
                }
                case UnitType.Monster:
                {
                    unit.AddComponent<ThreatComponent>();
                    unit.AddComponent<PathfindingComponent, string>(scene.Name.GetSceneConfigName());
                    break;
                }
                case UnitType.NPC:
                {
                    break;
                }
                case UnitType.Virtual:
                {
                    break;
                }
            }

            int ai = numericComponent.GetAsInt(NumericType.AI);
            if (ai != 0)
            {
                BuffHelper.CreateBuff(unit, unit.Id, IdGenerater.Instance.GenerateId(), ai, null);
            }

            unitComponent.Add(unit);

            // 通用扩展点:通知高层玩法包单位已创建(如 cn.etetet.ballbattle 装配球),避免 mapplay 反向依赖高层包
            EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreateServer { Unit = unit });
            return unit;
        }

        public static Unit CreatePet(Scene scene, Unit owner, long id, int configId)
        {
            Unit pet = Create(scene, id, configId);
            pet.AddComponent<PetComponent>().OwnerId = owner.Id;

            UnitPetComponent unitPetComponent = owner.GetComponent<UnitPetComponent>() ?? owner.AddComponent<UnitPetComponent>();
            unitPetComponent.PetId = pet.Id;
            return pet;
        }
    }
}
