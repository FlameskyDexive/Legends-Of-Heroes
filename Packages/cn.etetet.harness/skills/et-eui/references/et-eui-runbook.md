# ET EUI 执行清单（仅步骤）

改完 EUI 代码或预制体后的“编译 → 刷新 → 运行 → 验收”可重复流程。只讲步骤，原理见 `et-eui-mechanism.md`。

> 本项目编辑器内操作统一走 `et-unitybridge`（不使用 UnityMCP）。具体桥接命令以 `Packages/cn.etetet.unitybridge/skills/et-unitybridge/SKILL.md` 为准，本清单只给顺序与判定。

---

## 0. 预备条件

- 已按 5 文件模板手写完 `DlgXxx`（含 `WindowID_Xxx` 枚举项），预制体命名为 `DlgXxx` 且 `Item*` 已挂 `LayoutElement`。
- 新建 `.cs` 不要手工建 `.meta`，由 Unity 刷新生成。

---

## 1. 编译（et-build）

```powershell
dotnet build ET.sln
```

- 通过标准：无编译错误、无分析器报错（EntityRef/await、Module、命名空间等）。
- 有错先修代码再继续，不要带着编译错误进编辑器。

---

## 2. 刷新与生成（et-unitybridge）

- 用 `et-unitybridge` 触发 `Refresh`（必要时 `RegenProject`），让 Unity 导入新文件、生成 `.meta` 与工程文件。
- 读取宿主状态确认 Unity 不在编译中、无新增报错。

---

## 3. 控制台验收（et-unitybridge）

- 用 `et-unitybridge` 读取 Unity 控制台。关注关键字：
  - `Error` / `Exception` / `NullReferenceException`
  - `uiTransform is null`
  - `is not have any windowId` / `uiPath is not Exist`（WindowID 或预制体名不匹配）
- 通过标准：无新增 UI 相关错误。

---

## 4. 运行态冒烟（et-unitybridge）

1. `EnterPlay` 进入 PlayMode。
2. 读取控制台，确认无新增异常。
3. 读取 `Global/UI` 层级，确认存在四个 Root：`NormalRoot`、`PopUpRoot`、`FixedRoot`、`OtherRoot`。
4. 触发目标窗口（或在测试入口调用 `UIComponent.ShowWindow<DlgXxx>()`），确认能正常打开/关闭。
5. `ExitPlay` 退出 PlayMode，再次读控制台确认无残留报错。

---

## 5. 文件产物核对

业务包内应成套出现（按改动类型）：

- `Packages/cn.etetet.{业务包}/Scripts/ModelView/Client/EUI/DlgXxx/DlgXxx.cs`
- `Packages/cn.etetet.{业务包}/Scripts/ModelView/Client/EUI/DlgXxx/DlgXxxViewComponent.cs`
- `Packages/cn.etetet.{业务包}/Scripts/HotfixView/Client/EUI/DlgXxx/DlgXxxSystem.cs`
- `Packages/cn.etetet.{业务包}/Scripts/HotfixView/Client/EUI/DlgXxx/DlgXxxViewComponentSystem.cs`
- `Packages/cn.etetet.{业务包}/Scripts/HotfixView/Client/EUI/DlgXxx/DlgXxxEventHandler.cs`

框架包内（如有改动）：

- `Packages/cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowID.cs`（新增 `WindowID_Xxx`）
- `Packages/cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowItemRes.cs`（如用循环列表预热）

推荐本地命令（PowerShell）：

```powershell
pwsh -Command "git status --short"
pwsh -Command "rg -n 'WindowID_' Packages/cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowID.cs"
```

---

## 6. 回归风险点（必查）

- `Item*` 必须挂 `LayoutElement`。
- 控件命名 `E*`，`FindDeepChild` 名字与预制体一致。
- `[AUIEvent(WindowID.WindowID_Xxx)]` 与 `WindowID` 项、预制体名三者一致。
- `await` 后访问 Entity 用 `EntityRef` 重新获取（叠加 `et-async`）。
- 业务窗口放在业务包，框架包不反向依赖业务包。

---

## 7. 故障恢复最小操作（et-unitybridge）

当桥接返回 `session not ready` / `timeout` 或域重载后：

1. 用 `et-unitybridge` 读取宿主状态（`HostState` / `Ping`）确认在线。
2. 等待编译/域重载完成。
3. 重新执行第 2~4 步（刷新 → 读控制台 → 冒烟）。
