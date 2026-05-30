# ET EUI 执行清单（仅步骤）

改完 EUI 代码或预制体后的“编译 → 刷新 → 运行 → 验收”可重复流程。只讲步骤，原理见 `et-eui-mechanism.md`。

> EUI 编辑器自动化**一律不使用 UnityBridge**。通道优先级：**AIBridge（`$CLI = ./.aibridge/cli/AIBridgeCLI.exe`）→ 不可用时降级 UnityMCP（`unity-mcp-skill`，`mcpforunity://`）**。PlayMode、读场景层级等 AIBridge 不提供的能力直接用 UnityMCP。AIBridge「不可用」判据：`AIBridgeCLI.exe` 不存在，或命令返回连接超时 /「Unity Editor not running or AIBridge not active」/ 状态无法确认。本清单只给顺序与判定。

---

## 0. 预备条件

- 已按 5 文件模板手写完 `DlgXxx`（含 `WindowID_Xxx` 枚举项），预制体命名为 `DlgXxx` 且 `Item*` 已挂 `LayoutElement`。
- 新建 `.cs` 不要手工建 `.meta`，由 Unity 刷新生成。

---

## 1. 编译（et-build）

```powershell
dotnet build ET.sln
```

- 通过标准：无编译错误、无分析器报错（EntityRef/await、Module、命名空间等）。
- 有错先修代码再继续，不要带着编译错误进编辑器。

---

## 2. 刷新与生成（AIBridge 优先 → UnityMCP）

- **AIBridge（首选）**：`$CLI compile unity` —— 触发 Unity 刷新 + 重新编译，自动导入新文件、生成 `.meta` 与工程文件。
- **AIBridge 不可用时降级 UnityMCP**：`refresh_unity(mode="force", compile="request", scope="all", wait_for_ready=true)`。
- 等 Unity 不在编译中、无新增报错再继续。**不要用 UnityBridge。**

---

## 3. 控制台验收（AIBridge 优先 → UnityMCP）

- **AIBridge（首选）**：`$CLI get_logs --logType Error`。**不可用时降级 UnityMCP**：`read_console(action="get", count="200", include_stacktrace="true")`。
- 关注关键字：
  - `Error` / `Exception` / `NullReferenceException`
  - `uiTransform is null`
  - `is not have any windowId` / `uiPath is not Exist`（WindowID 或预制体名不匹配）
- 通过标准：无新增 UI 相关错误。

---

## 4. 运行态冒烟（UnityMCP；AIBridge 不提供 PlayMode/层级）

> 进入退出 PlayMode、读场景层级用 UnityMCP（`unity-mcp-skill`）。**禁用 UnityBridge。**

1. `manage_editor(action="play", wait_for_completion="true")` 进入 PlayMode。
2. 读控制台确认无新增异常（`read_console(...)`，或 AIBridge `get_logs --logType Error`）。
3. `manage_scene(action="get_hierarchy", parent="Global/UI", max_depth="2", max_nodes="200")`，确认存在四个 Root：`NormalRoot`、`PopUpRoot`、`FixedRoot`、`OtherRoot`。
4. 触发目标窗口（或在测试入口调用 `UIComponent.ShowWindow<DlgXxx>()`），确认能正常打开/关闭。
5. `manage_editor(action="stop", wait_for_completion="true")` 退出 PlayMode，再次读控制台确认无残留报错。

---

## 5. 文件产物核对

业务包内应成套出现（按改动类型）：

- `Packages/cn.etetet.{业务包}/Scripts/ModelView/Client/EUI/DlgXxx/DlgXxx.cs`
- `Packages/cn.etetet.{业务包}/Scripts/ModelView/Client/EUI/DlgXxx/DlgXxxViewComponent.cs`
- `Packages/cn.etetet.{业务包}/Scripts/HotfixView/Client/EUI/DlgXxx/DlgXxxSystem.cs`
- `Packages/cn.etetet.{业务包}/Scripts/HotfixView/Client/EUI/DlgXxx/DlgXxxViewComponentSystem.cs`
- `Packages/cn.etetet.{业务包}/Scripts/HotfixView/Client/EUI/DlgXxx/DlgXxxEventHandler.cs`

框架包内（如有改动）：

- `Packages/cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowID.cs`（新增 `WindowID_Xxx`）
- `Packages/cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowItemRes.cs`（如用循环列表预热）

推荐本地命令（PowerShell）：

```powershell
pwsh -Command "git status --short"
pwsh -Command "rg -n 'WindowID_' Packages/cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowID.cs"
```

---

## 6. 回归风险点（必查）

- `Item*` 必须挂 `LayoutElement`。
- 控件命名 `E*`，`FindDeepChild` 名字与预制体一致。
- `[AUIEvent(WindowID.WindowID_Xxx)]` 与 `WindowID` 项、预制体名三者一致。
- `await` 后访问 Entity 用 `EntityRef` 重新获取（叠加 `et-async`）。
- 业务窗口放在业务包，框架包不反向依赖业务包。

---

## 7. 故障恢复最小操作

- **AIBridge 报连接超时 /「Unity Editor not running or AIBridge not active」/ 状态无法确认** → 视为 AIBridge 不可用，降级到 UnityMCP。
- **UnityMCP 返回 `session not ready` / `timeout` 或域重载后**：
  1. `read_mcp_resource("mcpforunity://instances")`
  2. `set_active_instance("Unity@<hash>")`
  3. `read_mcp_resource("mcpforunity://editor/state")`
  4. 等编译/域重载完成，重新执行第 2~4 步（刷新 → 读控制台 → 冒烟）。
- 全程**不要降级到 UnityBridge**。
