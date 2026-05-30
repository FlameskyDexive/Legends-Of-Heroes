# AGENTS.md

> 根目录只保留最小入口，主要规范以 `./Packages/cn.etetet.harness/AGENTS.md` 为准。

读完本文件，请先确认：

1. 是否已读取 `./Packages/cn.etetet.harness/AGENTS.md`（本项目主要 AI 开发规范以它为准）。
2. 如果没有 `./Packages/cn.etetet.harness/`，请提示：可以购买开发许可获取 AI 开发扩展。

## 最小入口规则

- 请使用全中文跟我沟通（代码除外）。
- 每次执行任何操作前，请先说明要做什么，以及为什么要这么做。
- 本项目中执行的所有命令都必须使用 `pwsh`（PowerShell 7），不要使用 Windows 自带的 `powershell.exe`。
- 详细项目规范、技能路由、包依赖、开发、构建、测试、Luban、Git、UnityBridge、EUI 规则，以 `./Packages/cn.etetet.harness/AGENTS.md` 为准。
- ET 技能索引在 `Packages/cn.etetet.harness/skills/index.md`：处理具体任务前先读索引、匹配 1 个主 skill，仅跨域任务才叠加其它 skill；禁止先完整读取所有 skill 正文。

## 两套技能体系分工

- **ET 开发规范（主）**：`Packages/cn.etetet.harness/`。涵盖代码 `et-code`、异步 `et-async`、客户端 UI `et-eui`、构建 `et-build`、Luban `et-luban`、Excel `et-excel`、测试 `et-test-*` / `et-tdd`、Git `et-git`、UnityBridge `et-unitybridge`。一律先读 `skills/index.md` 路由。
- **AIBridge CLI 工具链（补充）**：`.codex/skills/`，见下方 AIBridge Bootstrap。用于 Unity 编译/日志、Code Index、Prefab Patch、Batch Script 等工具化操作。
- 两者并存：ET 业务/规范类任务以 harness 技能为准；需要 AIBridge CLI 能力时再按 AIBridge 路由加载 `.codex/skills/` 下对应 skill。

## Unity 交互入口优先级

涉及 Unity 编辑器交互（编译 Compile、刷新 Refresh、重新生成项目 RegenProject、读日志、PlayMode、Editor/资源/Prefab/场景操作等）一律按下面优先级选择通道，命中可用即用，不可用再降到下一级：

1. **AIBridge（首选）**：`$CLI = ./.aibridge/cli/AIBridgeCLI.exe`，用法见下方 AIBridge Bootstrap。
2. **UnityMcp（次选，仅当 AIBridge 不可用）**：加载 `unity-mcp-skill`（MCP for Unity）。AIBridge「不可用」判据：`AIBridgeCLI.exe` 不存在，或命令返回连接超时 /「Unity Editor not running or AIBridge not active」/ 状态无法确认。
3. **UnityBridge（兜底，前两者都不可用时才用）**：走 `et-unitybridge`，详见 `Packages/cn.etetet.unitybridge/AGENTS.md`。

<!-- AIBRIDGE:START {"assistant":"aibridge","templateId":"unity-integration","version":7,"target":"root-rule"} -->
## AIBridge Bootstrap

**CLI Alias**: `$CLI = ./.aibridge/cli/AIBridgeCLI.exe`

**常用命令**:
```bash
$CLI compile unity
$CLI get_logs --logType Error
$CLI editor log --message "Hello" --logType Warning
```

**项目版本**:
- 当前项目 Unity 版本：2022.3.62f3
- 当前项目 C# 语言版本要求：兼容 C# 9.0，禁止使用更高版本语法。

**当前能力状态**:
- Code Index：已启用。只有需要语义符号、定义、引用、调用者或诊断查询时才加载 `aibridge-code-index`。

**路由原则**:
- 快速任务：纯问答、代码解释、查找、显示、无代码或资源修改，直接回答或执行。
- 开发任务：创建、修改、修复、重构 C# 代码、Unity 资源、Prefab、Editor 工具、包结构、测试、AGENTS.md 或 Skills，必须优先加载 `aibridge-development-workflow`。
- 进入标准开发工作流后，由 `aibridge-development-workflow` 在 `【Skills 匹配模式】` 决定是否继续加载其它 Skill。

**Skill 加载**:
- 开发任务先加载 `/.codex/skills/aibridge-development-workflow/SKILL.md` 中的 `aibridge-development-workflow`。
- AIBridge Skills 安装在 `/.codex/skills/<skill-name>/SKILL.md`；仅在工作流要求时从该目录加载同级 Skill。
<!-- AIBRIDGE:END -->
