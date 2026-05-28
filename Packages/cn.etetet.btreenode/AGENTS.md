# cn.etetet.btreenode

## 概述

新版本行为树（`cn.etetet.btree`）的**上层逻辑节点**库，参照 `cn.etetet.btnode`（旧版 behaviortree 的节点库）组织。本包只放业务逻辑节点的运行时 Handler 及其支撑组件，不含行为树框架本身。

- 类型前缀沿用 `BTree*`，命名空间 `ET`
- 依赖 `core`、`btree`、`unit`、`move`、`numeric`

## 目录约定（按节点类别划分）

- `Scripts/Hotfix/Share/ActionNode/`：动作节点 Handler（`ABTreeNodeHandler<TNode>`，TNode 为 `BTreeAction` 子类）
- `Scripts/Hotfix/Share/ConditionNode/`：条件节点 Handler（`ABTreeNodeHandler<TNode>`，TNode 为 `BTreeCondition` 子类）
- `Scripts/Hotfix/Share/`：节点共用的组件 System（如 `PatrolComponentSystem`）
- `Scripts/Model/Share/`：节点依赖的实体组件（如 `PatrolComponent`）

新增逻辑节点时，按 Action / Condition / Service 归入对应文件夹；Handler 用 `[BTreeNodeHandler]` 标记，由 `BTreeDispatcher` 反射注册，无需改动 `btree` 核心。

## 现有节点

- `ActionNode/BTreePatrolAction.cs`：巡逻动作（`ABTreeNodeHandler<BTreePatrol>`），用 Share 层 `MoveComponent.MoveToAsync` 驱动移动
- `ConditionNode/BTreeHasPatrolPathCondition.cs`：是否拥有巡逻路径（`ABTreeNodeHandler<BTreeHasPatrolPath>`），检查 `PatrolComponent`
- `PatrolComponent` / `PatrolComponentSystem`：巡逻路径数据与取点逻辑

## 与 btree 核心的边界

运行时节点类型（`BTreePatrol`/`BTreeHasPatrolPath`）、节点数据（`BTreePatrolNodeData`/`BTreeHasPatrolPathNodeData`）、`BTreeGraphBuilder` 的类型映射以及编辑器节点描述符仍在 `cn.etetet.btree`（编辑器序列化与图构建强耦合）。本包只承载这些节点的**运行时执行逻辑**，通过 `btree` 依赖引用上述类型。
