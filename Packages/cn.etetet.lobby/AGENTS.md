# cn.etetet.lobby

## 概述

Lobby（大厅）与 MatchTeam（组队匹配）EUI 模块，移植自 Legends-Of-Heroes 状态同步玩法。依赖 cn.etetet.eui 基础框架，业务调用走 cn.etetet.login 的 EnterMapHelper / PlayerComponent。

## 包结构

- `Scripts/ModelView/Client/EUI/DlgLobby/` — DlgLobby Entity 与 ViewComponent
- `Scripts/ModelView/Client/EUI/DlgMatchTeam/` — DlgMatchTeam Entity 与 ViewComponent
- `Scripts/ModelView/Client/EUI/Item_role/` — 滚动列表项 Scroll_Item_role
- `Scripts/HotfixView/Client/EUI/DlgLobby/` — DlgLobby 行为与事件订阅
- `Scripts/HotfixView/Client/EUI/DlgMatchTeam/` — DlgMatchTeam 行为
- `Scripts/Model/Share/PackageType.cs` — 包 Id 常量

## 关键依赖

- `cn.etetet.eui`：UIBaseWindow / UIComponent / WindowID / EUIHelper / LoopScrollRect。
- `cn.etetet.login`：EnterMapHelper.EnterMapAsync、PlayerComponent.MyId。

## 维护规则

- 不要在本包内新增业务逻辑实体，UI 之外的状态留给上层包。
- `WindowID` 列表统一维护在 `cn.etetet.eui` 的 WindowID.cs，本包仅消费。
- 修改 prefab 后无需调整 `WindowPrefabPath`（由 UIPathComponentSystem 自动派生）。
