# cn.etetet.btreedemo

## 概述

新版本行为树（`cn.etetet.btree`）的示例（Demo）包，从 Legends-Of-HeroesOri 迁移而来。只保留 Demo 启动逻辑（事件 + 工厂），具体的逻辑节点已拆到 `cn.etetet.btreenode`。

- 类型前缀沿用 `BTree*`（与 `cn.etetet.btree` 一致）
- 命名空间：客户端事件/工厂用 `ET.Client`

## 依赖

`core`、`btree`、`btreenode`、`unit`、`map`。逻辑节点（巡逻等）由 `cn.etetet.btreenode` 提供。

## 内容

- `Scripts/Model/Share/SceneType.cs`：新增 `SceneType.Demo`（= `PackageType.BTreeDemo*1000+1`）
- `Scripts/Model/Share/PackageType.cs`：`PackageType.BTreeDemo`
- `Scripts/Hotfix/Client/AfterMyUnitCreate_BTreeDemo.cs`：玩家单位创建后挂载 AITest 行为树的示例事件（`AfterMyUnitCreate` 是客户端事件，故在 `Client`）
- `Scripts/Hotfix/Client/BTreeClientDemoFactory.cs`：用运行时类型构造 AITest 行为树字节的工厂

## 框架适配（相对 Ori）

- `args.unit` → `args.Unit`（本项目 `AfterMyUnitCreate.Unit` 为 `EntityRef<Unit>`，隐式转 `Unit`）
- `await` 后通过 `EntityRef<Unit>` 重新获取 `unit`（ET 异步规范 ETAE001）
- `SceneType.Demo` 在本包内新增；Ori 用其作为 Demo 场景守卫

## 启用 Demo

`AfterMyUnitCreate_BTreeDemo` 默认不会在正式玩法触发（守卫 `root.SceneType != SceneType.Demo`，而正式场景不使用该类型）。要体验 Demo，把客户端根场景的 `SceneType` 设为 `SceneType.Demo`，或临时移除该守卫。AITest 字节文件由 `cn.etetet.btree` 编辑器菜单 `ET/AI/Export Demo AITest.bytes` 生成，或调用 `BTreeClientDemoFactory.CreateAITestBytes()`。

## 逻辑节点在哪

巡逻等业务逻辑节点 Handler 在 `cn.etetet.btreenode`（按 ActionNode/ConditionNode 划分）。节点数据/运行时节点类型/编辑器描述符在 `cn.etetet.btree`。本包只负责把行为树挂到单位上跑起来。
