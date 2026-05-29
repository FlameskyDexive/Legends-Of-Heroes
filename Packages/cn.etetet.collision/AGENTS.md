# cn.etetet.collision

## 概述

高性能 3D 碰撞检测包。提供无分配（allocation-free）的值类型几何体与静态检测接口，并附带客户端编辑器下的 Gizmos 可视化调试工具。

- **几何体**（`Scripts/Model/Share`，`ET.Model`，客户端/服务端共享，纯 `Unity.Mathematics` 计算）：
  - `AABB`：轴对齐包围盒（中心 + 半长，只读结构体）。
  - `OBB`：有向包围盒 / 带方向的立方体（中心 + 半长 + 旋转）。
  - `Sphere`：球体（中心 + 半径）。
  - `Collider3D`：统一碰撞体（标记联合），用于异构集合上的运行时多态检测。
  - `CollisionShapeType`：形状枚举。
- **对外接口** `Collision3DHelper`（静态类，`in` 入参 + 激进内联）：
  - `AABBIntersect`、`SphereIntersect`。
  - `OBBIntersect`：带方向立方体之间，分离轴定理（SAT，15 条候选轴）。
  - `OBBSphereIntersect`：带方向立方体与球体，最近点法。
  - `AABBSphereIntersect`、`AABBOBBIntersect`：派生组合。
  - `ContainsPoint`：AABB / OBB / Sphere 点包含。
  - `Intersect(in Collider3D, in Collider3D)`：统一多态派发。
  - `ComputeAABB(in Collider3D)`：任意碰撞体的世界 AABB（广相与快速预筛用）。
- **批量广相** `CollisionWorld3D`（`Scripts/Model/Share`，`[EnableClass]`，每帧零堆分配）：
  - 处理数千~上万碰撞体的同帧两两检测。`Clear` → 多次 `Add` → `DetectPairs(List<CollisionPair>)`。
  - 稠密均匀网格（dense uniform grid）广相：按世界 AABB 中心落单格，只比较 3x3x3 邻域，三轴剪枝，近 O(N)；网格边长自动取最大 AABB 边长并对总格数封顶。
  - 广相 AABB 重叠用标量 float 比较（避开非 Burst 下 `Unity.Mathematics` 向量开销）；窄相仅对候选对调用 `Collision3DHelper.Intersect`。
  - `CollisionPair`（只读 struct，A&lt;B）。
- **可视化调试**（`Scripts/ModelView/Client`，`ET.ModelView`，客户端专属）：
  - `CollisionGizmos`：在 `OnDrawGizmos` 内绘制 AABB/OBB/Sphere 线框。
  - `CollisionDebugComponent`：场景 MonoBehaviour，Inspector 配置多个碰撞体，实时两两检测，相交者高亮。

## 分层与程序集

- 核心几何与算法放 `Model/Share`（参照 `cn.etetet.conditionexpr` 的 `ConditionExprCompiler` 同样把纯算法放 Model/Share 的先例），使服务端、客户端、视图层、测试均可引用。
- 检测算法**禁止依赖** UnityEngine；只用 `Unity.Mathematics`，以保证服务端 `dotnet build ET.sln` 可编译。
- 可视化代码依赖 UnityEngine/UnityEditor，必须放 `ModelView/Client`（`ET.ModelView`，客户端专属），不能放 `Model`/`Hotfix` 共享程序集。
- 两个可视化文件整体用 `#if UNITY_EDITOR` 包裹，仅编辑器模式编译。
- `CollisionGizmos` 是静态类，ET0032 分析器对静态类豁免，无需特殊处理。
- `CollisionDebugComponent` 是普通 `MonoBehaviour`（非 ET Object），ModelView 程序集的 ET0032 分析器会拦截，需加 `[EnableClass]`（参照 `cn.etetet.statesync` 的 `MoveToGameObject`）。

## 依赖

- 仅依赖 `cn.etetet.core`；`Unity.Mathematics` 经 core 传递可达。
- 不依赖 `unit` 等更高层包，保持为可复用的低层几何库。

## 测试

- 测试位于 `Scripts/Hotfix/Test`（`ET.Hotfix`），命名 `Collision_{用例}_Test`，继承 `ATestHandler`。
- 纯数学验证使用 `SceneType.TestEmpty`；失败用 `Log.Console` 并返回唯一错误码，成功返回 `ErrorCode.ERR_Success`。
- 覆盖：`Collision_AABB_Test`、`Collision_OBB_Test`、`Collision_Sphere_Test`、`Collision_Unified_Test`。
- `Collision_Benchmark_Test`：1k/5k/10k/20k 碰撞体批量检测基准；先用暴力 O(N^2) 校验广相结果完全一致，再计时；报告存档到 `./Logs/CollisionBenchmark_<时间戳>.log`。当前实测（dev 服务端 managed 单线程）近 O(N)、约 320~340 colliders/ms（10k detect ≈ 21ms）。

## 注意

- 不要手工创建 `.meta` 或修改 `.csproj`；新增文件后通过 Unity 刷新 / UnityBridge `RegenProject` 生成工程引用，再 `dotnet build ET.sln`。
- 结构体均为 `readonly struct`，构造时对半长/半径取绝对值，保证非负。
