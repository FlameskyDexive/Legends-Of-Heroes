<div align="center">
  <h2 href="https://github.com/FlameskyDexive/Legends-Of-Heroes">
    <!-- <img src="./SamplePictures/CrazyCarIcon.png"  width="80px" height="80px"> -->
  </h2>
  <h2 align="center">
    Legends-Of-Heroes
  </h2>  
    <img src="https://img.shields.io/github/stars/FlameskyDexive/Legends-Of-Heroes?style=plastic" alt="">
    <img src="https://img.shields.io/github/forks/FlameskyDexive/Legends-Of-Heroes?color=09F709&label=forks&style=plastic" alt="">
    <img src="https://img.shields.io/github/license/FlameskyDexive/Legends-Of-Heroes?color=22DDB8&label=license&style=plastic" alt="">
    <img src="https://img.shields.io/github/commit-activity/m/FlameskyDexive/Legends-Of-Heroes?color=AA8855&label=commit-activity&style=plasticc"alt="">
    <img src="https://img.shields.io/github/last-commit/FlameskyDexive/Legends-Of-Heroes?color=%231AE66B&label=last-commit&style=plastic" alt="">
</div>

English: please use your browser to translate to english

一个LOL风格的[球球大作战游戏](https://github.com/FlameskyDexive/Legends-Of-Heroes)，基于ET，使用状态同步
![loh22023532242551](https://user-images.githubusercontent.com/8274346/235951176-f96efa8f-d3e5-4089-a1c9-56643859b487.gif)

## Main
#### 基于C#双端框架[ET框架](https://github.com/egametang/ET)，[ETPlus](https://github.com/FlameskyDexive/ETPlus) ET8.1加强版(EUI+YooAsset+Luban)。 注意：当前Master正在同步ET8.1升级修改（基础热更流程，技能/Buff系统已经迁移完毕，开房间匹配跟ET8.1一样修改ConstValue数量即可，当前房间内已经支持玩家移动同步，尚缺技能、子弹同步）。
# 觉得项目不错的话麻烦右上角给个star哈.
#### 此游戏为ET的一个实践项目demo，玩法主要是球球大作战类型的吃食物吃敌人/被敌人吃的生存玩法。此项目采用状态同步，所有碰撞检测、技能、AI等逻辑都放在服务端执行。当前仍在开发中，具体功能模块及进度看下面的Todo即可

## 国内GitHub访问异常或者速度太慢可以前往Gitee [Legends-Of-Heroes](https://gitee.com/flamesky/Legends-Of-Heroes)拉取项目

## 环境：
- 1. 安装Unity2022.3.15f1，安装VS2022/Rider2023，安装[.Net8 SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0).
- 
## 运行步骤：
- 1. Unity打开编辑器菜单：ET/BuildTool,CodeMode选择Client,找到GlobalConfig, 勾选EnableDll，按F6编译客户端代码（请不要用IDE编译dll，当前仅支持Unity编译递增dll），Init场景找到Global上的Init脚本选择YooAsset运行模式为EditorSimulate
- 2. 打开ET.sln，编译整个项目，运行DotNet.App.(服务器、客户端拆分模式才需要单独运行，默认是Client-Server模式)
- 3. Unity运行游戏即可看到登录页面,输入账号密码登录即可

# TODO && Features
- [x] 接入UnityC#热更新框架[HybridCLR](https://github.com/focus-creative-games/hybridclr)，包含热更新资源检测下载流程。
- [x] 一键打包（支持HybridCLR模式一键打包，不需要分开处理，当前测过Win/Android），菜单栏：ET/Build/一键打包xxx
- [x] 接入Unity运行时可视化Log调试组件[Debugger](https://github.com/FlameskyDexive/Debugger)
- [x] 接入基于UGUI适配ET的UI框架[EUI](https://github.com/zzjfengqing/ET-EUI)
- [x] 接入[YooAsset](https://github.com/tuyoogame/YooAsset)资源管理打包热更框架，实现热更下载重载逻辑。
- [x] 摇杆控制角色移动，相机跟随，通过服务端广播位置同步
- [x] 实现一个比较基础版本的断线重连，返回登陆等操作。
- [x] 配置表接入强大的多平台配置方案 [Luban](https://github.com/focus-creative-games/luban)
- [ ] 房间大厅，匹配房友，每个房间最多20个玩家。
- [ ] 一个Demo关卡流程：有限的2d地图，随机生成食物，吃食物会变大，碰到敌人会产生伤害（大吞小，小死亡）。
- [x] 基础战斗技能框架设计，主动/被动技能释放(demo已经包含主动技能演示)。
- [x] Buff系统
- [x] 时间线技能事件系统
- [x] 子弹碰撞系统（碰撞检测使用[Box2dSharp](https://github.com/Zonciu/Box2DSharp)）
- [ ] 额外（有空的话）：2d moba玩法。

	              
## 特别鸣谢

感谢JetBrains公司提供的使用许可证！

<p><a href="https://www.jetbrains.com/?from=Legends-Of-Heroes">
<img src="https://user-images.githubusercontent.com/8274346/223466125-611c027a-61f3-4ea0-a96d-4052283da746.png" alt="JetBrains的Logo" width="20%" height="20%"></a></p>

## 友情链接/鸣谢
### [Fantasy](https://github.com/qq362946/Fantasy) Fantasy是基于.NET的高性能网络开发框架，支持主流协议，前后端分离
### [UniJoystick](https://github.com/Bian-Sh/UniJoystick) 一个基于UGUI通用摇杆组件
### [X-ET7](https://github.com/IcePower/X-ET7) ET7的一个分支，集成FGUI+YooAsset+Luban 
### [NKGMobaBasedOnET](https://github.com/wqaetly/NKGMobaBasedOnET) 烟雨的开源moba案例，基于ET5.X魔改
### [XAsset](https://github.com/xasset/xasset) 一个很高效易用强大的资源管理系统（打包/加载/热更）
### [ETPro](https://github.com/526077247/ETPro) ET加强版，基于ET6.0，自带技能系统、UI框架、镜像版无缝大世界。

## Star History

![Star History Chart](https://api.star-history.com/svg?repos=FlameskyDexive/Legends-Of-Heroes)
