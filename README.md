# Legends-Of-Heroes
English: please use your browser to translate to english

一个LOL风格的球球大作战游戏，基于ET7.2，使用状态同步

![image](https://user-images.githubusercontent.com/8274346/222974074-9d661e84-6c81-4286-b9f6-859b4eee1e62.png)
![battle](https://user-images.githubusercontent.com/8274346/222968848-f183ccd6-a268-4c2e-af1f-4a75f37a1512.png)

## Main
#### 基于C#双端框架[ET7.2](https://github.com/egametang/ET)
#### 此游戏为ET7.2的一个实践项目demo，玩法主要是球球大作战类型的吃食物吃敌人/被敌人吃的生存玩法。此项目采用状态同步，所有碰撞检测、技能、AI等逻辑都放在服务端执行。当前仍在开发中，具体功能模块及进度看下面的Todo即可

## 运行步骤：
- 1. Unity打开编辑器菜单：ET/BuildTool,CodeMode选择Client,然后点击BuildModelAndHotfix.
- 2. 打开ET.sln，编译整个项目，运行DotNet.App.(服务器、客户端拆分模式才需要单独运行，默认是Client-Server模式)
- 3. Unity运行游戏即可看到登录页面,输入账号密码登录即可

# TODO && Features

- [x] 接入Unity运行时可视化Log查看组件[Debugger](https://github.com/FlameskyDexive/Debugger)
- [x] 接入UGUI框架[EUI](https://github.com/FlameskyDexive/Debugger)
- [x] 接入[YooAsset](https://github.com/tuyoogame/YooAsset)资源管理打包热更框架
- [x] 接入2D物理碰撞引擎[Box2dSharp](https://github.com/Zonciu/Box2DSharp)
- [x] 摇杆控制角色移动，通过服务端广播位置同步
- [ ] 配置表支持多Key
- [ ] 房间大厅，匹配房友，每个房间最多20个玩家。
- [ ] 一个Demo关卡流程：有限的2d地图，随机生成食物，吃食物会变大，碰到敌人会产生伤害（大吞小，小死亡）。
- [ ] 一个比较基本的战斗技能框架设计，主动/被动技能释放。


###

## 鸣谢
### ET7的一个分支，集成FGUI+YooAsset+Luban [X-ET7](https://github.com/IcePower/X-ET7)
### 烟雨的moba [NKGMobaBasedOnET](https://github.com/wqaetly/NKGMobaBasedOnET)
