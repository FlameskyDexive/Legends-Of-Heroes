# Legends-Of-Heroes
English: please use your browser to translate to english

一个LOL风格的球球大作战游戏，基于ET7.2，使用状态同步
![loh22023532242551](https://user-images.githubusercontent.com/8274346/235951176-f96efa8f-d3e5-4089-a1c9-56643859b487.gif)

## Main
#### 基于C#双端框架[ET7.2 Master](https://github.com/egametang/ET) ，同步到ET主干详情请看日志。注意：已经升级.Net7，请安装.
#### 此游戏为ET7.2的一个实践项目demo，玩法主要是球球大作战类型的吃食物吃敌人/被敌人吃的生存玩法。此项目采用状态同步，所有碰撞检测、技能、AI等逻辑都放在服务端执行。当前仍在开发中，具体功能模块及进度看下面的Todo即可

## 环境：
- 1. 安装Unity2022.3.0，安装VS2022/Rider2023，安装[.Net7 SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet/7.0).
- 
## 运行步骤：
- 1. Unity打开编辑器菜单：ET/BuildTool,CodeMode选择Client,然后点击BuildModelAndHotfix.
- 2. 打开ET.sln，编译整个项目，运行DotNet.App.(服务器、客户端拆分模式才需要单独运行，默认是Client-Server模式)
- 3. Unity运行游戏即可看到登录页面,输入账号密码登录即可

# TODO && Features

- [x] 接入Unity运行时可视化Log调试组件[Debugger](https://github.com/FlameskyDexive/Debugger)
- [x] 接入基于UGUI适配ET的UI框架[EUI](https://github.com/zzjfengqing/ET-EUI)
- [x] 接入[YooAsset](https://github.com/tuyoogame/YooAsset)资源管理打包热更框架
- [x] 摇杆控制角色移动，相机跟随，通过服务端广播位置同步
- [x] 配置表支持多Key（也可自行接入强大的多平台配置方案 [Luban](https://github.com/focus-creative-games/luban)，当前Demo用不到那么复杂excel就不进行接入了）。
- [ ] 房间大厅，匹配房友，每个房间最多20个玩家。
- [ ] 一个Demo关卡流程：有限的2d地图，随机生成食物，吃食物会变大，碰到敌人会产生伤害（大吞小，小死亡）。
- [x] 一个比较基本的战斗技能框架设计，主动/被动技能释放(demo已经包含主动技能演示)。
- [x]       时间线技能事件系统
- [x]       子弹碰撞系统（碰撞检测使用[Box2dSharp](https://github.com/Zonciu/Box2DSharp)）
- [ ] 额外（有空的话）：2d moba玩法。

## 补充说明
1. 多key配置说明，表格配置多key如下，字段列头顶增加“key”标识即可，最多支持4个key，组合key最后会合并成一个long字段存储，4个key最大数值分别为：32位，16位，8位，8位。
![image](https://user-images.githubusercontent.com/8274346/223321430-a1825695-95b1-4f15-8bba-83dad8e0b84b.png)

      示例，读取技能等级表中技能id=1001，level=2的数据： 
    
      SkillLevelConfig skillLevel = SkillLevelConfigCategory.Instance.GetByKeys(1001, 2);
	              

## 特别鸣谢

感谢JetBrains公司提供的使用许可证！

<p><a href="https://www.jetbrains.com/?from=Legends-Of-Heroes">
<img src="https://user-images.githubusercontent.com/8274346/223466125-611c027a-61f3-4ea0-a96d-4052283da746.png" alt="JetBrains的Logo" width="20%" height="20%"></a></p>

## 友情链接/鸣谢
### [X-ET7](https://github.com/IcePower/X-ET7) ET7的一个分支，集成FGUI+YooAsset+Luban 
### [NKGMobaBasedOnET](https://github.com/wqaetly/NKGMobaBasedOnET) 烟雨的开源moba案例，基于ET5.X魔改
### [XAsset](https://github.com/xasset/xasset) 一个很高效易用强大的资源管理系统（打包/加载/热更）
### [ETPro](https://github.com/526077247/ETPro) ET加强版，基于ET6.0，自带技能系统、UI框架、镜像版无缝大世界。
