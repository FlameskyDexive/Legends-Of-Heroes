# cn.etetet.btree

## 概述

行为树（BTree）包，本项目同时存在另一种行为树实现 `cn.etetet.behaviortree`（基于 Odin + ScriptableObject），为了避免命名冲突：

- **包名**：`cn.etetet.btree`（区别于 `cn.etetet.behaviortree`）
- **类型前缀**：所有原 `BT*` 类型在迁移时统一改名为 `BTree*`（如 `BTDispatcher → BTreeDispatcher`、`IBTHandler → IBTreeHandler`、`BTComponent → BTreeComponent`）
- **命名空间**：保持 `ET`（与原项目一致）

## 目录约定

- `Scripts/Model/Share/Module/BehaviorTree` ：行为树运行时数据与序列化结构（BTreeNodeData、BTreeDefinition、BTreeSerializer 等）
- `Scripts/Hotfix/Share/Module/BehaviorTree`：行为树执行器（Handler、ComponentSystem、FlowDriver）
- `Scripts/Editor/Share/BehaviorTreeEditor`：基于 UIElements/GraphView 的可视化编辑器

## 序列化

使用 Nino（`com.jasonxudeveloper.nino`，已在 `Packages/manifest.json` 添加 openupm 来源）。如需更换为 MemoryPack，请改写 `BTreeSerializer` 并补全节点数据类的 `[MemoryPackable]`/`[MemoryPackUnion]` 标记。

## 编辑器约定

- 编辑器代码通过 `Scripts/Editor/Share/AssemblyReference.asmref` 汇入 `ET.Editor`
- 编辑器内对运行时类型的反射调用使用全限定名 `ET.BTree*`（不要写裸字符串 `ET.BT*`）

## 不包含 Demo

迁移时未包含 `BTPatrolAction` / `BTHasPatrolPathCondition` / `AfterMyUnitCreate_BTDemo` / `BTClientDemoFactory`（依赖 unit/move/mapplay，且 SceneType.Demo 与本项目不一致）。如需 Demo，需要单独引入并适配 `EntityRef<Unit>`/`SceneType` 等差异。

巡逻数据类型保留在编辑器层用于 Inspector：`BTreePatrolPointData`、`BTreePatrolNodeData`、`BTreeHasPatrolPathNodeData`、`BTreePatrolNodeTypes`。
