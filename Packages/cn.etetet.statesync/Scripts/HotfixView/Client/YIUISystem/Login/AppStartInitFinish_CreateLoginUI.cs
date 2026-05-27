// Login 启动已切换为 EUI（EUI 版位于 Scripts/HotfixView/Client/EUI/DlgLogin/AppStartInitFinish_CreateLoginUI.cs）。
// YIUI 基础设施初始化（YIUIEventComponent / YIUIMgrComponent）也一并停用：
// 默认 UI 框架使用 EUI，YIUI 不再加载，避免加载 YIUIRoot 失败。
// 保留 namespace 空文件以兼容 Git 历史；如需彻底删除可直接清除整个 YIUISystem 目录。
namespace ET.Client
{
}
