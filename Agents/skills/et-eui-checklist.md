# et-eui-checklist - EUI提测前快速勾选单

> 用途：UI 改动后，提测/合并前 1 分钟快速自检。

---

## A. Prefab与命名

- [ ] 所有窗口 Prefab 命名为 `Dlg*`
- [ ] 所有子 UI Prefab 命名为 `ES*`
- [ ] 所有循环项 Prefab 命名为 `Item*`
- [ ] 可绑定控件命名为 `E*`
- [ ] 可绑定容器命名为 `EG*`
- [ ] 循环项 `Item*` 已挂 `LayoutElement`

---

## B. 代码生成

- [ ] 已执行 `ET/EUICodeSpawn`
- [ ] 未出现生成报错（控制台无 Error）
- [ ] `WindowId.cs` 已包含对应 `WindowID_*`
- [ ] `WindowItemRes.cs` 已更新（如使用循环列表）

快速命令（PowerShell）：

```powershell
pwsh -Command "rg -n 'WindowID_' Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowId.cs"
pwsh -Command "rg -n 'WindowItemResDictionary|WindowID\.' Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowItemRes.cs"
```

---

## C. 编译与控制台

- [ ] 已触发 Unity 刷新与编译
- [ ] 编译后控制台无新增 `Error/Exception/NullReferenceException`
- [ ] 无 `uiTransform is null` 相关错误

UnityMCP步骤：

- [ ] `refresh_unity(mode="force", compile="request", scope="all", wait_for_ready=true)`
- [ ] `read_console(action="get", count="300", include_stacktrace="true")`

---

## D. 运行态冒烟

- [ ] 进入 PlayMode 成功
- [ ] 目标 UI 可正常打开/关闭
- [ ] `Global/UI` 下存在四层 Root：
  - [ ] `NormalRoot`
  - [ ] `PopUpRoot`
  - [ ] `FixedRoot`
  - [ ] `OtherRoot`
- [ ] 退出 PlayMode 后控制台仍无新增 UI 错误

UnityMCP步骤：

- [ ] `manage_editor(action="play", wait_for_completion="true")`
- [ ] `manage_scene(action="get_hierarchy", parent="Global/UI", max_depth="2", max_nodes="200")`
- [ ] `manage_editor(action="stop", wait_for_completion="true")`

---

## E. 文件边界检查（防覆盖）

- [ ] 业务逻辑改在：
  - [ ] `HotfixView/Client/Demo/UI/DlgXxx/DlgXxxSystem.cs`
  - [ ] `HotfixView/Client/Demo/UI/DlgXxx/Event/DlgXxxEventHandler.cs`
- [ ] 未手改自动生成文件：
  - [ ] `*ViewComponent.cs`
  - [ ] `*ViewComponentSystem.cs`
  - [ ] 自动生成的 `UIItemBehaviour` 文件

---

## F. 提交前核对

- [ ] `git status` 仅包含本次预期改动
- [ ] 无临时调试代码/日志残留
- [ ] 关键交互（至少 1 条主流程）手动验证通过

快速命令（PowerShell）：

```powershell
pwsh -Command "git status --short"
pwsh -Command "git diff --name-only"
```

---

## G. 故障恢复（UnityMCP失连）

- [ ] 读取实例：`read_mcp_resource("mcpforunity://instances")`
- [ ] 重新绑定：`set_active_instance("Unity@<hash>")`
- [ ] 状态探测：`read_mcp_resource("mcpforunity://editor/state")`
- [ ] 重新执行 B/C 步骤

