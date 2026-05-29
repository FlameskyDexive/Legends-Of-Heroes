# cn.etetet.mapplay

地图玩法层包。

- 依赖 `cn.etetet.map` 基础包。
- 承接地图场景初始化、客户端地图表现、地图内玩法编排。
- 承接 UnitInfo、客户端/服务端 Unit 组装、地图内玩法消息与表现事件。
- 不直接依赖 `cn.etetet.transfer`，传送业务包需要玩法组装时由 `transfer` 依赖本包。
- 可以依赖 `spell`、`item`、`quest`、`login`、`move` 等玩法包。
- 不允许被 `cn.etetet.map` 反向依赖。

## 摇杆/操作(Opera)

- `OperaComponent`(ModelView/Client)收集每帧操作 `OperateInfos`，`OperaComponentSystem.LateUpdate` 批量经 `C2M_Operation` 上报服务端。
- 摇杆 UI(DlgBattle，在 statesync)调用 `OperaComponent.OnMove/StopMove/OnClickSkill1/OnClickSkill2`（实现在本包 `OperaComponentSystem`）。
- 操作枚举 `EInputType`/`EOperateType`/`EOperateStatus` 在 `Model/Share/OperateDefine.cs`（从 Ori 战斗包下沉至此，避免高层战斗包 `cn.etetet.skill` 被反向依赖）。
- **proto 消息**：`OperateInfo`/`C2M_Operation` 定义在 `Proto/MapPlay_C_11200.proto`，已生成到 `cn.etetet.proto/CodeMode`（opcode 11210/11211）。
- **服务端处理（已实现）**：`Scripts/Hotfix/Server/C2M_OperationHandler.cs` 解析 `OperateInfos`：
  - `Move` + 按下/持续 → `unit.FindPathMoveToAsync(unit.Position + dir*距离)`；`Move` + 抬起 → `unit.Stop(1)`。
  - `Skill1/Skill2` → 发布 `OperateSkillCast` 事件（结构体在 `Model/Share/OperateDefine.cs`），由 `cn.etetet.skill` 订阅响应（`OperateSkillCast_SpellSkill`，调用 `SkillComponent.SpellSkill`）。**这样 mapplay 不反向依赖 skill**；skill(L6) 依赖 mapplay(L5) 是合法方向。
  - 技能瞄准方向：当前 `SpellSkill` 不接收 `Vec3` 方向，技能朝向由 `unit.Forward` 决定；若需按摇杆方向瞄准，需扩展 `SpellSkill` 签名。
