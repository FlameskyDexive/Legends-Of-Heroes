# cn.etetet.box2dsharp

Box2DSharp 2D 物理库(纯 C#,基于 `System.Numerics.Vector2`),从 Legends-Of-HeroesOri 的 `ThirdParty/Box2DSharp` 迁移而来,共 110 个源文件。

## 用途
球球大作战是 2D 玩法,用 Box2D 做 2D 碰撞/重叠检测(大球吞小球、子弹命中)。**不使用** `cn.etetet.collision`(那是 3D 库,基于 `Unity.Mathematics.float3`,与本库无关)。

## 打包
- 纯库,无 UnityEngine 依赖(0 处 `using UnityEngine`),仅依赖 .NET BCL(`System`/`System.Numerics`/`System.Buffers` 等)。
- 源码在 `Scripts/Core/Share/Box2DSharp/`,通过 `AssemblyReference.asmref → ET.Core` 合并进 ET.Core 程序集(客户端+服务端通用),与 `cn.etetet.recast` 同范式。
- `allowUnsafeCode: true`(库内用到 `System.Buffers`/`CompilerServices`)。
- Level 2(低层,供玩法包依赖);依赖 `cn.etetet.core`。

## 关键类型(命名空间 `Box2DSharp.*`)
- `Box2DSharp.Dynamics.World`:物理世界根(建 Body、`Step`、查询)。
- `Box2DSharp.Dynamics.Body` / `Fixture`;形状 `Box2DSharp.Collision.Shapes.{CircleShape,PolygonShape}`。
- `Box2DSharp.Dynamics.IContactListener`:接触回调(用 `BeginContact` 做重叠检测)。
- `Fixture.UserData`:挂 ET `Unit`,接触时由此反查实体。

## 坐标桥接
Box2D 用 `System.Numerics.Vector2`(2D)。本项目 `Unit.Position` 是 `Unity.Mathematics.float3`,玩法层按 **X/Z 平面**映射:`new Vector2(pos.x, pos.z)`。该桥接逻辑放在玩法包(`cn.etetet.ballbattle`),不在本库内。
