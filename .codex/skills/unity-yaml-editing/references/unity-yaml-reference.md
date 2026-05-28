# UnityYAML 直接修改规范

## 依据

- Unity 2019.4 Manual - UnityYAML: https://docs.unity.cn/cn/2019.4/Manual/UnityYAML.html
- Unity 2019.4 Manual - 文本场景格式: https://docs.unity.cn/cn/2019.4/Manual/TextSceneFormat.html
- Unity 2019.4 Manual - 脚本序列化: https://docs.unity.cn/cn/2019.4/Manual/script-Serialization.html
- Unity 2019.4 Manual - ClassId索引: https://docs.unity.cn/cn/2019.4/Manual/ClassIDReference.html

Unity 官方说明：UnityYAML 是 Unity 为资源序列化定制的 YAML 子集，外部工具不应生成或编辑它。实际必须手改时，把它当作危险兜底流程，尽量小改并让 Unity 重新导入验证。

## 文件识别

常见文本序列化资源：

- `.unity`：Scene。
- `.prefab`：Prefab / Prefab Variant。
- `.asset`：ScriptableObject、ScriptableObjectTable、自定义配置、部分 ProjectSettings。
- `.mat`、`.controller`、`.anim`、`.overrideController` 等 Unity 资源。

编辑前确认文件头或结构：

```yaml
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &123456789
GameObject:
```

含 `--- !u!<classID> &<fileID>` 的文件由多个 Unity 对象文档组成；`classID` 决定对象类型，`fileID` 是同文件内本地引用锚点。

## 首选工具顺序

1. `aibridge inspector get_properties/find_property/set_property/set_properties`：字段读写、普通 `.asset`、Prefab asset 的组件属性。
2. `aibridge-prefab-patch`：Prefab 内 `ensure_child`、`ensure_component`、`set_property`、`set_properties`、`set_array`、`append_array`、`clear_array`、GameObject/Component/Asset 引用。
3. UnityYAML 直接编辑：AIBridge 不能表达的 Scene、Prefab、Prefab Variant、ScriptableObjectTable、复杂 `.asset` 创建/结构修改。

### 资源修改推荐矩阵

| 目标 | 推荐方式 |
|---|---|
| 单个可读字段 | `inspector set_property` |
| 同一目标多个字段 | `inspector set_properties` |
| Prefab 内新增/确保子物体、组件、数组、引用 | `prefab patch`，先 `--dryRun true` |
| Scene 对象创建、移动、Transform 修改 | `gameobject`、`transform`、`scene`、`inspector` |
| Unity 能安全生成的复杂资产 | 临时 Editor 脚本或现有 Unity/AIBridge API |
| API 不支持的文本序列化结构、修复、批量 authoring | `unity-yaml-editing` |

直接 YAML 不是默认路径；只有当上面 API 不能表达目标、或需要修复/生成明确的 UnityYAML 结构时才使用。

## UnityYAML 核心格式

每个对象文档形态：

```yaml
--- !u!<classID> &<fileID>
<ClassName>:
  m_ObjectHideFlags: 0
  ...
```

常见 `classID`：

| classID | 类型 |
|---:|---|
| 1 | GameObject |
| 4 | Transform |
| 20 | Camera |
| 23 | MeshRenderer |
| 33 | MeshFilter |
| 114 | MonoBehaviour |
| 115 | MonoScript |
| 1001 | PrefabInstance |
| 1003 | Prefab |

对象引用形态：

```yaml
{fileID: 123456789}
{fileID: 11500000, guid: abcdef0123456789abcdef0123456789, type: 3}
{fileID: 0}
```

- 只有 `fileID`：引用同一 YAML 文件中的本地对象。
- `fileID + guid + type`：引用外部资源。脚本通常是 `{fileID: 11500000, guid: <script-meta-guid>, type: 3}`。
- `{fileID: 0}`：空引用。

## 编辑前检查

1. 确认 Unity 项目启用文本序列化；二进制资源不能按 YAML 改。
2. 读目标文件和对应 `.meta`；涉及脚本时读脚本 `.meta` 的 `guid`。
3. 搜索目标 `fileID`、`guid`、对象名、组件名，理解引用关系。
4. 找同类型现有对象作为模板，不凭记忆写字段结构。
5. 记录新增 `fileID` 列表，确保不与同文件任意 `&<fileID>` 或 `{fileID: <id>}` 冲突。
6. 新建资产时，先搜索计划使用的 `.meta` GUID，确认项目中未出现。
7. 对 Light、Camera、ParticleSystem、Renderer、URP/HDRP 组件等版本敏感结构，必须复制同项目、同 Unity 版本、同渲染管线样例。

## 修改规则

### 保持格式

- 保留 `%YAML 1.1`、`%TAG` 和每个文档头。
- 使用两个空格缩进，沿用周围列表写法。
- 不用通用 YAML formatter，它可能删除 Unity tag、改变锚点或重排字段。
- 不批量重排字段；只改必要行。
- 字符串优先沿用原文件风格。含冒号、井号、前后空格或特殊字符时加引号。

### fileID 与引用

- 修改已有对象时，尽量不改 `fileID`。
- 新增对象时，使用文件内未出现过的较大整数或沿用 Unity 生成风格；同一新增关系内保持一致。
- 新增 Component 文档后，必须把组件引用加入所属 `GameObject.m_Component`。
- 新增 GameObject 子节点后，必须更新父 Transform 的 `m_Children`，并设置子 Transform 的 `m_Father`。
- 删除对象时，必须删除所有引用它的列表项和字段；无法确认引用完整性时不要删除。

Prefab 中新增节点或组件的最小双向关系：

| 新增内容 | 必须同步 |
|---|---|
| GameObject + Transform | GameObject 文档、Transform 文档、`GameObject.m_Component`、`Transform.m_GameObject` |
| 根节点 | 根 Transform 的 `m_Father: {fileID: 0}` |
| 子节点 | 父 `Transform.m_Children` 追加子 Transform，子 `Transform.m_Father` 指向父 Transform |
| 新组件 | 组件文档、所属 `GameObject.m_Component`、组件 `m_GameObject` |
| 外部资源引用 | 使用稳定 `{fileID, guid, type}`，不要修改被引用资源 `.meta` GUID |

### MonoBehaviour / ScriptableObject

MonoBehaviour 和 ScriptableObject 文档通常是 `!u!114`：

```yaml
--- !u!114 &123456789
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 111111111}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: <script-guid>, type: 3}
  m_Name:
  m_EditorClassIdentifier:
```

- Prefab/Scene 上的 MonoBehaviour 需要 `m_GameObject` 指向所属 GameObject。
- ScriptableObject `.asset` 通常没有 `m_GameObject` 和 `m_Enabled`，以同项目同类 `.asset` 为准。
- `m_Script` 必须使用目标 `.cs.meta` 的 `guid`，不得改变已有脚本 GUID。
- 字段名来自 C# 序列化字段名，不一定等于 Inspector 显示名。

### GameObject / Transform 基本关系

新增 GameObject 至少需要 GameObject 文档和 Transform/RectTransform 文档：

```yaml
--- !u!1 &100000001
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 100000002}
  m_Layer: 0
  m_Name: NewObject
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &100000002
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 100000001}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
```

不同 Unity 版本字段可能不同；复制同文件同类型对象的结构更安全。

### Prefab / Prefab Variant

- 普通 Prefab 优先使用 `aibridge-prefab-patch`。
- Prefab Variant 通常包含 `PrefabInstance`、`m_Modification.m_Modifications`、`m_AddedGameObjects`、`m_AddedComponents` 等结构。直接编辑前必须找同项目变体样例。
- 不要随意扁平化 Variant 或删除 `m_SourcePrefab`、`m_CorrespondingSourceObject`、`m_PrefabInstance`、`m_PrefabAsset`。
- 对 Variant 的 override，优先追加/修改对应 modification 记录；不确定结构时通过 Unity API 或手动生成一个样例再复制形状。

### 从零创建普通 Prefab

仅在 Unity/AIBridge API 不适合生成目标结构时使用。最小文件集合：

1. `<Name>.prefab`：包含 `%YAML`、`%TAG`、至少一个 GameObject 文档和一个 Transform 文档。
2. `<Name>.prefab.meta`：包含唯一 `guid` 和 `PrefabImporter`。
3. 根 GameObject 的 `m_Component` 引用根 Transform。
4. 根 Transform 的 `m_GameObject` 指向根 GameObject，`m_Father: {fileID: 0}`。
5. 每个子节点都同时具备 GameObject、Transform、父子引用。
6. 每个组件都在 `GameObject.m_Component` 中注册，并让组件 `m_GameObject` 指回所属 GameObject。

最小 `.meta` 形状：

```yaml
fileFormatVersion: 2
guid: <unique-32-hex-guid>
PrefabImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
```

创建后必须导入并确认：

```bash
$CLI asset import --assetPath "Assets/Generated/Name.prefab"
$CLI prefab get_info --prefabPath "Assets/Generated/Name.prefab"
$CLI prefab get_hierarchy --prefabPath "Assets/Generated/Name.prefab" --depth 8 --includeInactive true
```

### 增量追加 Prefab 节点 / 组件

1. 分配未使用的 GameObject、Transform、组件 fileID。
2. 添加新 GameObject 文档。
3. 添加新 Transform 文档，设置 `m_GameObject`、`m_Father`、`m_Children`。
4. 把新 Transform 加入父 Transform 的 `m_Children`。
5. 添加组件文档并设置 `m_GameObject`。
6. 把组件 fileID 加入所属 `GameObject.m_Component`。
7. 导入后用 `prefab get_hierarchy` 和 `inspector get_properties` 复查。

常见失败点：

- 只新增 Transform，忘记父 `m_Children`。
- 只新增组件文档，忘记 `GameObject.m_Component`。
- 复制组件时保留了旧 `m_GameObject`。
- 新 `.meta` GUID 与项目已有 GUID 冲突。
- 用了其它 Unity 版本或其它渲染管线的组件字段。

### Scene `.unity`

- Scene 由多个对象文档组成，根对象 Transform 的 `m_Father` 为 `{fileID: 0}`。
- 新增根对象时，只需对象文档之间引用自洽；新增子对象时还要更新父子 Transform。
- Lightmap、NavMesh、Occlusion、Timeline、PrefabInstance 等场景系统对象引用复杂；除简单字段修改外，优先用 Unity/AIBridge API 或生成样例复制。

### `.asset` / ScriptableObjectTable

- 简单字段、数组、对象引用优先走 `inspector set_property/set_properties`。
- AIBridge 不支持创建或结构化重写时，可直接改 YAML。
- 创建新的 `.asset` 最好通过 Unity 创建以生成稳定 `.meta`；手工创建时必须同时创建合法 `.meta` 并保证 GUID 唯一。
- ScriptableObjectTable 或类似表格资产通常有嵌套数组/字典替代结构，必须复制同类型资产的数据形状，避免自行发明字段。

## 浮点数

Unity 文档说明，手工编辑浮点值时应使用十进制表示。Unity 可能生成十六进制 IEEE 754 形式用于精确表示；不要为了“格式统一”改写未触碰的浮点值。

推荐：

```yaml
m_LocalPosition: {x: 1.5, y: 0, z: -2}
```

避免手写：

```yaml
m_LocalPosition: {x: 0x3fc00000(1.5), y: 0, z: 0}
```

## 可接受的直接修改场景

- 修改清晰的 ScriptableObject `.asset` 标量、数组、字符串、资源引用。
- 为不支持的自定义 `.asset` 增加表项，且已有同类表项可复制。
- 修改 `.unity` / `.prefab` 中无法通过 AIBridge 表达的简单结构，且引用关系可完整追踪。
- 直接创建普通 Prefab 或追加节点/组件，且已从同项目样例复制组件结构，并能用 AIBridge 导入/复查。
- 修复明显损坏的 GUID/fileID/缩进/冲突，且有可验证来源。

## 禁止或需暂停确认

- 大量删除对象或批量重排 fileID。
- 修改导入器 `.meta` 的 GUID、脚本 `.meta` GUID 或第三方资源 GUID。
- 不理解 Prefab Variant modification 结构时直接改 override。
- 需要生成复杂动画、AnimatorController、Timeline、LightingData 等高耦合资源。
- 无法通过 Unity 重新导入或编译验证。

## 验证清单

1. `rg '&<newFileID>|fileID: <newFileID>' <file>` 确认新增 ID 唯一且引用完整。
2. 检查 YAML 头、文档分隔符、缩进、列表项格式未被整体重写。
3. 涉及脚本引用时，确认 `<script>.cs.meta` GUID 与 `m_Script.guid` 一致。
4. 新建资产时，`rg "<new-guid>" Assets Packages ProjectSettings` 确认 GUID 只出现在预期 `.meta` 或引用处。
5. 涉及 Prefab/Scene 时，先 `$CLI asset import --assetPath "<path>"`，再查询 hierarchy/properties。
6. 新增 Prefab 节点/组件时，确认：
   - 父 `Transform.m_Children` 包含子 Transform。
   - 子 `Transform.m_Father` 指回父 Transform。
   - `GameObject.m_Component` 包含所有组件。
   - 每个组件 `m_GameObject` 指向所属 GameObject。
7. 执行 `$CLI compile unity`。
8. 执行 `$CLI get_logs --logType Error`，确认没有 YAML parse/import/serialization 错误。
9. 若修改用户可见资源，按需求打开场景/Prefab 或截图验证。
