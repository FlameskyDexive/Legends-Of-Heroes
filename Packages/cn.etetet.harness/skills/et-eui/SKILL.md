---
name: et-eui
description: ET EUI client UI workflow for this project's cn.etetet.eui framework. Use when creating or modifying DlgXxx windows, ESxxx reusable sub-UI, ItemXxx loop items, AUIEvent handlers, WindowID registration, widget binding (E* via UIFindHelper), ShowWindow/Hide/Close flow, EUI root layers, or LoopScrollRect lists. Hand-authored 5-file pattern; there is NO code-gen menu here.
---

# et-eui - ET EUI 入口

> 本项目 EUI 框架在 `Packages/cn.etetet.eui`，移植自原 Legends-Of-Heroes，但**未移植代码生成工具**（不存在 `ET/EUICodeSpawn` 菜单 / `UICodeSpawner`）。所有 EUI 代码都是按约定**手写**的，不要去找或调用生成菜单。
>
> **编辑器自动化通道（重要）**：EUI **一律不走 UnityBridge**。优先级 = **AIBridge（`$CLI = ./.aibridge/cli/AIBridgeCLI.exe`）→ 不可用时降级 UnityMCP（`unity-mcp-skill`，`mcpforunity://`）**；PlayMode、读场景层级等 AIBridge 不提供的能力直接用 UnityMCP。步骤见 `references/et-eui-runbook.md`。

## 何时使用

- 新建或修改窗口 `DlgXxx`、可复用子 UI `ESXxx`、循环项 `ItemXxx`。
- 编写 `DlgXxxEventHandler`（`[AUIEvent(WindowID.WindowID_Xxx)]`）、注册控件事件、绑定 `E*` 控件。
- 新增 `WindowID` 枚举项、接好窗口显示链路（`ShowWindow` / `ShowWindowAsync` / `HideWindow` / `CloseWindow`）。
- 排查 UI 打不开、控件取到 null、`uiTransform is null`、循环列表不刷新、Root 层级不对。

## 不要加载

- 只是写普通 ET 代码（Entity/System/Helper）：用 `et-code`，必要时叠加 `et-async`。
- 只是编译 / 跑测试 / 导配置：用 `et-build` / `et-test-run` / `et-luban`。
- 只做与 EUI 无关的通用编辑器操作（编译/刷新/PlayMode/读控制台）：按根 `AGENTS.md`「Unity 交互入口优先级」（AIBridge→UnityMcp→UnityBridge）执行，不必加载本 skill。

## 默认动作

1. 先确认落点：EUI **框架**改动在 `cn.etetet.eui`；**业务窗口 DlgXxx** 放进所属业务包（参考 `cn.etetet.statesync`、`cn.etetet.lockstep`），不要散布到框架包，也不要让框架包反向依赖业务包。
2. 新增窗口前，先确认 `WindowID` 枚举里有没有对应项；没有则在 `cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowID.cs` 加 `WindowID_Xxx`。
3. 预制体命名必须是 `Dlg{Name}`，`{Name}` 即枚举去掉 `WindowID_` 前缀（`UIPathComponentSystem` 据此自动映射路径与 `typeof(T).Name`），不需要手填路径表。
4. 按 5 文件手写模板成套创建（见 `references/et-eui-mechanism.md`），逻辑只写在 `DlgXxxSystem` / `DlgXxxEventHandler`，控件懒加载放 `DlgXxxViewComponent`。
5. 控件命名用 `E*`（如 `ELogin`），`DlgXxxViewComponent` 用 `UIFindHelper.FindDeepChild<T>(uiTransform.gameObject, "E...")` 懒加载并在 `DestroyWidget()` 置空。
6. 涉及 `async` / `await` / `ETTask`、`await` 后访问 `UIBaseWindow` 等 Entity：叠加 `et-async`，用 `EntityRef` 重新获取。
7. 改完代码后用 `et-build`（`dotnet build ET.sln`）编译；编辑器刷新 `.meta` / PlayMode 验收走 EUI 编辑器自动化通道：**AIBridge 优先（`$CLI compile unity` / `get_logs`），不可用时 UnityMCP（`unity-mcp-skill`）；EUI 一律不走 UnityBridge**（见 `references/et-eui-runbook.md`）。
8. 严禁手工生成 `.meta` 或改 `.csproj`；新建 `.cs` 后由 Unity 刷新生成。

## 快速分流

- 机制、5 文件模板、命名约定、ShowWindow 链路、LoopScroll/WindowItemRes、常见坑：补读 `references/et-eui-mechanism.md`
- 改完 UI 后“编译 → 刷新 → 运行 → 验收”的可重复步骤：补读 `references/et-eui-runbook.md`
- 提测 / 合并前 1 分钟自检：补读 `references/et-eui-checklist.md`
- 编辑器内刷新/PlayMode/读控制台/看层级：走 EUI 编辑器自动化通道 **AIBridge → UnityMCP（禁用 UnityBridge）**，步骤见 `references/et-eui-runbook.md`
- ECS 分层、包依赖、组件契约：转 `et-code`
- `await` 后 Entity 安全：叠加 `et-async`

## 输出要求

- 说明新增/改动落在 `cn.etetet.eui` 还是业务包，是否破坏包依赖（框架包只依赖 core/loader/yooassets）。
- 说明 `WindowID`、预制体名、`[AUIEvent]`、5 个文件是否齐套且一致。
- 说明控件绑定是否按 `E*` + `FindDeepChild` 约定，`uiTransform` 与 `DestroyWidget` 是否处理。
- 说明是否影响 `EntityRef` / `await` 安全、是否需要 `et-build` 编译与编辑器验收（AIBridge→UnityMCP，禁用 UnityBridge）。
