# ET EUI 提测前快速勾选单

用途：EUI 改动后，提测 / 合并前 1 分钟自检。详细机制见 `et-eui-mechanism.md`，步骤见 `et-eui-runbook.md`。

---

## A. 命名与落点

- [ ] 窗口逻辑类命名为 `DlgXxx`，预制体命名为 `Dlg{Name}`（与 `WindowID_Xxx` 后缀一致）
- [ ] 子 UI 命名 `ESXxx`，循环项命名 `ItemXxx`
- [ ] 可绑定控件命名 `E*`，可绑定容器命名 `EG*`
- [ ] 循环项 `Item*` 已挂 `LayoutElement`
- [ ] 业务窗口放在业务包（如 `cn.etetet.statesync` / `cn.etetet.lockstep`），框架改动才放 `cn.etetet.eui`
- [ ] 框架包未反向依赖业务包

---

## B. 5 文件成套 + 注册

- [ ] `DlgXxx.cs`（`[ComponentOf(typeof(UIBaseWindow))]`, `IAwake, IUILogic`, `View` 属性）
- [ ] `DlgXxxViewComponent.cs`（`[ComponentOf(typeof(DlgXxx))][EnableMethod]`, `E*` 懒加载 + `DestroyWidget()`）
- [ ] `DlgXxxViewComponentSystem.cs`（`Awake` 赋值 `uiTransform`，`Destroy` 调 `DestroyWidget()`）
- [ ] `DlgXxxEventHandler.cs`（`[AUIEvent(WindowID.WindowID_Xxx)] : IAUIEventHandler` 接好 6 个回调）
- [ ] `DlgXxxSystem.cs`（`[FriendOf]` 静态类，`RegisterUIEvent` / `ShowWindow` / 业务方法）
- [ ] `WindowID.cs` 已加 `WindowID_Xxx`
- [ ] 用循环列表时 `WindowItemRes.cs` 已登记

快速命令（PowerShell）：

```powershell
pwsh -Command "rg -n 'WindowID_' Packages/cn.etetet.eui/Scripts/ModelView/Client/EUI/WindowID.cs"
```

---

## C. 编译与控制台

- [ ] `dotnet build ET.sln` 通过，无分析器报错（EntityRef/await、Module、命名空间）
- [ ] 用 `et-unitybridge` 触发 `Refresh`，Unity 导入新文件、生成 `.meta`
- [ ] 控制台无新增 `Error/Exception/NullReferenceException`、无 `uiTransform is null`、无 `uiPath is not Exist`

---

## D. 运行态冒烟（et-unitybridge）

- [ ] `EnterPlay` 成功
- [ ] 目标窗口可正常打开/关闭（`ShowWindow<DlgXxx>()`）
- [ ] `Global/UI` 下存在四层 Root：`NormalRoot` / `PopUpRoot` / `FixedRoot` / `OtherRoot`
- [ ] `ExitPlay` 后控制台仍无新增 UI 错误

---

## E. 代码边界（防 await 崩溃 / 误存 Entity）

- [ ] 业务逻辑写在 `DlgXxxSystem` / `DlgXxxEventHandler`，不写进 `*ViewComponent`
- [ ] 闭包/字段只持有 `EntityRef`，不直接保存 `Entity`
- [ ] `await` 后通过 `EntityRef` 重新获取 Entity 再用

---

## F. 提交前核对

- [ ] `git status` 仅包含本次预期改动（含成套 5 文件 + 对应 `.meta`）
- [ ] 无临时调试日志残留
- [ ] 至少 1 条主交互手动验证通过

快速命令（PowerShell）：

```powershell
pwsh -Command "git status --short"
```
