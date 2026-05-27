# cn.etetet.hotupdate

## 概述

HotUpdate（资源热更）EUI 模块。包含 DlgHotUpdate 显示资源下载进度。

**注意**：源工程的 `OnPatchDownloadProgress` / `OnPatchDownlodFailed` 事件类与 YooAsset 补丁下载链路当前工程尚未移植。本包提供事件类的最小桩，UI 编译通过即可。真正的下载进度推送需要在 YooAsset 补丁流程中改造为发布这两个事件。

## 包结构

- `Scripts/Model/Share/Stubs/PatchEvents.cs` — `OnPatchDownloadProgress` / `OnPatchDownlodFailed` 桩事件类
- `Scripts/ModelView/Client/EUI/DlgHotUpdate/`
- `Scripts/HotfixView/Client/EUI/DlgHotUpdate/`

## 关键依赖

- `cn.etetet.eui`：UIBaseWindow / UIComponent / WindowID。

## TODO

- 把 `OnPatchDownloadProgress` / `OnPatchDownlodFailed` 替换为 `cn.etetet.yooassets` 内的真实事件类型，并删除 `Scripts/Model/Share/Stubs/`。
- 在 YooAsset 补丁下载流程中调用 `EventSystem.Publish(new OnPatchDownloadProgress(...))` 推送进度。
