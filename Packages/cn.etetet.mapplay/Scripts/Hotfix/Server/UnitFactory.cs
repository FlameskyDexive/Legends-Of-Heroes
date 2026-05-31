using Unity.Mathematics;

namespace ET.Server
{
    public static partial class UnitFactory
    {
        // position:可选的初始位置。必须在 AddComponent<AOIEntity> 之前设好——AOIEntity.Awake 会按当前位置把单位注册进 AOI 网格
        // 并立刻 EnterSight→广播 M2C_CreateUnits(携带当前位置)。若创建后才改 Position(如球球大作战食物原先的写法),
        // 客户端拿到的是旧位置(配置默认 0,0,0)→ 所有食物挤在原点、且与服务端实际碰撞位置不符导致吃不到。传 position 即可避免。
        public static Unit Create(Scene scene, long id, int configId, float3? position = null)
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
                // 用 GetOrDefault(缺 key 返回 null)而非 Get(缺 key 抛 KeyNotFoundException):
                // 地图预先摆放的单位 id 才是 MapUnitConfig 的 key;运行时动态生成的单位(球球大作战食物/机器人,
                // 用 IdGenerater 随机 id)不在表里,此处本就该返回 null 跳过(下方有 !=null 判断)。
                MapUnitConfig mapUnitConfig = scene.Fiber().GetSingleton<MapUnitConfigCategory>().GetOrDefault((int)id);
                if (mapUnitConfig != null)
                {
                    foreach ((NumericType k, long v) in mapUnitConfig.KV)
                    {
                        numericComponent.SetNoEvent(k, v);
                    }
                }
            }

            unit.Position = position ?? new float3(numericComponent.GetAsFloat(NumericType.X), numericComponent.GetAsFloat(NumericType.Y), numericComponent.GetAsFloat(NumericType.Z));
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
