# 行为树功能设计文档

## 1. 文档目的

本文档用于整理当前项目行为树系统的功能设计、代码结构、运行流程、扩展方式、性能特征与已知边界，作为后续开发、维护与排障的统一参考。

当前实现同步分派架构：

- 运行入口：`BTDispatcher.Instance.Handle(node, env)`
- 运行时核心：`BTExecutionSession + BTEnv + BTNode`
- 导出格式：`BTPackage / BTDefinition / BTNodeData`
- 运行时建图：`BTGraphBuilder`
- 异步兼容：`BTFlowDriver` + `Wait/Service` 异步桥接

## 2. 设计目标

### 2.1 核心目标

- 保持编辑器与 bytes 导出格式稳定
- 支持客户端与服务端共享树定义
- 支持热更层扩展节点执行逻辑
- 运行时按节点类型分派，减少字符串分发的核心占比
- 保留 `Wait`、`Service`、`Blackboard Self Abort` 等能力
- 让节点双击跳转、脚本定位


## 3. 目录结构

### 3.1 Core

位置：`Unity/Assets/Scripts/Core/World/Module/BehaviorTree`

职责：只保留 Loader 可见的最小契约。

当前保留：

- `BTBytesLoader`
  - `ClientBehaviorTreeBytesDir`
  - `ServerBehaviorTreeBytesDir`
  - `BTAssetDir`
  - `GetOneBehaviorTreeBytes`

### 3.2 Model

位置：`Unity/Assets/Scripts/Model/Share/Module/BehaviorTree`

职责：共享定义、运行时对象图、运行时静态上下文、加载门面。

主要内容：

- 导出数据层
  - `BTPackage`
  - `BTDefinition`
  - `BTNodeData`
  - 各类 `xxxNodeData`
- 运行时对象图
  - `BTNode`
  - `BTRoot`
  - `BTComposite`
  - `BTDecorator`
  - `BTAction`
  - `BTCondition`
  - `BTService`
  - `BTWait`
  - `BTBlackboardCondition`
  - `BTSubTreeCall`
- 分派基础设施
  - `IBTHandler`
  - `ABTNodeHandler<TNode>`
  - `BTDispatcher`
- 运行时上下文
  - `BTEnv`
  - `BTExecutionSession`
  - `BTExecutionContext`
- 数据辅助
  - `BTBlackboard`
  - `BTValueUtility`
  - `BTSerializer`
  - `BTDebugHub`
  - `BTDebugSnapshot`
- 加载器
  - `BTLoader`

### 3.3 Hotfix

位置：`Unity/Assets/Scripts/Hotfix/Share/Module/BehaviorTree`

职责：承载行为树全部执行逻辑与运行时推进逻辑。

主要内容：

- 建图：`BTGraphBuilder`
- 运行驱动：`BTFlowDriver`
- 结构节点 handler：
  - `BTRootHandler`
  - `BTSequenceHandler`
  - `BTSelectorHandler`
  - `BTParallelHandler`
  - `BTInverterHandler`
  - `BTSucceederHandler`
  - `BTFailerHandler`
  - `BTRepeaterHandler`
  - `BTBlackboardConditionHandler`
  - `BTSubTreeCallHandler`
- 叶子桥接 handler：
  - `BTActionCallHandler`
  - `BTConditionCallHandler`
  - `BTServiceCallHandler`
  - `BTWaitHandler`
- 旧叶子扩展点调度器：
  - `BTActionDispatcher`
  - `BTConditionDispatcher`
  - `BTServiceDispatcher`
- 挂载入口：`BTComponentSystem`

### 3.4 Editor

位置：`Unity/Assets/Scripts/Editor/BehaviorTree`

职责：作者工具、节点描述、建树、导出、运行时脚本跳转。

主要内容：

- `BTAsset`
- `BTEditorWindow`
- `BTEditorRuntimeBridge`
- `BTGraphView`
- `BTEditorRuntimeNodeFactory`
- `BTExporter`
- `BTNodeDescriptor` 体系
- `BTEditorUtility`

当前 Editor 侧采用纯反射思路：

- Editor 不再静态引用 `Unity.Model`
- Editor 通过 `BTEditorRuntimeBridge` 按需反射访问运行时定义与序列化入口
- 行为树运行时主体仍尽量保留在 `Model / Hotfix`

这使得：

- 打开 `BTAsset` Inspector 与行为树编辑器窗口时，不再因为静态字段/类型直接依赖 `Unity.Model` 而触发 `TypeLoadException`
- 导出 bytes 时，Editor 仍然可以构造运行时 `BTPackage / BTDefinition / BTNodeData`

### 3.5 Gameplay 扩展点

共享叶子定义位置：`Unity/Assets/Scripts/Model/Share/GamePlay/AI/BehaviorTree`

热更叶子 handler 位置：`Unity/Assets/Scripts/Hotfix/Share/GamePlay/AI/BehaviorTree`

当前示例：

- `BTPatrolNodeData`
- `BTHasPatrolPathNodeData`
- `BTPatrolAction`
- `BTHasPatrolPathCondition`

## 4. 数据模型

### 4.1 两层表示

当前实现保留两层模型：

#### 导出/持久化层

- `BTPackage`
- `BTDefinition`
- `BTNodeData`
- 各类 `xxxNodeData`

这层用于：

- 编辑器产物
- bytes 序列化/反序列化
- 运行时建图输入

#### 运行时对象层

- `BTNode`
- 及其各种运行时节点子类

这层用于：

- 同步分派
- 运行时状态推进
- 异步恢复

### 4.2 当前数据类命名规范

当前统一使用 `xxxData` 作为节点数据类命名后缀，例如：

- `BTWaitNodeData`
- `BTParallelNodeData`
- `BTServiceNodeData`
- `BTLogNodeData`
- `BTPatrolNodeData`

后续新增节点数据类也应遵循 `xxxData` 命名规范。

### 4.3 `BTNodeData` 是否仍有运行时作用

有。

当前 `BTNodeData` 的作用主要是：

- 行为树运行时建图输入
- 作为运行时节点 `BTNode.Definition` 的原始配置来源
- 作为参数、标题、注释、子树信息等静态配置载体

注意：

- 运行时节点不再重复定义和保存 `BTNodeData` 中的配置字段
- 节点配置统一从 `node.Definition` 读取

## 5. 运行时对象模型

### 5.1 `BTNode`

运行时节点公共基类包含：

- `RuntimeNodeId`
- `SourceNodeId`
- `TreeId`
- `TreeName`
- `Definition`
- `Children`

其中：

- `Definition` 指向原始 `BTNodeData`
- 节点配置不再在运行时节点内重复定义

### 5.2 结构节点

- `BTRoot`
- `BTSequence`
- `BTSelector`
- `BTParallel`
- `BTInverter`
- `BTSucceeder`
- `BTFailer`
- `BTRepeater`
- `BTBlackboardCondition`
- `BTSubTreeCall`

### 5.3 叶子节点

#### typed leaf

- `BTLog`
- `BTSetBlackboard`
- `BTBlackboardExists`
- `BTBlackboardCompare`
- `BTPatrol`
- `BTHasPatrolPath`

#### generic fallback leaf

- `BTActionCall`
- `BTConditionCall`
- `BTServiceCall`
- `BTWait`

## 6. 行为树执行流程

### 6.1 加载 bytes

- 通过 `BTLoader.LoadBytes/LoadPackage`
- Loader 层通过 `BTBytesLoader.GetOneBehaviorTreeBytes` 提供 bytes
- `Loader` 不依赖 `Model`

### 6.2 反序列化

- `BTSerializer.Deserialize(bytes)`
- 结果为 `BTPackage`

### 6.3 创建 Session

- `BTRuntime.Create(owner, bytes, treeIdOrName)`
- 创建 `BTExecutionSession`
- 初始化：
  - `Package`
  - `TreeIdMap / TreeNameMap`
  - `EntryDefinition`
  - `Blackboard`
  - `BTEnv`

### 6.4 Data -> Graph

- `BTGraphBuilder.Build(session)`
- 根据 `BTDefinition.RootNodeId` 和 `BTNodeData` 列表构建运行时对象图
- 每个运行时节点会记录其 `Definition`

### 6.5 运行

- `BTFlowDriver.RunRoot(session)`
- `BTDispatcher.Instance.Handle(node, env)`
- dispatcher 根据 `BTNode` 的真实 CLR 类型找到对应 `BTNodeHandler`

### 6.6 异步恢复

对于 `Wait`、`Service`、`Patrol` 这类异步节点：

- 首次进入返回 `Running`
- 启动异步任务
- 完成后通过 `BTFlowDriver.Resume(...)` 恢复树推进

## 7. 节点分派模型

### 7.1 分派入口

- `BTDispatcher.Instance.Handle(node, env)`

### 7.2 返回值

- `BTExecResult.Success = 0`
- `BTExecResult.Failure = 1`
- `BTExecResult.Running = 2`

### 7.3 结构节点 handler

每种结构节点对应一个独立 handler 文件，便于：

- 逻辑拆分
- 双击跳转
- 后续维护

### 7.4 叶子节点 handler

叶子节点有两套执行路径：

#### typed handler

适合 builtin/demo/稳定节点：

- `BTLogActionHandler`
- `BTSetBlackboardActionHandler`
- `BTBlackboardExistsConditionHandler`
- `BTBlackboardCompareConditionHandler`
- `BTPatrolAction`
- `BTHasPatrolPathCondition`

#### generic bridge handler

用于旧字符串 handler 扩展点：

- `BTActionCallHandler`
- `BTConditionCallHandler`
- `BTServiceCallHandler`
- `BTWaitHandler`

## 8. 叶子节点策略

### 8.1 typed leaf 优先

当前推荐优先使用 typed leaf：

1. 定义 `xxxData`
2. 定义运行时节点类型
3. 定义 `BTNodeDescriptor`
4. 定义对应 `ABTNodeHandler<TNode>`

优点：

- 跳转更准确
- 参数结构更清晰
- 后续性能优化空间更大

### 8.2 generic leaf 保留回退

当前仍保留 `BTActionNodeData / BTConditionNodeData / BTServiceNodeData` 与 generic call 节点，作为兼容与扩展回退通道。

## 9. 黑板设计

### 9.1 黑板能力

- 默认值填充
- 值读写
- observer 通知
- 按 key 监听

### 9.2 当前实现

黑板由 `BTBlackboard` 提供，能力包括：

- `ApplyDefaults`
- `GetBoxed`
- `SetBoxed`
- `Remove`
- `Observe`
- `RemoveObserver`

### 9.3 BlackboardCondition

`BTBlackboardConditionHandler` 支持：

- 无子节点时直接求值
- 有子节点时注册 observer
- `Self` 模式下条件变假时中断当前子树

当前已完整支持：

- `Self`

当前仍保留但未完整实现：

- `LowerPriority`
- `Both`

## 10. Wait / Service / SubTree

### 10.1 Wait

- `BTWaitHandler`
- 使用 `TimerComponent.WaitAsync`
- 返回 `Running`
- 结束后由 `BTFlowDriver.Resume(...)` 恢复

### 10.2 Service

- `BTServiceCallHandler`
- 启动 service loop
- 周期执行旧 `ABTServiceHandler.Tick(...)`
- 子节点完成后 service loop 停止

### 10.3 SubTree

- `BTSubTreeCallHandler`
- 通过 `SubTreeId / SubTreeName` 解析定义
- `BTGraphBuilder` 在建图时递归构建子树根节点

## 11. 编辑器设计

### 11.1 编辑器资产

- 使用 `BTAsset` 保存本地编辑信息
- 节点的编辑态描述保存在 `BTEditorNodeData`

### 11.2 节点创建

- `BTSearchWindowProvider` 提供创建入口
- `BTNodeDescriptor` 负责节点菜单定义、标题、参数描述
- `BTEditorRuntimeNodeFactory` 负责把编辑节点转换为运行时 `BTNodeData` 对象
- 该转换不再通过静态引用 `Unity.Model` 完成，而是通过 `BTEditorRuntimeBridge` 反射创建目标类型

### 11.3 导出

- `BTExporter` 负责从 `BTAsset` 构建运行时 `BTPackage`
- `BTExporter` 内部通过 `BTEditorRuntimeBridge` 反射创建：
  - `BTPackage`
  - `BTDefinition`
  - `BTNodeData`
- `BTEditorRuntimeBridge` 会在序列化前自动执行 Nino 生成注册初始化：
  - `Unity.Model.NinoGen.NinoBuiltInTypesRegistration.Init()`
  - `Unity.Model.NinoGen.Serializer.Init()`
  - `Unity.Model.NinoGen.Deserializer.Init()`
  - `ET.EntitySerializeRegister.Init()`
- 根树与所有子树一起打包导出为 bytes

### 11.3.1 纯反射方式解耦 Editor 导出链

当前 Editor 导出链路如下：

1. `BTAsset` / `BTEditorNodeData` 保存 authoring 数据
2. `BTEditorRuntimeNodeFactory` 将编辑节点转换为运行时 `BTNodeData`
3. `BTExporter` 反射构建 `BTDefinition / BTPackage`
4. `BTEditorRuntimeBridge.SerializePackage(...)` 调用运行时 `BTSerializer`
5. 输出 bytes 到客户端与服务端目标目录

这条链路的关键点是：

- Editor 资产层与运行时层已经解耦
- Editor 只保留共享基础类型的静态依赖
- 对 `Unity.Model` 的访问统一收口到 `BTEditorRuntimeBridge`

### 11.4 双击跳转

节点双击/打开脚本采用两级解析：

1. 优先解析 `BTNodeHandler`
2. 找不到则回退到默认运行时代码文件

当前 handler 文件已经按类型拆分，因此跳转不再落到聚合文件。

### 11.5 Editor 与运行时的依赖边界

当前为了避免 `TypeLoadException`，只将 Editor 资产层必须直接反射使用的基础共享类型放在 `Core`，例如：

- `BTNodeKind`
- `BTNodeState`
- `BTCompareOperator`
- `BTAbortMode`
- `BTParallelPolicy`
- `BTValueType`
- `BTSerializedValue`
- `BTArgumentData`
- `BTBlackboardEntryData`
- `BTDebugHub`
- `BTDebugSnapshot`
- `BTPatrolPointData`

而行为树运行时执行主体仍保留在：

- `Model`
- `Hotfix`

这就是当前纯反射方案落地形式：

- 运行时逻辑尽量不下沉到 `Core`
- 仅把 Editor 直接依赖且会触发反射加载的共享基础类型放回 `Core`

## 12. 性能特征

### 12.1 当前优势

- 行为树初始化与运行解耦
- 真正执行阶段基于 `BTNode` 对象图
- typed leaf 比旧字符串分发更直接
- 非编辑器环境已关闭调试快照发布

### 12.2 已做优化

#### 节点索引缓存

`BTDefinition.GetNode` 已加入节点索引缓存，避免建图阶段每次线性扫描 `Nodes`。

#### 调试发布只在 Editor 生效

`BTExecutionSession.PublishDebug` 只在 `UNITY_EDITOR` 下参与编译，非编辑器运行时不再全量组装快照。

#### 节点不再重复保存配置

运行时节点不再复制 `Data` 中的配置字段，只保留 `Definition` 引用，减少对象体积和构图复制成本。

### 12.3 仍存在的成本点

- `BTRuntime.Create` 仍然会执行 `package.Clone()`
- `BTGraphBuilder` 仍然是“每个 session 现建对象图”
- generic leaf 参数读取仍然依赖 `BTNodeData.Arguments`
- 同一棵树被大量单位同时实例化时，仍会有初始化峰值

## 13. 调试与观测

### 13.1 调试中心

- `BTDebugHub`
- `BTDebugSnapshot`

### 13.2 当前行为

- 编辑器可以读取运行时快照显示节点状态和黑板值
- 非编辑器构建下，调试快照发布默认不参与运行路径

## 14. 典型使用方式

### 14.1 挂载到 Unit

- `unit.AddComponent<BTComponent, byte[], string>(treeBytes, "AITest")`

### 14.2 修改黑板

- `unit.GetComponent<BTComponent>()?.SetBlackboardValue("HasTarget", true)`

### 14.3 重新加载行为树

- `BTComponent.Reload(bytes, treeIdOrName)`

### 14.4 服务端验证

- `Robot LoadTree AITest`
- `Robot RunTree AITest`

## 15. 已知边界

- `LowerPriority / Both` 中断语义未完整实现
- 仍保留 generic leaf fallback
- `BTNodeData` 仍然是运行时建图输入层，不是纯编辑器废数据
- 尚未做“编译后模板缓存 / graph template cache”

## 16. TODO

### 16.1 下一步优化

- 将 `package.Clone()` 进一步按所有权场景优化
- 引入 graph/template cache，减少批量实例化时的初始化分配
- 继续把 generic leaf 逐步 concrete 化

### 16.2 新增节点流程

新增节点建议按以下顺序：

1. 定义 `xxxData`
2. 补充 `BTNodeDescriptor`
3. 定义运行时节点 `BTNode`
4. 定义对应 `BTNodeHandler`
5. 在 `BTEditorRuntimeNodeFactory` 与 `BTGraphBuilder` 中接入

这样能保证：

- 编辑器创建正确
- 导出结构稳定
- 运行时分派准确
- 双击跳转正确

### 16.3 最小完整模板示例

当前项目已提供一个完整的 typed leaf 示例模板：`BTSetBlackboardIfMissing`

推荐把它视为“新增功能节点”的标准模板。

对应文件：

- 数据定义：`Unity/Assets/Scripts/Model/Share/Module/BehaviorTree/BTTypedLeafNodeData.cs`
- 运行时节点：`Unity/Assets/Scripts/Model/Share/Module/BehaviorTree/BTNode.cs`
- 运行时建图：`Unity/Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTGraphBuilder.cs`
- 执行器：`Unity/Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTSetBlackboardIfMissingActionHandler.cs`
- 编辑器描述：`Unity/Assets/Scripts/Editor/BehaviorTree/BTBuiltinNodeDescriptors.cs`
- 编辑器导出工厂：`Unity/Assets/Scripts/Editor/BehaviorTree/BTEditorRuntimeNodeFactory.cs`
- 编辑器脚本跳转映射：`Unity/Assets/Scripts/Editor/BehaviorTree/BTEditorUtility.cs`

这个节点演示了“参数型 Action 节点”的完整接入方式，可作为后续新增功能节点的标准参考。

### 16.4 最小模板节点的创建流程

以 `BTSetBlackboardIfMissing` 为例，新增一个正式功能节点的最小流程如下：

1. 在共享常量里增加 `TypeId`
2. 在 `Model` 新增 `XxxData : BTNodeData`
3. 在 `Model` 新增运行时节点 `BTXxx : BTAction/BTCondition/BTService`
4. 在 `Hotfix` 新增 `BTXxxHandler : ABTNodeHandler<BTXxx>`
5. 在 `BTGraphBuilder` 中增加 `XxxData -> BTXxx` 映射
6. 在 `BTNodeDescriptor` 中增加菜单、标题、参数定义
7. 在 `BTEditorRuntimeNodeFactory` 中增加 `TypeId -> XxxData` 的导出映射
8. 在 `BTEditorUtility` 中增加 `TypeId -> BTXxx` 的脚本跳转映射
9. 执行 `dotnet build ET.sln` 并验证编辑器创建、双击跳转、导出 bytes 与运行时分派

如果一个新节点尚处于试验阶段，也可以先走 generic fallback：

- `BTActionNodeData / BTConditionNodeData / BTServiceNodeData`
- `BTActionCallHandler / BTConditionCallHandler / BTServiceCallHandler`

但正式功能节点仍推荐优先采用 typed leaf。
