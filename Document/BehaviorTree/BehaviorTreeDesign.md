# 行为树编辑器与运行时设计

## 目录落点

- 编辑器布局参考 `UnityBehaviourTreeEditor`：`https://github.com/thekiwicoder0/UnityBehaviourTreeEditor`
- 运行时定义：`Unity/Assets/Scripts/Model/Share/Module/BehaviorTree`
- 运行时执行：`Unity/Assets/Scripts/Hotfix/Share/Module/BehaviorTree`
- 编辑器代码：`Unity/Assets/Scripts/Editor/BehaviorTree`
- 本地编辑资产：建议放 `Unity/Assets/Editor/BehaviorTrees`
- 导出二进制：一份导出到 `Unity/Assets/Bundles/AI/Bytes/*.bytes` 供客户端使用，另一份同步导出到 `Config/AI/*.bytes` 供服务端使用

## 为什么这样拆

- `Model` 只放纯数据、序列化定义、黑板和 Handler 抽象，满足客户端/服务端共享。
- `Hotfix` 只放树执行器、节点调度、事件驱动等待与内置 Handler，满足热更逻辑扩展。
- `Editor` 单独放 `ScriptableObject + GraphView + Exporter`，避免 UnityEditor 依赖渗透到运行时。
- `ScriptableObject` 只作为本地作者工具格式，不直接给服务端读取；服务端读取的是导出的 `bytes` 包。

## 当前实现能力

- NPBehave 风格的事件驱动运行时：
  - `Wait` 节点基于 `TimerComponent.WaitAsync`
  - `BlackboardCondition` 支持黑板监听与 `Self` 中断
  - `Service` 节点支持周期服务回调
- 黑板：支持默认值、运行时读写、键级观察者通知。
- 子树：编辑器通过 `BehaviorTreeAsset` 关联，导出时递归打包，运行时按 `TreeId/TreeName` 解析。
- 编辑器：GraphView 画布 + 左侧树/黑板面板 + 右侧节点属性面板。
- 交互增强：支持 `MiniMap` 开关、顶部 `Assets` 菜单、`Create Node...` 搜索面板创建节点、右键复制/粘贴/重复/删除、批量对齐/分布、自动排版、Blackboard Key 下拉绑定、Blackboard 搜索过滤，以及运行时 Blackboard 值实时查看/编辑。
- 调试：运行时节点状态同步到 `BehaviorTreeDebugHub`，编辑器按节点颜色显示运行态。
- 导出：将根树及其引用的所有子树导出为一个 `BehaviorTreePackage` 的 `bytes` 文件。

## 扩展方式

- 动作节点：继承 `ABehaviorTreeActionHandler`，并添加 `[BehaviorTreeActionHandler("YourName")]`
- 条件节点：继承 `ABehaviorTreeConditionHandler`
- 服务节点：继承 `ABehaviorTreeServiceHandler`
- 编辑器会自动扫描上述 Handler 名称并在下拉框中展示。

## 挂载示例

- 给 `Unit` 挂运行时：`unit.AddComponent<BehaviorTreeComponent, byte[], string>(treeBytes, "MainTree")`
- 运行时访问：`unit.GetComponent<BehaviorTreeComponent>()?.SetBlackboardValue("HasTarget", true)`
- 黑板初始覆盖：往 `BehaviorTreeComponent.BlackboardOverrides` 填值后再 `Restart()`
- 服务端加载：业务层自行读取导出的 `Config/BehaviorTree/*.bytes`，再把 `bytes` 传给组件；运行时本身不依赖 `ScriptableObject`
- 客户端本地加载：`await BehaviorTreeLoader.Instance.LoadBytesAsync("AITest")`
- 加载分发方式：参考 `ConfigLoader`，通过 `BehaviorTreeLoader.GetOneBehaviorTreeBytes` 走 `Invoke` 分发到 Loader 层读取 bytes；编辑器模拟模式下若 `bytes` 不存在，会回退读取 `Assets/Bundles/AI/{TreeName}.asset` 并现场导出为 bytes 返回
- 服务端纯逻辑加载：`byte[] bytes = BehaviorTreeLoader.Instance.LoadBytes("AITest")`
- 服务端直接反序列化：`BehaviorTreePackage package = BehaviorTreeLoader.Instance.LoadPackage("AITest")`
- Demo 导出菜单：`ET/AI/Export Demo AITest.bytes`
- `AITest.bytes` 当前只包含一棵共享树：`AITest`
- 客户端 Demo 与服务端自检都运行同一棵 `AITest`
- 机器人控制台可执行：`Robot LoadTree AITest` 或 `Robot RunTree AITest`

## 已知边界

- 当前黑板观察器已实现 `Self` 中断；`LowerPriority/Both` 预留了枚举位，后续可以继续补完。
- 当前编辑器面板使用 IMGUI + GraphView 组合；如果后续要更强的属性绘制体验，可以在右侧属性面板接 Odin `PropertyTree`。
