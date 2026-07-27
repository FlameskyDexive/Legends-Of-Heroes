<div align="center">
  <h2 href="https://github.com/FlameskyDexive/Legends-Of-Heroes">
    <!-- <img src="./SamplePictures/CrazyCarIcon.png"  width="80px" height="80px"> -->
  </h2>
  <h2 align="center">
    Legends-Of-Heroes
  </h2>
  <p><em>一个基于 ET 8.x、Unity 2022.3 与 .NET 10 构建的开源、可投入生产的多人游戏框架。</em></p>
    <img src="https://img.shields.io/github/stars/FlameskyDexive/Legends-Of-Heroes?style=plastic" alt="">
    <img src="https://img.shields.io/github/forks/FlameskyDexive/Legends-Of-Heroes?color=09F709&label=forks&style=plastic" alt="">
    <img src="https://img.shields.io/github/license/FlameskyDexive/Legends-Of-Heroes?color=22DDB8&label=license&style=plastic" alt="">
    <img src="https://img.shields.io/github/commit-activity/m/FlameskyDexive/Legends-Of-Heroes?color=AA8855&label=commit-activity&style=plasticc"alt="">
    <img src="https://img.shields.io/github/last-commit/FlameskyDexive/Legends-Of-Heroes?color=%231AE66B&label=last-commit&style=plastic" alt="">
</div>

[English](./README.md) | **简体中文**

此项目为基于 [ET 框架](https://github.com/egametang/ET) 搭建的前后端游戏框架，包含基础热更流程以及较为完善的战斗系统（当前已有 ECS 技能 + Buff 系统；技能编辑器 / 行为树编辑器开发中）。[英雄传说](https://github.com/FlameskyDexive/Legends-Of-Heroes) 采用**状态同步**：所有碰撞检测、技能、AI 等逻辑都放在服务端执行。

<a href="./Document/loh.mp4"><img src="./Document/et-bt.png" alt="Legends-Of-Heroes 演示视频（点击播放）" width="100%"></a>

> 📺 **点击上方图片观看演示视频。** 原始 MP4 位于 [`Document/loh.mp4`](./Document/loh.mp4)，在 GitHub 直接打开该文件会以网页内嵌播放器呈现。

---

## 📐 架构图（Architecture Diagram）

Legends-Of-Heroes 采用经典的 ET 风格 **ECS + Actor** 分层架构。下图展示了服务端进程、Unity 客户端、共享的 `Model` / `Hotfix` 程序集以及接入其中的第三方生态之间的关系。

```
                         ┌─────────────────────────────────────────────┐
                         │                   客户端 (CLIENT)             │
                         │            Unity 2022.3.62f3                 │
                         │  ┌─────────────────────────────────────────┐ │
                         │  │  UI 层：EUI (UGUI) / YIUI                │ │
                         │  │  摇杆 · 相机跟随 · Debugger              │ │
                         │  ├─────────────────────────────────────────┤ │
                         │  │  HybridCLR（热更新）                     │ │
                         │  │  YooAsset 3.0（资源 / bundle）           │ │
                         │  ├─────────────────────────────────────────┤ │
                         │  │  ModelView  ·  HotfixView  (客户端 ECS)  │ │
                         │  └───────────────────┬─────────────────────┘ │
                         └──────────────────────┼──────────────────────┘
                                                │  TCP / KCP / WebSocket
                                                │  状态同步 + 位置广播
                         ┌──────────────────────┼──────────────────────┐
                         │                      ▼                       │
                         │              服务端进程 (SERVER PROCESS)      │
                         │                  .NET 10                     │
                         │  ┌─────────────────────────────────────────┐ │
                         │  │  Actor Location · 消息路由               │ │
                         │  │  (login · lobby · room · map)            │ │
                         │  ├─────────────────────────────────────────┤ │
                         │  │  Gameplay ECS                            │ │
                         │  │  技能 · Buff · 时间线 · 碰撞              │ │
                         │  │  行为树 · AOI · 移动 · Lockstep           │ │
                         │  ├─────────────────────────────────────────┤ │
                         │  │  Model  ·  Hotfix  (服务端 ECS)           │ │
                         │  ├─────────────────────────────────────────┤ │
                         │  │  DB · Redis · 配置 (Luban)               │ │
                         │  └─────────────────────────────────────────┘ │
                         └─────────────────────────────────────────────┘
                              ▲                                   ▲
                              │                                   │
                ┌─────────────┴──────────┐          ┌─────────────┴────────────┐
                │     共享程序集           │          │   工具链 / AI 工作流      │
                │  cn.etetet.* packages   │          │  AIBridge CLI · UnityMCP │
                │  (Model / Hotfix)       │          │  AI Agents（可插拔）      │
                └────────────────────────┘          └──────────────────────────┘
```

### 分层说明

| 层级 | 职责 | 关键包 |
|------|------|--------|
| **服务端（`.NET 10`）** | 游戏逻辑、AI、碰撞、网络、持久化 | `core`、`actorlocation`、`netinner`、`statesync`、`skill`、`spell`、`behaviortree`、`collision`、`aoi`、`db` |
| **客户端（`Unity 2022.3`）** | 渲染、输入、UI、本地预测 | `eui`、`yiui*`、`hybridclr`、`yooassets`、`move` |
| **共享 `Model` / `Hotfix`** | ECS 组件、系统、可热更逻辑 | 所有 `cn.etetet.*` 包 |
| **工具链** | 构建、代码生成、AI 辅助开发 | `harness`、`unitybridge`、AIBridge CLI、UnityMCP |

> 完整模块清单位于 [`Packages/`](./Packages)（70+ 个 `cn.etetet.*` 包）。每个包都遵循 ET 的 `Model` / `ModelView` / `Hotfix` / `HotfixView` 程序集拆分，以支持热重载。

---

## ✨ 功能特性（Features）

### 🌐 网络同步（Networking）
- **状态同步** —— 所有权威玩法逻辑在服务端运行，客户端只渲染广播状态。
- **Actor 模型消息** —— `actorlocation` + `netinner` 提供跨进程的位置透明 RPC 与消息路由。
- **多协议传输** —— ET 网络层支持 TCP / KCP / WebSocket。
- **断线重连与会话** —— 基础的断线重连、返回登陆流程。
- **分布式就绪** —— `servicediscovery` + `router` 支持多进程拓扑（通过 .NET Aspire 编排）。

### 🤖 AI Agents
- 可插拔的 AI 工作流钩子，用于 AI 辅助开发（见 `Packages/cn.etetet.harness/`）。
- 设计上可与外部 AI Agents / 兼容 MCP 的工具集成，用于代码生成、调试与内容创作。

### 🌳 行为树（Behavior Tree）
- 引擎内 **行为树编辑器**，用于编写 NPC / 敌人 AI。
- 节点库通过 `btree` / `btnode` / `btreedemo` 包暴露。

![et-bt](./Document/et-bt.png)

### ⚔️ 技能系统（Skill System）
- 基于 ECS 的技能框架，支持**主动 / 被动**技能。
- 服务端权威的施法、目标选取与冷却。
- Ball Battle demo 中已接入一个主动技能演示。

### 💥 Buff 系统（Buff System）
- 可组合的 Buff / 效果管线，与技能系统、时间线系统集成。
- 叠加规则、持续时间管理、修饰器应用全部在服务端执行。

### 📊 Luban
- 配置由 [Luban](https://github.com/focus-creative-games/luban) 驱动 —— 多平台、类型安全、代码生成的配置方案。
- 工作流已集成进 `harness` 构建管线（`et-luban` / `et-excel`）。

### 📦 YooAsset
- 通过 [YooAsset 3.0](https://github.com/tuyoogame/YooAsset) 实现资源打包、加载与热更新。
- 与 HybridCLR 组合实现代码 + 资源全量热更。

### 🔌 UnityMCP
- 兼容 MCP 的桥接层，用于编辑器自动化（编译、刷新、读日志、Prefab/场景操作）。
- 通道优先级（AIBridge / UnityMCP / UnityBridge）见 [`AGENTS.md`](./AGENTS.md)。

### 🎮 其它
- **HybridCLR** —— C# 热更新，含检测与下载流程。
- **Box2DSharp** —— 2D 物理 / 子弹碰撞。
- **EUI / YIUI** —— 适配 ET 的 UGUI 框架。
- **AOI** —— 大世界的兴趣区域管理。
- **Lockstep / TrueSync** —— 帧同步与确定性同步原语。
- **一键打包** —— 菜单 `ET/Build/...`，已在 Win/Android 验证。

---

## 🗺️ 路线图（Roadmap）

| 状态 | 事项 |
|:----:|------|
| 🚧 | **时间轴技能编辑器**配套战斗系统 |
| ✅ | 行为树编辑器配套战斗（行为树已发布） |
| 🚧 | **房间大厅 & 匹配** —— 每个房间最多 20 个玩家 |
| ✅ | 技能框架（主动 / 被动）+ Buff 系统 |
| ✅ | 时间线技能事件系统 |
| ✅ | 子弹碰撞系统（Box2DSharp） |
| ✅ | HybridCLR + YooAsset 热更流程 |
| ✅ | 一键打包（Win/Android） |
| 🚧 | **2D MOBA 玩法**（拓展目标） |
| 🔜 | 更多逐模块的视频解说 |

图例：✅ 已完成 · 🚧 进行中 / 计划中 · 🔜 未来

---

## 📋 环境要求

- **Unity**：2022.3.62f3
- **IDE**：Visual Studio 2022 或 Rider 2023
- **.NET SDK**：10.0
- **操作系统**：Windows（主要平台）；部分工具在 macOS/Linux 上可能不可用。

## 🚀 快速开始

1. **克隆**
   ```bash
   git clone https://github.com/FlameskyDexive/Legends-Of-Heroes.git
   cd Legends-Of-Heroes
   ```
2. **打开** Unity 2022.3.62f3 并让其导入项目。
3. **参照** [`Book/`](./Book) 目录下的运行指南（`1.1运行指南.md`）运行。

> 国内 GitHub 访问异常或速度太慢？可前往 Gitee 镜像拉取：[Legends-Of-Heroes](https://gitee.com/flamesky/Legends-Of-Heroes)。

---

## 🤝 参与贡献（Contributing）

欢迎贡献 —— Bug 报告、特性建议、文档完善与 PR 均受欢迎。提交 PR 前请先阅读 [CONTRIBUTING.md](./CONTRIBUTING.md)。

快速链接：[Issue 列表](https://github.com/FlameskyDexive/Legends-Of-Heroes/issues) · [CONTRIBUTING.md](./CONTRIBUTING.md) · [行为准则](./CONTRIBUTING.md#code-of-conduct)

如果这个项目对你有帮助，右上角的 ⭐ 是对我们最大的鼓励！

---

## 📺 视频解说

当前暂时只录了一个演示视频 —— [实操 / 打包匹配](https://www.bilibili.com/video/BV1sP6fY2EQU/)，后续会把每个模块都出对应一个视频来解说设计思路跟使用方式。

---

## 🙏 特别鸣谢

感谢 JetBrains 公司提供的使用许可证！

<p><a href="https://www.jetbrains.com/?from=Legends-Of-Heroes">
<img src="https://user-images.githubusercontent.com/8274346/223466125-611c027a-61f3-4ea0-a96d-4052283da746.png" alt="JetBrains的Logo" width="20%" height="20%"></a></p>

## 🔗 友情链接 / 鸣谢
### [Fantasy](https://github.com/qq362946/Fantasy) —— 基于 .NET 的高性能网络开发框架，支持主流协议，前后端分离。
### [UniJoystick](https://github.com/Bian-Sh/UniJoystick) —— 基于 UGUI 的通用摇杆组件。
### [X-ET7](https://github.com/IcePower/X-ET7) —— ET7 的一个分支，集成 FGUI + YooAsset + Luban。
### [NKGMobaBasedOnET](https://github.com/wqaetly/NKGMobaBasedOnET) —— 烟雨的开源 MOBA 案例，基于 ET5.X 魔改。
### [XAsset](https://github.com/xasset/xasset) —— 高效易用强大的资源管理系统（打包 / 加载 / 热更）。
### [ETPro](https://github.com/526077247/ETPro) —— ET 加强版，基于 ET6.0，自带技能系统、UI 框架、镜像版无缝大世界。

---

## 📄 开源协议

本项目基于 [MIT License](./LICENSE) 开源。

## ⭐ Star History

![Star History Chart](https://api.star-history.com/svg?repos=FlameskyDexive/Legends-Of-Heroes)
