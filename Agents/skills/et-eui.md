# et-eui - ET EUI全链路与UnityMCP自动化实操

本 skill 用于 ET 项目中的 EUI 全流程工作：`Prefab创建`、`组件绑定`、`代码生成`、`运行逻辑`、`UnityMCP自动化执行`。

## 使用场景

- 新建/改造 `Dlg*`、`ES*`、`Item*` UI Prefab
- 批量或单体生成 EUI 代码
- 使用 UnityMCP 执行 UI 业务流程（生成→编译→运行→验收）
- 排查 UI 打不开、控件绑定 null、循环列表不刷新

---

## 一、项目内EUI真实流程（代码级）

### 1) 命名约定（决定生成结果）

核心规则见：`Unity/Assets/Scripts/Editor/EUI/UICodeSpawn/UICodeSpawner.cs`

- `Dlg*`：窗口主 Prefab（例：`DlgLogin`）
- `ES*`：可复用子 UI（例：`ESCommonUI`）
- `Item*`：循环列表项（例：`Item_test`）
- `E*`：可绑定控件节点
- `EG*`：可绑定普通容器节点（`RectTransform`）

扫描机制（`FindAllWidgets`）：

- `ES*` / `EG*` 记录 `RectTransform`
- `E*` 按组件白名单记录（Button/Text/InputField/Image/LoopScroll/...）
- 生成 Dlg 时默认不深入 `ES*` 子树，避免父界面重复绑定子 UI 内控件

### 2) 生成入口与批量生成

入口见：`Unity/Assets/Scripts/Editor/EUI/UIEditorController.cs`

- 单体：`GameObject/SpawnEUICode`（针对选中对象）
- 批量：`ET/EUICodeSpawn`
  - 扫描 `Assets/Bundles/UI/Dlg/`
  - 扫描 `Assets/Bundles/UI/Item/`
  - 扫描 `Assets/Bundles/UI/Common/`

### 3) 代码生成产物

#### Dlg 生成

- `HotfixView/Client/Demo/UI/DlgXxx/DlgXxxSystem.cs`
- `HotfixView/Client/Demo/UI/DlgXxx/Event/DlgXxxEventHandler.cs`
- `ModelView/Client/Demo/UI/DlgXxx/DlgXxx.cs`
- `HotfixView/Client/Demo/UIBehaviour/DlgXxx/DlgXxxViewComponentSystem.cs`
- `ModelView/Client/Demo/UIBehaviour/DlgXxx/DlgXxxViewComponent.cs`

同时更新：

- `Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowId.cs`
- `Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowItemRes.cs`

#### ES 生成

- `ModelView/Client/Demo/UIBehaviour/CommonUI/ESXxx.cs`
- `HotfixView/Client/Demo/UIBehaviour/CommonUI/ESXxxViewSystem.cs`

#### Item 生成

- `ModelView/Client/Demo/UIItemBehaviour/Item_xxx.cs`（类型 `Scroll_Item_xxx`）
- `HotfixView/Client/Demo/UIItemBehaviour/Item_xxxViewSystem.cs`

硬约束：`Item*` 必须挂 `LayoutElement`，否则生成失败。

### 4) 绑定组件代码生成逻辑

`CreateWidgetBindCode` 自动生成：

- 懒加载属性（`UIFindHelper.FindDeepChild`）
- 私有缓存字段
- `DestroyWidget()` 置空回收

循环项支持 `isCacheNode` 分支（缓存/非缓存两套读取逻辑）。

### 5) 运行时执行链路（ShowWindow）

关键文件：

- `Unity/Assets/Scripts/HotfixView/Client/Demo/EntryEvent3_InitClient.cs`
- `Unity/Assets/Scripts/HotfixView/Client/Plugins/EUI/UIPathComponentSystem.cs`
- `Unity/Assets/Scripts/HotfixView/Client/Plugins/EUI/UIEventComponentSystem.cs`
- `Unity/Assets/Scripts/HotfixView/Client/Plugins/EUI/UIComponentSystem.cs`

流程：

1. 启动挂载 `UIPathComponent/UIEventComponent/UIComponent`
2. `UIPathComponent` 建立 `WindowID_* -> Dlg*` 资源名映射
3. `UIEventComponent` 扫描 `[AUIEvent(...)]` 注册处理器
4. `UIComponent.ShowWindow/ShowWindowAsync` 加载并展示窗口
5. 依次触发：
   - `OnInitWindowCoreData`
   - `OnInitComponent`
   - `OnRegisterUIEvent`
   - `OnShowWindow`

UI 根节点路由（`EUIRootHelper`）：

- `NormalRoot` / `FixedRoot` / `PopUpRoot` / `OtherRoot`

### 6) 循环列表与预加载

关键文件：

- `Unity/Assets/Scripts/Loader/Plugins/EUI/LoopScrollRect/LoopScrollPrefabSource.cs`
- `Unity/Assets/Scripts/Loader/Plugins/EUI/GameObjectPoolHelper.cs`

要点：

- `LoopScrollRect.prefabSource.prefabName` 作为对象池键
- `WindowItemRes` 声明窗口打开前需预热的 Item
- `AddItemRefreshListener((Transform, index) => ...)` 驱动每项刷新

---

## 二、UnityMCP自动化SOP（可落地）

> 目标：统一“改UI后自动化验收”的固定流程。

### Step 0：连接预检

依次执行：

1. 读取实例：`read_mcp_resource(mcpforunity://instances)`
2. 设活动实例：`set_active_instance("Unity@hash")`
3. 读项目信息：`read_mcp_resource(mcpforunity://project/info)`
4. 读编辑器状态：`read_mcp_resource(mcpforunity://editor/state)`
5. 读当前场景：`manage_scene(action="get_active")`

若出现 `session not ready`：

- 重试 `set_active_instance`
- 延迟后重试 `editor/state`

### Step 1：执行EUI代码生成

推荐直接走菜单（与项目约定一致）：

- `execute_menu_item(menu_path="ET/EUICodeSpawn")`

### Step 2：刷新+编译

执行：

- `refresh_unity(mode="force", compile="request", scope="all", wait_for_ready=true)`

读取控制台：

- `read_console(action="get", count="200", include_stacktrace="true")`

### Step 3：运行冒烟

1. `manage_editor(action="play", wait_for_completion="true")`
2. `read_console(...)` 检查异常
3. `manage_scene(action="get_hierarchy", parent="Global/UI")` 验证 4 个 UI Root
4. `manage_editor(action="stop", wait_for_completion="true")`

### Step 4：产物验收

检查以下目录和文件是否同步：

- `Unity/Assets/Scripts/ModelView/Client/Demo/UI/`
- `Unity/Assets/Scripts/ModelView/Client/Demo/UIBehaviour/`
- `Unity/Assets/Scripts/ModelView/Client/Demo/UIItemBehaviour/`
- `Unity/Assets/Scripts/HotfixView/Client/Demo/UI/`
- `Unity/Assets/Scripts/HotfixView/Client/Demo/UIBehaviour/`
- `Unity/Assets/Scripts/HotfixView/Client/Demo/UIItemBehaviour/`
- `Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowId.cs`
- `Unity/Assets/Scripts/ModelView/Client/Plugins/EUI/WindowItemRes.cs`

---

## 三、可直接复用的UnityMCP调用清单

### 模板A：批量生成并检查日志

1. `execute_menu_item("ET/EUICodeSpawn")`
2. `refresh_unity(mode="force", compile="request", scope="all", wait_for_ready=true)`
3. `read_console(action="get", count="200", include_stacktrace="true")`

### 模板B：校验UI根节点

1. `manage_scene(action="get_hierarchy", parent="Global/UI", max_depth="2", max_nodes="200")`

期待至少包含：

- `NormalRoot`
- `PopUpRoot`
- `FixedRoot`
- `OtherRoot`

### 模板C：播放态冒烟

1. `manage_editor(action="play", wait_for_completion="true")`
2. `read_console(action="get", count="300", include_stacktrace="true")`
3. `manage_editor(action="stop", wait_for_completion="true")`

---

## 四、落地规范（避免后续被覆盖）

优先手改：

- `HotfixView/Client/Demo/UI/DlgXxx/DlgXxxSystem.cs`
- `HotfixView/Client/Demo/UI/DlgXxx/Event/DlgXxxEventHandler.cs`

避免手改（会被重新生成覆盖）：

- `*ViewComponent.cs`
- `*ViewComponentSystem.cs`
- 自动生成的 `UIItemBehaviour` 文件

---

## 五、常见问题速查

### 1) ShowWindow 失败

- 检查 `WindowId.cs` 是否有 `WindowID_Xxx`
- 检查 `AUIEvent(WindowID.WindowID_Xxx)` 处理器是否存在
- 检查 `DlgXxx.prefab` 名称是否与映射一致

### 2) 控件属性 null

- 检查节点是否按 `E*/EG*/ES*` 命名
- 检查层级路径变化是否导致 `FindDeepChild` 失效
- 检查同名节点冲突

### 3) 循环列表不显示

- `Item*` 是否挂 `LayoutElement`
- `prefabSource.prefabName` 是否正确
- `WindowItemRes` 是否包含对应 Item
- 是否注册 `AddItemRefreshListener`

### 4) UnityMCP偶发失连

- 重新 `set_active_instance`
- PlayMode/域重载后等待几秒再执行
- 先跑只读探测（instances/editor state）再跑修改命令

---

## 六、与其它skills联动

- 架构规范：`/et-arch`
- 构建编译：`/et-build`
- 运行测试：`/et-test-run`

当任务是 EUI 且涉及自动化执行时，优先启用本 skill，再按需联动其它 skill。

