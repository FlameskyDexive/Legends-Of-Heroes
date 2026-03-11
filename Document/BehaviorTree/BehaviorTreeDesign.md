# 行为树编辑器与运行时设计

## 目录落点

- 运行时定义：`Unity/Assets/Scripts/Model/Share/Module/BehaviorTree`
- 运行时执行：`Unity/Assets/Scripts/Hotfix/Share/Module/BehaviorTree`
- 编辑器代码：`Unity/Assets/Scripts/Editor/BehaviorTree`
- 本地编辑资产：建议放 `Unity/Assets/Editor/BehaviorTrees`
- 导出二进制：默认导出到 `Config/BehaviorTree/*.bytes`

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

## 已知边界

- 当前黑板观察器已实现 `Self` 中断；`LowerPriority/Both` 预留了枚举位，后续可以继续补完。
- 当前编辑器面板使用 IMGUI + GraphView 组合；如果后续要更强的属性绘制体验，可以在右侧属性面板接 Odin `PropertyTree`。
