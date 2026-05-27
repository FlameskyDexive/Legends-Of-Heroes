# cn.etetet.eui

EUI 基础 UI 框架包，移植自原 Legends-Of-Heroes 项目 EUI 系统。

## 使用规范

- 所有 EUI 代码必须放入本包，禁止散布到其它包。
- 业务侧 UI 对话框（DlgXxx）按所属业务包放置，例如 Login 对话框放在 `cn.etetet.statesync`。
- 不要让本包依赖业务包（如 cn.etetet.login）。
- 命名空间使用 `ET.Client`。

## 与 YIUI 共存

- EUI 与 YIUI 并存，互不依赖。
- Login 入口已切换为 EUI。
- 其它 YIUI 界面在迁移完成前保持现状。

## 包依赖

仅依赖：
- `cn.etetet.core`
- `cn.etetet.loader`
- `cn.etetet.yooassets`

## LoopScrollRect 复用策略

原 EUI 项目自带一份 marchingbytes 的 `UnityEngine.UI.LoopScrollRect`（约 3000 行）。
本项目已有 `cn.etetet.yiuiloopscrollrectasync` 内置同款 `UnityEngine.UI.LoopScrollRect`，
若再在 EUI 内复制会产生类型重复冲突。

策略：**直接复用** `cn.etetet.yiuiloopscrollrectasync` 中的 `UnityEngine.UI.LoopScrollRect`。
- EUI 业务侧若需要循环列表，使用同名 `UnityEngine.UI.LoopScrollRect` 即可。
- 不在 EUI 包内重复移植 LoopScrollRect 文件。
- 若未来需要去 YIUI 依赖，可将 LoopScrollRect 抽出为独立的 `cn.etetet.loopscrollrect` 包。
