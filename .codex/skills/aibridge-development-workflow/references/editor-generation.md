# 一次性 Editor C# 脚本规范

## 适用场景

`.aibridge/code/*.csx` 适合复杂一次性 Editor 侧 C# 任务，再通过 `code execute` 执行。它不是所有 Unity 操作的默认入口，而是当声明式 CLI 命令不足以稳定表达任务时的高阶工具。

优先考虑：

- 首次生成复杂 Prefab、复杂场景、复杂 UI、3D 特效资源、批量材质/网格/粒子系统、多资源组合。
- 需要 Unity Editor API 才能安全表达的结构化创建或分析，例如 `PrefabUtility`、`EditorSceneManager`、`AssetDatabase`。
- 需要调用项目 Runtime/Public API 做一次性验证、迁移、数据生成、诊断或报告。
- 需要统计当前场景或资源状态并返回结构化报告，例如组件分布、Renderer/Collider/ParticleSystem 数量、缺失引用、资源依赖、性能预算。
- 操作步骤多、分支多，使用多条 CLI 命令会脆弱或难维护。

不优先：

- 生成正式业务逻辑脚本并直接挂载。需要新 MonoBehaviour/ScriptableObject 类型时，先写入 `Assets/*.cs`，执行 `compile unity`，再添加组件或创建资产。
- 简单场景对象操作：优先 `gameobject`、`transform`。
- 已有 Prefab 的局部结构修改：优先 `prefab patch --dryRun true`。
- 单个或少量 SerializedProperty 修改：优先 `inspector set_property/set_properties`。
- 可审计、可回滚、低风险的小改动：优先声明式 AIBridge 命令。
- 需要真实 Player 性能结论：优先 Unity Profiler、Performance Test 或目标平台验证。
- 需要直接编辑 Prefab/Scene YAML 的结构修改：最后才加载 `unity-yaml-editing`。

## 决策树

1. 新建复杂资源、多资源组合、复杂分析/诊断、Runtime/Public API 一次性调用：优先 `.aibridge/code/*.csx`。
2. 修改已有 Prefab 结构：优先 `prefab patch --dryRun true`，通过后再写入。
3. 修改属性：优先 `inspector set_property/set_properties`。
4. 简单场景对象操作：优先 `gameobject`、`transform`。
5. 上述都无法表达，且确认需要直接改 Unity 序列化文本时，才使用 `unity-yaml-editing`。

## 必须规则

1. 脚本文件放在 `.aibridge/code/<task-name>.csx`，长脚本不要用 `--code` 内联。
2. 输出资源集中到 `Assets/AIBridgeGenerated/<TaskName>/` 或用户指定目录。
3. 路径、资源名、尺寸、数量、颜色、Prefab 名等稳定参数定义为常量。
4. 脚本必须幂等：重复执行不会无限创建副本；必要时清理旧输出或复用已有资源。
5. 复杂对象结构使用 Unity Editor API 生成，不直接写 UnityYAML。
6. 优先使用 `AIBridgeGeneration` helper；仅在 helper 不覆盖时直接调用 `AssetDatabase`、`PrefabUtility`、`EditorSceneManager`。
7. 生成后返回结构化结果，至少包含创建/更新的 asset、prefab、scene、warning。
8. 执行后必须运行 `compile unity`，并检查 `get_logs --logType Error`。
9. Unity 2019 或老版本首次执行复杂脚本前，先用最小 `code execute` 做 smoke test，确认 Roslyn 编译器路径可用。
10. `code execute` 不能并发；超时后先用 `code status` 确认 Unity 端异步任务是否仍在收尾，必要时用 `code cancel` 释放 AIBridge 等待状态。
11. 编译器输出按 UTF-8 读取；中文诊断仍可能受 Roslyn/系统输出影响，优先查看结果里的结构化 `diagnostics`。
12. 用户脚本末尾已有顶层 `return` 或 `throw` 时，包装器不会追加 fallback return，避免 `CS0162` 噪声。

## 推荐脚本模板

```csharp
const string TaskName = "MyEffect";
const string OutputRoot = "Assets/AIBridgeGenerated/" + TaskName;
const string PrefabPath = OutputRoot + "/" + TaskName + ".prefab";

var result = new AIBridgeGenerationResult();

AIBridgeGeneration.EnsureFolder(OutputRoot);
AIBridgeGeneration.DeleteAssetIfExists(PrefabPath);

var root = new GameObject(TaskName);
try
{
    var material = AIBridgeGeneration.LoadOrCreateMaterial(
        OutputRoot + "/Glow.mat",
        "Standard",
        result);
    material.color = Color.cyan;

    var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    body.name = "Core";
    body.transform.SetParent(root.transform, false);
    body.GetComponent<Renderer>().sharedMaterial = material;

    AIBridgeGeneration.SavePrefab(root, PrefabPath, result);
    AIBridgeGeneration.RefreshAssets();
}
finally
{
    UnityEngine.Object.DestroyImmediate(root);
}

return result;
```

## Helper API

优先使用这些 Editor helper，减少重复样板和路径错误：

```csharp
AIBridgeGeneration.EnsureFolder("Assets/AIBridgeGenerated/MyTask");
AIBridgeGeneration.DeleteAssetIfExists("Assets/AIBridgeGenerated/MyTask/Old.prefab");
AIBridgeGeneration.LoadOrCreateMaterial("Assets/AIBridgeGenerated/MyTask/Mat.mat", "Standard", result);
AIBridgeGeneration.AddOrGetComponent<BoxCollider>(gameObject);
AIBridgeGeneration.SavePrefab(root, "Assets/AIBridgeGenerated/MyTask/Root.prefab", result);
AIBridgeGeneration.SaveScene(scene, "Assets/AIBridgeGenerated/MyTask/Scene.unity", result);
AIBridgeGeneration.MarkDirty(asset);
AIBridgeGeneration.RefreshAssets();
```

## 执行流程

```powershell
$CLI code execute --code "return 1;" --timeout 5000
$CLI code execute --file ".aibridge/code/my_effect.csx" --timeout 30000
$CLI code status
$CLI code cancel
$CLI compile unity --timeout 120000
$CLI get_logs --logType Error --count 20
```

## 结果要求

脚本返回 `AIBridgeGenerationResult` 或等价结构：

- `assets`：创建或更新的普通资源路径。
- `prefabs`：创建或更新的 Prefab 路径。
- `scenes`：创建或更新的 Scene 路径。
- `warnings`：非阻断警告。
- `messages`：关键信息。

只读分析或诊断脚本也应返回结构化对象，例如：

- `summary`：结论摘要。
- `counts`：关键计数。
- `warnings`：潜在风险。
- `items`：需要用户查看的明细。
