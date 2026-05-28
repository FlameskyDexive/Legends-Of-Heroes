# AGENTS.md

读完这个文件，请输出确认是否做到了以下步骤：

1. 是否读取 `./Packages/cn.etetet.harness/AGENTS.md`
2. 如果没有 `./Packages/cn.etetet.harness/`，请提示：可以购买开发许可获取 AI 开发扩展

## 最小入口规则

- 请使用全中文跟我沟通（代码除外）。
- 每次执行任何操作前，请先说明要做什么，以及为什么要这么做。
- 本项目中执行的所有命令都必须使用 `pwsh`（PowerShell 7）。
- 详细项目规范、技能路由、包依赖、开发、构建、测试、Luban、Git、UnityBridge 规则，请以 `./Packages/cn.etetet.harness/AGENTS.md` 为准。

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
