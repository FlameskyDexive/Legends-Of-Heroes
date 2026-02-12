# et-eui-runbook - EUI自动化执行清单（仅步骤）

本清单只提供执行顺序，不讲原理。

## 适用场景

- 改完 `Assets/Bundles/UI` 下任意 `Dlg* / ES* / Item*` Prefab 后，快速完成“生成→编译→运行验收”。
- 需要用 UnityMCP 进行可重复、可回放的 UI 流程执行。

---

## 0. 预备条件

- Unity Editor 已打开，工程为 `D:/Git/Legends-Of-Heroes/Unity`。
- ET 工程代码在最新可编译状态。
- 如发生域重载，先重新绑定实例再继续。

---

## 1. 会话绑定（UnityMCP）

按顺序执行：

1. `read_mcp_resource("mcpforunity://instances")`
2. `set_active_instance("Unity@<hash>")`
3. `read_mcp_resource("mcpforunity://project/info")`
4. `read_mcp_resource("mcpforunity://editor/state")`
5. `manage_scene(action="get_active")`

判定通过：

- 能拿到活动场景（通常 `Assets/Scenes/Init.unity`）
- `projectRoot` 指向 `D:/Git/Legends-Of-Heroes/Unity`

---

## 2. 生成EUI代码

执行菜单命令：

- `execute_menu_item(menu_path="ET/EUICodeSpawn")`

说明：

- 该命令会批量处理：
  - `Assets/Bundles/UI/Dlg/`
  - `Assets/Bundles/UI/Item/`
  - `Assets/Bundles/UI/Common/`

---

## 3. 刷新与编译

执行：

- `refresh_unity(mode="force", compile="request", scope="all", wait_for_ready=true)`

若超时或返回 not ready：

1. 重试 `set_active_instance`
2. 再执行一次 `refresh_unity(...)`

---

## 4. 控制台验收

执行：

- `read_console(action="get", count="300", include_stacktrace="true")`

关注关键字：

- `Error`
- `Exception`
- `NullReferenceException`
- `windowId`
- `uiTransform is null`

通过标准：

- 不存在新增的 UI 相关错误。

---

## 5. 运行态冒烟

执行：

1. `manage_editor(action="play", wait_for_completion="true")`
2. `read_console(action="get", count="300", include_stacktrace="true")`
3. `manage_scene(action="get_hierarchy", parent="Global/UI", max_depth="2", max_nodes="200")`
4. `manage_editor(action="stop", wait_for_completion="true")`

通过标准：

- `Global/UI` 下存在：`NormalRoot`、`PopUpRoot`、`FixedRoot`、`OtherRoot`
- Play 期间无新增 UI 报错

---

## 6. 文件产物核对

至少检查以下路径有同步变更（按改动类型）：

- `Unity/Assets/Scripts/ModelView/Client/Demo/UI/`
- `Unity/Assets/Scripts/ModelView/Client/Demo/UIBehaviour/`
- `Unity/Assets/Scripts/ModelView/Client/Demo/UIItemBehaviour/`
- `Unity/Assets/Scripts/HotfixView/Client/Demo/UI/`
- `Unity/Assets/Scripts/HotfixView/Client/Demo/UIBehaviour/`
- `Unity/Assets/Scripts/HotfixView/Client/Demo/UIItemBehaviour/`
- `Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowId.cs`
- `Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowItemRes.cs`

推荐本地命令（PowerShell）：

```powershell
pwsh -Command "git status --short"
pwsh -Command "rg -n 'WindowID_' Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowId.cs"
pwsh -Command "rg -n 'WindowItemResDictionary|WindowID\.' Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowItemRes.cs"
```

---

## 7. 回归风险点（必查）

- `Item*` 必须挂 `LayoutElement`，否则生成失败。
- 避免手改自动生成文件：
  - `*ViewComponent.cs`
  - `*ViewComponentSystem.cs`
  - 自动生成的 `UIItemBehaviour` 文件
- 业务代码优先写在：
  - `HotfixView/Client/Demo/UI/DlgXxx/DlgXxxSystem.cs`
  - `HotfixView/Client/Demo/UI/DlgXxx/Event/DlgXxxEventHandler.cs`

---

## 8. 故障恢复最小操作

当 UnityMCP 出现 `session not ready` / `timeout`：

1. `read_mcp_resource("mcpforunity://instances")`
2. `set_active_instance("Unity@<hash>")`
3. `read_mcp_resource("mcpforunity://editor/state")`
4. 再继续第 2 步（重新生成与刷新）

