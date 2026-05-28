---
name: aibridge-development-workflow
description: "AIBridge/Unity 项目的标准开发工作流。Use when creating, modifying, fixing, refactoring, or validating C# code, Unity assets, prefabs, editor tools, package structure, tests, AGENTS.md, or local Skills. Preserves visible modes for Skills matching, requirement confirmation, analysis, implementation, and final checklist. Do not use for pure Q&A, simple search/display, or code explanation tasks with no file/code/asset changes."
---

# AIBridge Development Workflow

## 目标

在开发任务中保留清晰的阶段可视化，让用户知道当前 AI 正处于哪个工作阶段、加载了哪些 Skills，并在结束前执行明确的验收清单。

本 Skill 只放主流程。细节规则按需读取：

- `references/risk-gates.md`：需求确认、风险分级、何时必须暂停确认。
- `references/coding-rules.md`：C#、Unity、注释、硬编码、重复代码规则。
- `references/editor-generation.md`：复杂生成、分析、诊断、Runtime/Public API 调用等一次性 Editor C# 脚本规范。
- `references/checklist.md`：最终检查清单和收尾输出格式。

## 阶段总览

开发任务按以下模式推进：

`【Skills 匹配模式】 → 【需求确认模式】 → 【分析模式】 → 【方案实施模式】 → 【检查清单模式】`

简单低风险修改可以在一条回复中连续展示多个模式，但不能省略分析和最终检查清单。

## Skills 匹配模式

收到开发任务后，先输出实际使用的 Skills，仅列名称：

```text
【Skills 匹配模式】aibridge-development-workflow、aibridge
```

匹配规则：

- 所有开发任务至少包含 `aibridge-development-workflow`。
- 涉及 AIBridge CLI、Unity 编译、日志、代码文件查找、资源搜索、预制、场景或 Inspector 操作时，加入 `aibridge`。
- 涉及语义符号、定义、引用、实现、调用者或诊断查询时，只有 `aibridge-code-index` 已安装且项目规则未声明 Code Index 关闭，才加入 `aibridge-code-index`；否则使用 `rg` 和常规文件读取。
- 涉及复杂 Prefab 资源修改、批量 SerializedProperty、子物体/组件创建或引用写入时，加入 `aibridge-prefab-patch`。
- 涉及直接修改 Unity YAML 文本序列化文件（`.unity`、`.prefab`、`.asset`、`.mat`、`.controller` 等），或 AIBridge 不支持的 Prefab/Scene/ScriptableObjectTable 结构修改时，加入 `unity-yaml-editing`。
- 涉及 batch、multi、批处理脚本、长脚本、stdin 或脚本自动化时，加入 `aibridge-batch-script`。
- 涉及创建或修改 Skill 时，加入 `skill-creator`。
- 涉及复杂一次性 Editor 侧 C# 任务时，读取 `references/editor-generation.md`，优先评估 `.aibridge/code/*.csx`；包括复杂资源首次生成、调用项目 Runtime/Public API、场景/资源统计、诊断报告或多步骤 UnityEditor API 编排。

## 需求确认模式

先判断是否需要用户确认。低风险且需求明确时，不阻塞实施：

```text
【需求确认模式】
无需确认：需求明确，修改范围低风险，继续分析。
```

以下情况必须暂停并等待用户确认：

- 需求目标、交互行为、数据来源或验收标准不明确。
- 存在多个合理方案，且会影响公共 API、配置格式、资源结构或用户体验。
- 可能破坏兼容性、迁移数据、修改大量 Prefab/场景对象、删除资源或引入新依赖。
- 用户明确要求先给方案。

不确定时读取 `references/risk-gates.md`。

## 分析模式

任何文件、代码或资源修改前必须先分析。优先使用直接工具收集上下文：

- 用 `rg` / `rg --files` 搜索现有实现、工具类、命名模式和相邻代码。
- 涉及 Unity 对象、Prefab、资源、Console 时，结合 `aibridge` 查询。
- 涉及外部库或版本敏感信息时，只查官方文档。
- 代理工具只有在用户明确要求或当前环境允许时才使用；不可伪造已启动代理。

输出 2-3 句话的综合分析结果：

```text
【分析模式】
✅ 上下文收集：已通过 rg/AIBridge 查找现有实现和约束。
✅ 综合分析：项目使用 X 模式，影响范围集中在 Y，主要风险是 Z。
✅ 复杂度评估：常规问题，可以直接实施。
✅ 实施门控：条件满足，进入方案实施。
```

## 方案实施模式

实施前必须显示当前 Skills 和已加载的关键规范：

```text
【方案实施模式】
当前匹配 Skills：aibridge-development-workflow、aibridge
已加载规范：coding-rules、risk-gates
规范已加载，开始实施方案。
```

实施要求：

- 开发业务逻辑前，搜索现有工具类和相邻实现，避免重复造轮子。
- 优先遵循项目现有命名、目录、序列化和命令风格。
- 小步修改，每个子任务完成后做对应自测。
- 复杂逻辑添加必要的简体中文注释。
- 修改 C# 或 Unity 相关逻辑前读取 `references/coding-rules.md`。
- 使用一次性 Editor 脚本处理复杂生成、分析、诊断或 Runtime/Public API 调用前读取 `references/editor-generation.md`。

## 检查清单模式

所有开发任务结束前必须进入检查清单模式，读取 `references/checklist.md`，并按实际任务执行适用项。

输出必须包含通过、失败、已修复或不适用状态：

```text
【检查清单模式】
1. AIBridge 编译检查 ✅
2. Unity Console Error 检查 ✅
3. C# 9.0 兼容性 ✅
4. Unity 对象判空规范 ✅
5. 不适用项：Prefab dry-run（本次未修改 Prefab）

检查清单全部通过，任务完成。
```

如果检查无法执行，必须说明原因，不能假装通过。
