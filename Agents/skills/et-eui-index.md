# et-eui-index - EUI技能文档导航

用于统一导航 EUI 相关文档，按“理解机制 → 执行流程 → 提测自检”顺序使用。

---

## 1. 文档总览

- 机制说明（完整）：`Agents/skills/et-eui.md`
  - 适合：要理解 EUI 的 Prefab、绑定、生成、运行链路。
- 执行流程（实操）：`Agents/skills/et-eui-runbook.md`
  - 适合：改完 UI 后按步骤自动化执行。
- 提测自检（勾选）：`Agents/skills/et-eui-checklist.md`
  - 适合：提测/合并前 1 分钟快速核查。

---

## 2. 推荐使用顺序

### 场景A：首次接手EUI

1. 阅读：`Agents/skills/et-eui.md`
2. 演练：`Agents/skills/et-eui-runbook.md`
3. 提测：`Agents/skills/et-eui-checklist.md`

### 场景B：日常改UI并验证

1. 直接执行：`Agents/skills/et-eui-runbook.md`
2. 提测前勾选：`Agents/skills/et-eui-checklist.md`

### 场景C：排查疑难问题

1. 先看原理：`Agents/skills/et-eui.md`
2. 再按回放步骤复现：`Agents/skills/et-eui-runbook.md`

---

## 3. 与其它skills协作

- 架构规范检查：`/et-arch`
- 编译构建：`/et-build`
- 测试执行：`/et-test-run`

建议：

- EUI 相关任务优先使用 `et-eui` 文档组。
- 涉及架构规范、编译、测试时再串联对应 skill。

---

## 4. 维护规则

- 修改 EUI 机制时，同步更新：`et-eui.md`
- 调整执行步骤时，同步更新：`et-eui-runbook.md`
- 变更提测标准时，同步更新：`et-eui-checklist.md`
- 每次更新后，回到本索引检查链接是否完整。

