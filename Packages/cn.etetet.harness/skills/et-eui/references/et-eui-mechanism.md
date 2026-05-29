# ET EUI 机制与手写模板

适用对象：本项目 `cn.etetet.eui` 框架及其业务窗口。所有结论均来自项目内真实代码，不是通用 EUI 文档。

> 重要差异：本项目**没有**代码生成工具（无 `ET/EUICodeSpawn` 菜单、无 `UICodeSpawner` / `CreateWidgetBindCode`）。窗口代码全部按下面的模板**手写**。不要去找生成菜单。

---

## 一、包与落点

- 框架包：`cn.etetet.eui`，命名空间 `ET.Client`，仅依赖 `cn.etetet.core` / `cn.etetet.loader` / `cn.etetet.yooassets`。
  - 框架核心：`UIComponent` / `UIBaseWindow` / `UIEventComponent` / `UIPathComponent` / `EUIRootComponent` 及其 System。
  - 公共复用 UI（`ESXxx`）、对象池、LoopScrollRect、RedDot 等也在框架包。
- 业务窗口 `DlgXxx`：放进**所属业务包**，例如：
  - `cn.etetet.statesync`：`DlgLogin` / `DlgRedDot` / `DlgHelper`
  - `cn.etetet.lockstep`：`DlgLSLogin` / `DlgLSLobby` / `DlgLSRoom`
- 业务包内固定目录：
  - `Scripts/ModelView/Client/EUI/DlgXxx/`：`DlgXxx.cs`、`DlgXxxViewComponent.cs`
  - `Scripts/HotfixView/Client/EUI/DlgXxx/`：`DlgXxxSystem.cs`、`DlgXxxViewComponentSystem.cs`、`DlgXxxEventHandler.cs`
- 约束：框架包不依赖业务包；业务窗口不要塞进 `cn.etetet.eui`。

---

## 二、命名约定（决定能否被找到/加载）

- `Dlg*`：窗口主体。预制体必须命名为 `Dlg{Name}`。
- `ES*`：可复用子 UI（如 `ESReuseUI`）。
- `Item*`：循环列表项；预制体需挂 `LayoutElement`。
- `E*`：可绑定控件节点（`UIFindHelper.FindDeepChild` 按这个名字查找）。
- `EG*`：可绑定的普通容器节点（`RectTransform`）。

`WindowID` 与预制体名是**自动映射**的，见下。

---

## 三、WindowID 与预制体路径（自动映射，无需手填）

`WindowID` 是枚举：`cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowID.cs`

```csharp
public enum WindowID
{
    WindowID_Invaild = 0,
    WindowID_Login,
    WindowID_Lobby,
    // ... 新增窗口在此加一项 WindowID_Xxx
}
```

`UIPathComponentSystem.Awake` 遍历枚举自动建立映射（不需要手写路径表）：

```csharp
string dlgName = "Dlg" + windowID.ToString().Split('_')[1];   // WindowID_Login -> "DlgLogin"
self.WindowPrefabPath[(int)windowID] = dlgName;                // 加载时按名字 LoadAsset
self.WindowTypeIdDict[dlgName] = (int)windowID;                // typeof(DlgLogin).Name -> WindowID
```

含义：
- 新增窗口 = **加一个 `WindowID_Xxx` 枚举项 + 一个名为 `DlgXxx` 的预制体 + 一套 `DlgXxx` 代码**，三者命名必须一致。
- 因为 `WindowTypeIdDict` 的 key 是 `typeof(T).Name`，所以 `self.ShowWindow<DlgXxx>()` 能直接定位窗口。
- 预制体通过 `ResourcesLoaderComponent.LoadAssetSync/Async<GameObject>(dlgName)` 按名字加载（资源名只写名字，不写完整路径与后缀）。

---

## 四、运行时显示链路（ShowWindow）

入口（`UIComponentSystem`）：`ShowWindow<T>()` / `ShowWindow(WindowID)` / `ShowWindowAsync<T>()` / `ShowWindowAsync(WindowID)`。

首次加载窗口时 `LoadBaseWindows(Async)` 的顺序（务必牢记）：

1. 按 `WindowPrefabPath` 取得 `DlgXxx` 名字，`LoadAsset` 并 `Instantiate`。
2. `OnInitWindowCoreData(baseWindow)`：设置 `uiBaseWindow.windowType`（决定挂到哪个 Root）。
3. `SetRoot(EUIRootHelper.GetTargetRoot(root, windowType))` + `SetAsLastSibling()`。
4. `OnInitComponent(baseWindow)`：`AddComponent<DlgXxx>().AddComponent<DlgXxxViewComponent>()`。
5. `OnRegisterUIEvent(baseWindow)`：`GetComponent<DlgXxx>().RegisterUIEvent()`。

显示时 `RealShowWindow`：`SetActive(true)` → `OnShowWindow(baseWindow, contextData)`。
隐藏 `HideWindow`：`SetActive(false)` → `OnShowWindow` 的反向 `OnHideWindow`。
卸载 `UnLoadWindow`：`BeforeUnload` → 销毁 GameObject。`CloseWindow` = Hide + UnLoad。

所有回调通过 `UIEventComponent.GetUIEventHandler(WindowID)` 找到对应的 `DlgXxxEventHandler`（靠 `[AUIEvent(WindowID.WindowID_Xxx)]` 注册）。

### UI 根层级（`UIWindowType` → Root）

`EUIRootHelper.GetTargetRoot` 按 `windowType` 返回 `EUIRootComponent` 上的四个 Root：

- `UIWindowType.Normal` → `NormalRoot`
- `UIWindowType.Fixed` → `FixedRoot`
- `UIWindowType.PopUp` → `PopUpRoot`
- `UIWindowType.Other` → `OtherRoot`

在 `OnInitWindowCoreData` 里设 `uiBaseWindow.windowType = UIWindowType.Xxx;`。

---

## 五、5 文件手写模板（以 `DlgLogin` 为真实样例）

### 1) `WindowID.cs`（框架包）新增枚举项

```csharp
WindowID_Login,
```

### 2) `ModelView/.../DlgLogin/DlgLogin.cs`（逻辑实体）

```csharp
namespace ET.Client
{
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgLogin : Entity, IAwake, IUILogic
    {
        public DlgLoginViewComponent View => this.GetComponent<DlgLoginViewComponent>();
    }
}
```

### 3) `ModelView/.../DlgLogin/DlgLoginViewComponent.cs`（控件容器，懒加载 `E*`）

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgLogin))]
    [EnableMethod]
    public class DlgLoginViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Button ELoginButton
        {
            get
            {
                if (this.uiTransform == null) { Log.Error("uiTransform is null."); return null; }
                if (this.m_ELoginButton == null)
                {
                    // 第二个参数是预制体里的节点名（E* 命名）
                    this.m_ELoginButton = UIFindHelper.FindDeepChild<Button>(this.uiTransform.gameObject, "ELogin");
                }
                return this.m_ELoginButton;
            }
        }

        public void DestroyWidget()
        {
            this.m_ELoginButton = null;
            this.uiTransform = null;
        }

        private Button m_ELoginButton;
    }
}
```

### 4) `HotfixView/.../DlgLogin/DlgLoginViewComponentSystem.cs`（绑定 `uiTransform` + 回收）

```csharp
namespace ET.Client
{
    [EntitySystemOf(typeof(DlgLoginViewComponent))]
    [FriendOf(typeof(DlgLoginViewComponent))]
    public static partial class DlgLoginViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgLoginViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgLoginViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
```

### 5) `HotfixView/.../DlgLogin/DlgLoginEventHandler.cs`（生命周期接线）

```csharp
namespace ET.Client
{
    [FriendOf(typeof(UIBaseWindow))]
    [AUIEvent(WindowID.WindowID_Login)]
    public class DlgLoginEventHandler : IAUIEventHandler
    {
        public void OnInitWindowCoreData(UIBaseWindow uiBaseWindow)
            => uiBaseWindow.windowType = UIWindowType.Normal;

        public void OnInitComponent(UIBaseWindow uiBaseWindow)
            => uiBaseWindow.AddComponent<DlgLogin>().AddComponent<DlgLoginViewComponent>();

        public void OnRegisterUIEvent(UIBaseWindow uiBaseWindow)
            => uiBaseWindow.GetComponent<DlgLogin>().RegisterUIEvent();

        public void OnShowWindow(UIBaseWindow uiBaseWindow, Entity contextData = null)
            => uiBaseWindow.GetComponent<DlgLogin>().ShowWindow(contextData);

        public void OnHideWindow(UIBaseWindow uiBaseWindow) { }
        public void BeforeUnload(UIBaseWindow uiBaseWindow) { }
    }
}
```

### 6) `HotfixView/.../DlgLogin/DlgLoginSystem.cs`（业务逻辑 + 事件注册）

```csharp
namespace ET.Client
{
    [FriendOf(typeof(DlgLogin))]
    public static class DlgLoginSystem
    {
        public static void RegisterUIEvent(this DlgLogin self)
        {
            EntityRef<DlgLogin> selfRef = self;   // 闭包里只能持有 EntityRef
            self.View.ELoginButton.AddListener(self.Root(), () => OnLoginButtonClicked(selfRef));
        }

        public static void ShowWindow(this DlgLogin self, Entity contextData = null) { }

        public static async ETTask OnLogin(this DlgLogin self)
        {
            // await 后访问 self 等 Entity 必须用 EntityRef 重新获取，见 et-async
            await LoginHelper.Login(self.Root(), /* ... */);
        }

        private static void OnLoginButtonClicked(EntityRef<DlgLogin> selfRef)
        {
            DlgLogin dlg = selfRef;
            if (dlg == null) return;
            dlg.OnLogin().Coroutine();
        }
    }
}
```

> `DlgLogin` 显示：调用方执行 `uiComponent.ShowWindow<DlgLogin>()` 或 `await uiComponent.ShowWindowAsync<DlgLogin>()`（`uiComponent = Root.GetComponent<UIComponent>()`）。

---

## 六、循环列表（LoopScrollRect）与预加载

- 本项目**直接复用** `cn.etetet.yiuiloopscrollrectasync` 中的 `UnityEngine.UI.LoopScrollRect`，不要在 EUI 内重复移植（会类型冲突）。
- 预热：在 `WindowItemRes.WindowItemResDictionary[WindowID.WindowID_Xxx] = new List<string>{ "ItemXxx" }` 声明窗口打开前要预加载的 Item（`WindowItemRes.cs` 在框架包）。
- Item 预制体必须挂 `LayoutElement`。
- 对象池键来自 `LoopScrollPrefabSource.prefabName`；`UIComponentSystem.Awake` 已 `LoopScrollPoolBridge.Bind(Root)` 接好取物委托。

---

## 七、常见坑速查

1. **窗口打不开**：`WindowID` 是否有 `WindowID_Xxx`；预制体名是否正好是 `DlgXxx`；`[AUIEvent(WindowID.WindowID_Xxx)]` 的 Handler 是否存在。
2. **控件取到 null / `uiTransform is null`**：节点是否按 `E*` 命名；`FindDeepChild` 第二参名字是否与预制体一致；`DlgXxxViewComponentSystem.Awake` 是否给 `uiTransform` 赋值；层级变动导致深查失效。
3. **循环列表不刷新**：Item 是否挂 `LayoutElement`；`prefabSource.prefabName` 是否正确；`WindowItemRes` 是否登记；刷新回调是否注册。
4. **Root 不对**：`OnInitWindowCoreData` 里 `windowType` 是否设对；场景 Root 上是否有 `EUIRootComponent`（四个 Root）。
5. **await 后崩**：窗口逻辑里 `await` 后直接用旧 `self` / `UIBaseWindow`，必须 `EntityRef` 重新获取（叠加 `et-async`）。
6. **不要把控件字段直接存 Entity**：闭包/字段里只持有 `EntityRef`（参考 `RegisterUIEvent` 里的 `selfRef`）。

---

## 八、与其它 skill 联动

- 架构/包依赖：`et-code`
- 异步与 `EntityRef`：`et-async`
- 编译：`et-build`（`dotnet build ET.sln`）
- 编辑器刷新 / PlayMode / 控制台验收：`et-unitybridge`
