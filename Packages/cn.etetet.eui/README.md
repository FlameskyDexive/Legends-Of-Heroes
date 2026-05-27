# cn.etetet.eui

EUI（Easy UI）基础 UI 框架。

移植自 Legends-Of-Heroes 原版项目（`Assets/Scripts/{Loader|ModelView|HotfixView|Editor}/EUI`），并按 ET10 模块化分包结构重组：

- `Scripts/ModelView/Client/EUI/` — EUI 数据组件、Window 定义、事件接口、属性。
- `Scripts/HotfixView/Client/EUI/` — EUI 系统、辅助方法、Root 管理、图标加载。
- `Scripts/Model/Share/` — `PackageType`、协程锁类型分类。

## 与 YIUI 的关系

本包独立运行，不依赖 `cn.etetet.yiui*`。同一项目内 EUI 与 YIUI 可并存，分别承载不同界面流程。当前 Login 启动已切换为 EUI，其它界面（Lobby / Main / Loading 等）仍走 YIUI。

## 使用方式

```csharp
World.Instance.AddSingleton<UIEventComponent>();
root.AddComponent<EUIRootComponent>();
root.AddComponent<UIPathComponent>();
var uiComponent = root.AddComponent<UIComponent>();
await uiComponent.ShowWindowAsync(WindowID.WindowID_Login);
```

界面预制体规范：

- 预制体名 `Dlg{WindowID 去掉 WindowID_}`，例如 `WindowID_Login -> DlgLogin`。
- 预制体位置：`Assets/Bundles/UI/Dlg/DlgXxx.prefab`（由 YooAssets 加载，加载键为预制体名）。
