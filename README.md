# Legends-Of-Heroes
###English: please use your browser to translate to english

一个LOL风格的球球大作战游戏，基于ET7.2，使用状态同步

![image](https://user-images.githubusercontent.com/8274346/221614980-2390ad29-ae4b-4877-9bb5-30b730dd0819.png)

## Main
#### 基于C#双端框架[ET7.2](https://github.com/egametang/ET)
#### 此游戏为ET7.2的一个实践项目demo，玩法主要是球球大作战类型的吃食物吃敌人/被敌人吃的生存玩法。此项目采用状态同步，所有碰撞检测、技能、AI等逻辑都放在服务端执行。当前仍在开发中，具体功能模块及进度看下面的Todo即可

## 运行步骤：
- 1. Unity打开编辑器菜单：ET/BuildTool,CodeMode选择Client,然后点击BuildModelAndHotfix.
- 2. 打开ET.sln，编译整个项目，运行DotNet.App.
- 3. Unity运行游戏即可看到登录页面,输入账号密码登录即可

# TODO && Features

- [x] 接入Unity运行时可视化Log查看组件[Debugger](https://github.com/FlameskyDexive/Debugger)
- [x] 接入UGUI框架[EUI](https://github.com/FlameskyDexive/Debugger)
- [x] 接入[YooAsset](https://github.com/tuyoogame/YooAsset)资源管理打包热更框架
- [x] 接入2D物理碰撞引擎[Box2dSharp](https://github.com/Zonciu/Box2DSharp)
- [ ] 配置表支持多Key
- [ ] 摇杆控制角色移动，通过服务端广播位置同步
- [ ] 一个Demo关卡流程：有限的2d地图，随机生成食物，吃食物会变大，碰到敌人会产生伤害（大吞小，小死亡）。
- [ ] 一个比较基本的战斗技能框架设计，主动/被动技能释放。


###

## 鸣谢
### [X-ET7, a branch of ET7](https://github.com/IcePower/X-ET7)
