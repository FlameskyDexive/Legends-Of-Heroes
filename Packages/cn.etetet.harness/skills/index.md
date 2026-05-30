# ET Skills 索引

先读本文件，禁止先完整读取所有 skill 正文。

## 加载策略

1. 先用场景匹配 1 个主 skill；只有跨域任务才叠加其它 skill。
2. 先读命中的 `SKILL.md`；harness 内技能位于 `skills/{skill-name}/SKILL.md`，包内专属技能按索引给出的包路径读取；需要细节时再读该 skill 的 `references/*.md`。
3. 能直接调用现成脚本或 CLI 时，优先现成入口，不重复展开长命令。
4. 所有命令必须使用 `pwsh`（PowerShell 7），绝对不要使用 Windows 自带的 `powershell.exe`。
5. 测试相关任务：完整 TDD 用 `et-tdd`，只写测试用 `et-test-write`，只执行或排查测试用 `et-test-run`；命中后读取 `Packages/cn.etetet.test/AGENTS.md`，由 test 包说明继续分流到具体 skill。
6. 涉及 Unity 编辑器内操作时，通道优先级固定为 **AIBridge（`./.aibridge/cli/AIBridgeCLI.exe`）→ UnityMcp（`unity-mcp-skill`，仅 AIBridge 不可用时）→ UnityBridge（`et-unitybridge`，兜底）**；选到 UnityBridge 时再读取 `Packages/cn.etetet.unitybridge/AGENTS.md` 继续分流。完整判据见根 `AGENTS.md` 的「Unity 交互入口优先级」。
7. 修改或新增 C# 代码时默认遵守“每个类一个文件”；除天然绑定的极小枚举/结构外，不要把多个类塞进同一 `.cs`。

## 任务入口

### 核心开发

- `et-code`
  - 场景：Entity / Component / System / Helper、包依赖、程序集、Handler 结构、组件契约、Module analyzer、分析器报错、新建或移动 C# 文件
  - 补读：`skills/et-code/SKILL.md`
- `et-async`
  - 场景：`async` / `await` / `ETTask` / `ETCancellationToken`、`EntityRef` await 安全、`NewContext(...)`、并发等待
  - 注意：改任何含 `async` / `ETTask` 的代码时，叠加此 skill
  - 补读：`skills/et-async/SKILL.md`

### 客户端 UI

- `et-eui`
  - 场景：新建/改 `DlgXxx` 窗口、`ESXxx` 子 UI、`ItemXxx` 循环项、`AUIEvent` 处理器、`WindowID` 注册、`E*` 控件绑定、ShowWindow/Hide/Close 链路、LoopScrollRect
  - 注意：本项目 EUI 在 `cn.etetet.eui`，无代码生成菜单，按 5 文件模板手写；编辑器验收走 **AIBridge → UnityMCP（`unity-mcp-skill`），EUI 一律不走 UnityBridge**
  - 补读：`skills/et-eui/SKILL.md`

### Unity 编辑器操作

> 通道优先级：AIBridge → UnityMcp（`unity-mcp-skill`）→ UnityBridge（`et-unitybridge`）。详见规则 6 与根 `AGENTS.md` 的「Unity 交互入口优先级」。下面的 `et-unitybridge` 是兜底通道。

- `et-unitybridge`
  - 场景：查询 Unity 心跳/状态、AI 操作 Unity Editor、资源/场景/对象/Inspector/Prefab/GameView/截图/Editor 测试、触发编译/刷新/重新生成项目/进入退出 PlayMode/热更新 Reload、排查桥接返回
  - 补读：`Packages/cn.etetet.unitybridge/AGENTS.md`

### 构建与导出

- `et-build`
  - 场景：编译（`dotnet build ET.sln`）、导出 Proto、启动服务器、发布
  - 补读：`skills/et-build/SKILL.md`
- `et-luban`
  - 场景：导出 Luban 生成的 C# 配置代码与 C# 数据代码、刷新聚合 `luban.conf`、核对 `CodeMode/Model` / `CodeMode/Config`、排查 `ET.ExcelExporter` / `LubanGen.ps1` / `luban.conf`
  - 补读：`skills/et-luban/SKILL.md`

### 测试验证

- `et-tdd`
  - 场景：TDD 驱动开发、完整测试闭环（设计 -> 测试方案 -> 编写 -> 编译 -> 运行 -> 回归）
  - 叠加：`et-test-write`、`et-test-run`、涉及业务代码时 `et-code`
  - 补读：`Packages/cn.etetet.test/AGENTS.md`
- `et-test-write`
  - 场景：编写或修改 `ATestHandler` 测试用例、补 `Test.md` 或最小验证清单
  - 补读：`Packages/cn.etetet.test/AGENTS.md`
- `et-test-run`
  - 场景：执行测试、查看 `Logs/All.log`、调试测试失败、做回归验证
  - 补读：`Packages/cn.etetet.test/AGENTS.md`

### 数据与提交

- `et-excel`
  - 场景：通过 `ET.ExcelMcp` 读写 Excel、维护 Luban 配置表、批量导入导出、样式 / 公式 / 图表 / 工作表操作
  - 补读：`skills/et-excel/SKILL.md`
- `et-git`
  - 场景：提交前检查、整理 `git status` / `git diff`、筛除无关文件、编写中文提交信息、与远端 rebase 同步
  - 补读：`skills/et-git/SKILL.md`

## 组合场景

- 改普通 ET 代码：`et-code` -> 涉及异步叠加 `et-async` -> 必要时 `et-build` / `et-test-run`
- 创建新功能或修 Bug：`et-tdd` -> `et-test-write` -> `et-code` -> `et-build` -> `et-test-run`
- 只做异步链路梳理或 review：`et-async` -> 必要时 `et-code`
- 架构合规检查：`et-code`
- 改客户端 UI：`et-eui` -> 涉及异步叠加 `et-async` -> `et-build` 编译 -> 编辑器验收 AIBridge→UnityMCP（EUI 禁用 UnityBridge）
- 做 Unity 编辑器内操作：AIBridge 优先 -> 不可用则 `unity-mcp-skill` -> 仍不可用则 `et-unitybridge` -> 确认就绪后执行对应操作
- 改配置表并导出：`et-excel` -> `et-luban` -> 必要时 `et-build`
- 只做 Luban 导出：`et-luban`
- 只做编译 / Proto / 运行 / 发布：`et-build`
- 只执行测试或排查测试失败：`et-test-run`
- 准备提交或审查 diff：`et-git`
- 完成开发准备提交：对应开发 skill -> 必要时 `et-build` / `et-test-run` -> `et-git`
