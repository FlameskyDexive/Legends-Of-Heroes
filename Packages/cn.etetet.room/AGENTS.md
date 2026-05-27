# cn.etetet.room

## 概述

Room、RoomList、CreateRoom EUI 模块，移植自 Legends-Of-Heroes 状态同步房间系统。

**注意**：源工程的 `RoomInfo` / `PlayerInfo` / `RoomMode` / `G2C_GetRoomList` / `G2C_JoinRoom` / `G2C_CreateRoom` / `EnterMapHelper.GetRoomListAsync / JoinRoomAsync / CreateRoomAsync` 等 proto 消息与业务 helper 当前工程尚未移植。本包提供了最小桩类型（位于 `Scripts/Model/Share/Stubs/`），用于 UI 编译通过。真正使用时需要把 proto 与 helper 移植完成后，再把桩类型替换为正式实现。

## 包结构

- `Scripts/Model/Share/Stubs/` — RoomInfo / PlayerInfo / RoomMode / G2C_* 桩类型与 EnterMapHelper 桩扩展
- `Scripts/ModelView/Client/EUI/DlgRoom/`
- `Scripts/ModelView/Client/EUI/DlgRoomList/`
- `Scripts/ModelView/Client/EUI/DlgCreateRoom/`
- `Scripts/HotfixView/Client/EUI/DlgRoom/`
- `Scripts/HotfixView/Client/EUI/DlgRoomList/`
- `Scripts/HotfixView/Client/EUI/DlgCreateRoom/`

## 关键依赖

- `cn.etetet.eui`：UIBaseWindow / UIComponent / WindowID / LoopScrollRect。
- `cn.etetet.lobby`：复用 `Scroll_Item_role`。
- `cn.etetet.login`：现有 EnterMapHelper 部分类，本包在同一类中追加 partial 方法。

## TODO

- 待 proto 移植完成后，删除 `Scripts/Model/Share/Stubs/` 全部文件，由 `cn.etetet.proto` 提供真实定义。
- `EnterMapHelper.GetRoomListAsync / JoinRoomAsync / CreateRoomAsync` 桩为空实现，需要补真实网络调用逻辑。
