# 检查清单

## 选择规则

所有开发任务结束前必须执行本清单。按任务实际情况标记 `✅`、`❌`、`已修复` 或 `不适用`。

## 通用检查

1. 修改范围符合用户需求，没有无关重构。
2. 已遵循现有目录、命名、编码和序列化模式。
3. 无明显重复代码；同一业务规则重复时已提取公共逻辑。
4. 无不必要 hard code；业务配置、路径、魔法数字已集中为配置或常量。
5. 未回滚或覆盖用户无关改动。

## C# / Unity 检查

1. C# 9.0 兼容，无 C# 10.0+ 语法。
2. using 和命名空间正确。
3. Unity 对象使用 `!= null` 显式判空，未使用 `?.`。
4. 复杂逻辑有必要的简体中文注释。
5. SerializedProperty、Prefab、Scene、`.asset` 修改优先使用 AIBridge/Unity API，不直接改 YAML，除非没有可行 API。
6. 直接修改 UnityYAML 时，已加载 `unity-yaml-editing` 并完成 fileID/GUID/缩进/引用完整性检查。

## AIBridge 验证

`$CLI` 表示当前平台的 AIBridge CLI 调用方式，Windows 项目通常是 `./.aibridge/cli/AIBridgeCLI.exe`。

涉及 C# 或 Unity 逻辑修改时执行：

```bash
$CLI compile unity
```

涉及 Unity Console 风险时检查 Error 日志：

```bash
$CLI get_logs --logType Error
```

涉及复杂 Prefab 修改时：

1. 先执行 `prefab patch --dryRun true`。
2. 写入后复查 hierarchy/properties。
3. 再执行 `compile unity` 和 Error 日志检查。

涉及直接 UnityYAML 修改时：

1. 确认没有可行 AIBridge/Unity API 路径，或该路径不足以表达目标操作。
2. 检查新增/修改的 `fileID`、`guid`、`m_Script`、`.meta`、父子/组件双向引用。
3. 新建或增量修改 Prefab 时，复查 `GameObject.m_Component`、`Transform.m_Children`、`Transform.m_Father`、组件 `m_GameObject`。
4. 写入后执行 Unity 导入、AIBridge hierarchy/properties 复查、`compile unity` 和 Error 日志检查。

## 输出格式

```text
【检查清单模式】
1. 修改范围检查 ✅
2. C# 9.0 兼容性 ✅
3. Unity 对象判空规范 ✅
4. AIBridge 编译检查 ❌ → 已修复并重新检查 ✅
5. 不适用项：Prefab dry-run（本次未修改 Prefab）

检查清单全部通过，任务完成。
```

无法执行的检查必须说明原因，不能标记为通过。
