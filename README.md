# Legends-Of-Heroes
A battle of balls game, lol style
一个LOL风格的球球大作战游戏，基于ET7.2，使用状态同步

![image](https://user-images.githubusercontent.com/8274346/221614980-2390ad29-ae4b-4877-9bb5-30b730dd0819.png)

## Main
### [Base on ET7.2](https://github.com/egametang/ET)
### Multiplayers battle in a map

## 运行步骤：
- 1. Unity打开编辑器菜单：ET/BuildTool,CodeMode选择Client,然后点击BuildModelAndHotfix.
- 2. Unity打开YooAsset/AssetBundleBuilder, 点击构建.
- 3. 打开ET.sln，编译整个项目，运行DotNet.App.
### 4. Unity运行游戏即可看到登录页面,输入账号密码登录即可

# TODO && Features

- [x] 接入Unity运行时可视化Log查看组件[Debugger](https://github.com/FlameskyDexive/Debugger)
- [x] 接入UGUI框架[EUI](https://github.com/FlameskyDexive/Debugger)
- [x] 接入[YooAsset](https://github.com/tuyoogame/YooAsset)资源管理打包热更框架
- [x] 接入2D物理碰撞引擎[Box2dSharp](https://github.com/Zonciu/Box2DSharp)
- [ ] 摇杆控制角色移动，通过服务端广播位置同步
- [ ] 一个Demo关卡流程：吃食物会变大，碰到敌人会产生伤害（大吞小，小死亡）。
- [ ] 一个比较基本的战斗技能框架设计。


###

## 鸣谢
### [X-ET7, a branch of ET7](https://github.com/IcePower/X-ET7)
