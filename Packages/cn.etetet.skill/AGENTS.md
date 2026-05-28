# cn.etetet.skill

## 概述

战斗 / 技能系统,从 Legends-Of-HeroesOri 的 `Assets/Scripts/.../GamePlay/Battle` 迁移而来。包含:

- **Skill**:`Skill`、`SkillComponent`、`SkillTimelineComponent`、技能时间轴
- **ActionEvent(战斗事件)**:`ActionEvent`、`ActionEventComponent`(派发器)、`IActionEvent`、`ActionEventAttribute`,以及事件实现 `ActionEventRangeDamage`(范围伤害)、`ActionEventBullet`(发射子弹)
- **Bullet**:`BulletComponent`
- **Dungeon**:`DungeonComponent`(占位)
- **Battle**:`BattleHelper`(命中结算 `HitSettle`)、`ActionEventHelper`、`BattleDefine`(枚举)、`HitResult` 战斗事件

命名空间 `ET`;`Scripts/Model/Share/Battle/**` 与 `Scripts/Hotfix/Share/Battle/**` 镜像原 GamePlay/Battle 子结构。

## Buff:改用本项目 buff 系统(关键)

原工程战斗系统自带 Buff(`Battle/Buff/`),**本包未迁移它**——`ActionEventHelper.CreateActionEvent(this Buff self)` 里的 `Buff` 会自动绑定到本项目 `cn.etetet.spell` 的 `ET.Buff`(同名同为 Entity,只用到 `AddChild`,Entity 通用方法)。这就满足"调用 buff 处改用本项目 buff"。本包依赖 `cn.etetet.spell`。

## 已做的框架适配(相对 Ori)

- **MongoDB.Bson 剥离**:实体上的 `[BsonIgnore]`/`[BsonElement]`/`[BsonDictionaryOptions]` 及 using 已移除(本项目用 ET/MemoryPack 序列化)。**注意**:被 `[BsonIgnore]` 标记的字段语义是"不序列化",迁移后需按本项目序列化方式复核(必要时加 `[MemoryPackIgnore]` 或调整为只读属性)。
- **UnitType**:`unit.Type()`/`EUnitType.Player/Monster` → `unit.UnitType`/`UnitType.Player/Monster`(本项目 `UnitType` 枚举,见 cn.etetet.config)。
- **NumericType**:`NumericType.Hp` → `NumericType.HP`(本项目命名)。
- **HitResult 战斗事件**:从 Ori `Model/Share/GamePlay/EventType.cs` 单独迁出到 `Model/Share/Battle/BattleEvents.cs`(其余事件本项目已有,不重复迁移)。

## 编译前必须完成的依赖(gating,需在 Unity/项目侧处理)

1. **配置导表(SkillConfig / ActionEventConfig)** —— 本项目配置框架与 Ori 不兼容(本项目 `Singleton<>,IConfig`;Ori `Bright/ConfigSingleton`),**不能直接搬生成码**。已把 Ori 的数据表暂存到 `Luban/Config/Datas/{SkillConfig,ActionEventConfig}.xlsx`。需要:
   - 在某配置包(建议 `cn.etetet.config` 或本包新建 `Luban/Config/Base`)的 `__tables__.xlsx` 增加 `SkillConfig`(联合主键 `Id,Level`)、`ActionEventConfig`(主键 `Id`)表定义,字段对齐战斗代码使用(SkillConfig:Id/Level/AbstractType/Name/Desc/Life/CD/ActionEventIds/ActionEventTriggerPercent;ActionEventConfig:Id/Name/Desc/ActionEventType/IsClientOnly/Params)。
   - 在中央 `luban.conf` 注册;运行 et-luban 导表,生成本项目风格的 `SkillConfigCategory.Instance.Get(Id,Level)` 与 `ActionEventConfigCategory.Instance.Get(Id)`(战斗代码已按此 API 调用)。
2. **数值类型 Attack** —— `BattleHelper.HitSettle` 用 `NumericType.Attack` 计算伤害,本项目 `NumericType` 当前**没有 Attack**(只有 HP/MaxHP/MP/Speed/Radius/AOI…)。需在本项目数值配置补一个攻击力数值类型(如 `Attack`)。
3. **服务端 proto / 工厂(仅 `#if DOTNET`)** —— `ActionEventBullet` 在服务端分支用到 `M2C_CreateUnits`(proto 消息)、`Server.UnitFactory.CreateBullet`、`Server.UnitHelper.CreateUnitInfo`、`Server.MapMessageHelper.SendToClient`。客户端不受影响;服务端需具备这些类型(proto 与 map/unit 服务端工厂)。

## 未迁移

- Ori `Battle/Buff/*`(改用本项目 buff)
- `Battle/Skill/SkillTimelineComponentSystem` 等若引用了未列出的其它系统,按报错补依赖
- DlgBattle UI(属 UI 层,另行处理)
