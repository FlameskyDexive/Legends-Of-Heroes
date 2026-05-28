# cn.etetet.bundles

## 概述

美术 / 运行时资源 Bundle 包(YooAsset 可收集),从 Legends-Of-HeroesOri 迁移而来。镜像原工程 `Assets/Bundles` + `Assets/Res` 的目录划分,作为本工程集中的资源落点之一。

> 本工程采用"每包自带 Bundles"约定:YooAsset 收集器(`Packages/cn.etetet.statesync/Settings/AssetBundleCollectorSetting.asset`)从各包的 `Bundles/` 或 `Assets/GameRes/` 收集。本包把可共享的美术与行为树数据集中存放。

## 目录划分(参照 Ori)

- `Bundles/AI/Bytes/`   行为树运行时字节(AITest.bytes、NewBehaviorTree.bytes)——已注册为收集器组 `BTreeAI`
- `Res/UI/`             Dlg 预制体引用的 UI 图片(LOL/Ball 等,从 Ori 迁入,保留原 GUID)
- `Res/Font/`           字体(MinCartoon.ttf)
- `Res/Audio/`          音效(LOL 角色/Balls,从 Ori 迁入)——已注册为收集器组 `Audio`

## 迁移记录(从 Ori)

- **Dlg 预制体**:11 个(DlgLogin/DlgLobby/DlgHotUpdate/DlgMatchTeam/DlgBattle/DlgHelper/DlgRedDot/DlgLSLogin/DlgLSLobby/DlgLSRoom/DlgRoom)用 Ori 内容覆盖到各自所在的功能包(statesync/lobby/hotupdate/lockstep/room 的 `Assets/GameRes/EUI/Dlg`),**保留各自当前 `.meta`**(EUI 按窗口名加载,GUID 不参与注册)。
- **脚本 GUID 重映射**:预制体里 Ori 的 `EUIButton`/`EUIImage`/`LoopHorizontalScrollRect`/`Joystick` 组件 GUID 已字节级替换为本工程 `cn.etetet.eui` 对应脚本的 GUID;`ReferenceCollector`(本工程缺失,仅 DlgLSRoom 用)整脚本迁入 `cn.etetet.eui/Scripts/Loader/Client/EUI/UIExtension/`(保留原 GUID)。
- **图片/字体/音频**:连 `.meta` 迁入(保留 Ori GUID),预制体引用按 GUID 解析。
- 未解析的 GUID 均为 Unity 内置 UGUI/TMP 组件,运行时由 Unity 包提供。

## YooAsset 收集

- 组 `BTreeAI` → `Bundles/AI/Bytes`(AddressByFileName / CollectAll / PackDirectory);运行时 `ResourcesComponent.Instance.LoadAssetAsync<TextAsset>("Packages/cn.etetet.bundles/Bundles/AI/Bytes/{TreeName}.bytes")`(见 `cn.etetet.btreedemo` 的 `GetOneBehaviorTreeBytesHandler`)。
- 组 `Audio` → `Res/Audio`(AddressByFileName / CollectAll / PackSeparately);可按文件名加载。
- `Res/UI` **不单独建组**:这些图片被各 Dlg 预制体引用,预制体已在 `EUI` 组内收集,YooAsset 会自动把图片作为依赖打包;另建组会导致重复打包。
- 新增其它资源目录时,建议用 Unity 的 YooAsset 收集器窗口操作,避免手改 GUID 出错。

## 与 btree 的关系

`cn.etetet.btree` 的 `BTreeBytesLoader` 路径常量已指向本包 `Bundles/AI/Bytes`;编辑器导出(`BTreeExporter`/`BTreeAsset.ExportRelativePath`)也写到这里。

## 注意

- 纯资源包(Level 1),仅依赖 core;不要在此包写业务逻辑代码。
- `.meta`/GUID 由 Unity 生成;迁入新资源后需在 Unity 中 reimport。
