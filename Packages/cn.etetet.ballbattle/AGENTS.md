# cn.etetet.ballbattle

球球大作战 PvP 玩法包。基于 **statesync/mapplay 状态同步**(服务端权威 + AOI 广播),2D 碰撞用 `cn.etetet.box2dsharp`(**不用** 3D 的 `cn.etetet.collision`)。Level 7,依赖 box2dsharp / mapplay / move / skill / numeric / unit。

## 核心机制与同步思路
- **服务端权威**:位置/血量/体型/吞噬/分裂全在服务端裁决。客户端发意图(`C2M_Operation`),服务端改 `NumericComponent` → 自动经 `M2C_NumericChange` 广播给 AOI 视野内客户端。
- **质量=HP**:`Radius = RadiusCoef*sqrt(HP)`(面积∝HP)、`Speed` 随 HP 增大而降。见 `BallNumericHelper.RecalcFromHp`。任何改 HP 处(吞噬/扣血/分裂/吃食物)调一次即自动重算并广播 Radius/Speed。
- **体型表现**:客户端 `NumericWatcher_Radius_ScaleBall` 监听 Radius 变化缩放视图 GameObject;服务端 `NumericWatcher_Radius_UpdateCollision` 同步 Box2D 碰撞圆半径。

## 已实现(Phase A–D 核心)
- **Box2D 碰撞包装**(`Scripts/Model/Share/Collision` + `Hotfix/Share/Collision`):
  - `CollisionWorldComponent`(挂 Scene,持 Box2D `World`,`IUpdate` 每帧 `Step`)
  - `CollisionComponent`(挂 Unit,持 `Body`,`IUpdate` 同步 Unit→Body 位置;`AddCollider`/`SetBodyCircleRadius`)
  - `CollisionListenerComponent`(挂 Scene,实现 `IContactListener.BeginContact` → 发 `OnCollisionContact` 事件)
  - `CollisionHelper`(Circle/Box fixture + World 工厂)
  - 坐标:Unit 的 **X/Z** 映射到 Box2D 的 X/Y。
  - 原 Ori 用 `IFixedUpdate`/`DefineCore.FixedDeltaTime`/`MathHelper.Angle`/Bson,本项目无 → 改 `IUpdate` + `BallDefine.FixedDeltaTime` + `math.atan2` + 去 Bson。
- **数值公式** `BallNumericHelper`(HP→Radius/Speed)+ 两个 Radius `NumericWatcher`。
- **球类型** `BallComponent`(`EBallType` Player/Food/Bullet)。
- **碰撞裁决** `OnCollisionContactHandler`(`[Event(SceneType.Map)]`):子弹命中扣血变小/死亡移除、玩家吃食物、玩家间大吃小(比半径)。HP/质量变化直接在本包处理(skill 的 `BattleHelper.HitSettle` 当前是只算不写的桩,不依赖它应用伤害)。

## 待实现(下一增量)
- **Arena 装配**:把 `CollisionWorldComponent` + `CollisionListenerComponent` 加到 Map 场景(在 `mapplay/FiberInit_Map` 或新建 `BallArenaComponent`)。
- **给球加碰撞体**:Unit 创建时加 `BallComponent` + `CollisionComponent` 并 `AddCollider(Circle, radius, sensor=true, userData=unit)`,初始 `RecalcFromHp`。挂在 `UnitFactory.Create` 后或 `AfterUnitCreate` 事件。
- **食物刷新器**:Map 上 `RepeatedTimer` 维持 N 个食物球(`UnitFactory.Create` + `BallComponent(Food)`),被吃后补刷。仿 `FiberInit_Map` 造单位写法。
- **速度式移动**(Phase E):给 `MoveComponent` 加方向速度模式,改 `mapplay/C2M_OperationHandler` 的 Move 分支为设方向(替代 `FindPathMoveToAsync` 寻路)。
- **子弹技能**:`skill` 的 ActionEvent 发射子弹(需补 `UnitFactory.CreateBullet`,子弹 = `BallComponent(Bullet)` + `CollisionComponent` 朝向飞行)。
- **分裂技能**:`ActionEventSplit`(HP/2 + `UnitFactory.Create` 第二个球冲刺)。
- **死亡/重生/结算 UI**。

## Gating(配置/数据,需 Unity/Luban 侧补)
- 球的 `UnitConfig`(玩家球、食物球的 configId + 初始 HP/RadiusBase)。
- `skill` 的 `SkillConfig`/`ActionEventConfig` 导表(子弹/分裂技能用),见 `cn.etetet.skill/AGENTS.md`。
- 调参(EatRatio/RadiusCoef/Speed/BulletDamage/食物数量)正式应迁到配置表 `BallConfig`,当前在 `BallDefine` 给默认值。
