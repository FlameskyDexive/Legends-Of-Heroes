# cn.etetet.ballbattle

球球大作战 PvP 玩法包。基于 **statesync/mapplay 状态同步**(服务端权威 + AOI 广播),2D 碰撞用 `cn.etetet.box2dsharp`(**不用** 3D 的 `cn.etetet.collision`)。Level 7,依赖 box2dsharp / mapplay / move / skill / numeric / unit。

## 核心机制与同步思路
- **服务端权威**:位置/血量/体型/吞噬/分裂全在服务端裁决。客户端发意图(`C2M_Operation`),服务端改 `NumericComponent` → 自动经 `M2C_NumericChange` 广播给 AOI 视野内客户端。
- **质量=HP**:`Radius = RadiusCoef*sqrt(HP)`(面积∝HP)、`Speed` 随 HP 增大而降。见 `BallNumericHelper.RecalcFromHp`。任何改 HP 处(吞噬/扣血/分裂/吃食物)调一次即自动重算并广播 Radius/Speed。
- **体型表现**:客户端 `NumericWatcher_Radius_ScaleBall` 监听 Radius 变化缩放视图 GameObject;服务端 `NumericWatcher_Radius_UpdateCollision` 同步 Box2D 碰撞圆半径。

## 已实现(Phase A–D 核心)
- **Box2D 碰撞包装**(`Scripts/Model/Share/Collision` + `Hotfix/Share/Collision`):
  - `CollisionWorldComponent`(挂 Scene,持 Box2D `World`,`IUpdate` 每帧 `Step`)
  - `CollisionComponent`(挂 Unit,持 `Body`,`IUpdate` 同步 Unit→Body 位置;`AddCollider`/`SetBodyCircleRadius`)
  - `CollisionListenerComponent`(挂 Scene,实现 `IContactListener.BeginContact` → 发 `OnCollisionContact` 事件)
  - `CollisionHelper`(Circle/Box fixture + World 工厂)
  - 坐标:Unit 的 **X/Z** 映射到 Box2D 的 X/Y。
  - 原 Ori 用 `IFixedUpdate`/`DefineCore.FixedDeltaTime`/`MathHelper.Angle`/Bson,本项目无 → 改 `IUpdate` + `BallDefine.FixedDeltaTime` + `math.atan2` + 去 Bson。
- **数值公式** `BallNumericHelper`(HP→Radius/Speed)+ 两个 Radius `NumericWatcher`。
- **球类型** `BallComponent`(`EBallType` Player/Food/Bullet)。
- **碰撞裁决** `OnCollisionContactHandler`(`[Event(SceneType.Map)]`):子弹命中扣血变小/死亡移除、玩家吃食物、玩家间大吃小(比半径)。HP/质量变化直接在本包处理(skill 的 `BattleHelper.HitSettle` 当前是只算不写的桩,不依赖它应用伤害)。

## 已实现(Phase D 装配 + 食物)
- **`BallArenaComponent`**(挂 Scene)+ `BallArenaComponentSystem`(ET.Server):Awake 时给场景装配 `CollisionListenerComponent` + `CollisionWorldComponent`(Listener 先于 World),并启动每秒食物刷新定时器(`TimerInvokeType.BallFoodSpawn`)。Destroy 时停定时器。
- **食物刷新** `SpawnFood`:补足场上食物到 `MaxFoodCount`,随机位置 `UnitFactory.Create(FoodConfigId)` + 设 HP + `SetupBall(Food)`。`FoodConfigId==0` 时跳过(未配置则不刷,安全)。
- **`BallHelper.SetupBall(unit, EBallType)`**(Hotfix/Share):给 Unit 加 `BallComponent` + `CollisionComponent` + 圆形 sensor 碰撞体 + 初始 `RecalcFromHp`。玩家球/食物球/子弹球通用。

## 场景架构:专属地图(SceneType.Map)+ 组件隔离
ball-battle **不新建 SceneType**(mapplay 整套基建 AOI/广播/移动都硬标 `SceneType.Map`,且场景类型精确匹配,新 SceneType 会让基建全失效)。改用:**一张专属地图(MapConfig 名 = `BallDefine.BallMapName` "BallBattle",仍是 SceneType.Map)**,复用全部 Map 基建;隔离靠组件——只有该地图挂 `BallArenaComponent`、其单位带 `BallComponent`,普通 Map 玩法零影响。

## 已实现(接线)
- **通用扩展点**:mapplay 服务端 `UnitFactory.Create` 末尾发布 `AfterUnitCreateServer{Unit}`(`Model/Share/ServerEventType.cs`,通用、不引用高层包)。
- **接线订阅者** `AfterUnitCreateServer_SetupBall`(ballbattle,`[Event(SceneType.Map)]`):单位在 `BallMapName` 地图创建时 → 懒装配 `BallArenaComponent`(碰撞世界+食物刷新)+ 对玩家 Unit `SetupBall(Player)`。食物球由 Arena 刷新器自行 `SetupBall(Food)`。

## 地图配置(已加)
- `cn.etetet.map/Luban/Config/Datas/Map.xlsx` 已加 **Id=5 BallBattle**(CopyType=Copy, MaxPlayer=20, MapResName=BallBattle),已导表进 `MapConfigCategory`。
- 占位 navmesh:`cn.etetet.map/Bundles/Recast/BallBattle.bytes`(复制自 Map1)。因 `FiberInit_Map` 对非 GateMap 地图强制 `NavmeshComponent.Load(mapName)`(缺数据会抛异常),而球游戏用直线移动不需要真实 navmesh,故放一份占位让进场不崩。后续若要真实地形碰撞再换。

## 进图路由(已打通)
关键发现:整套 room/copy/transfer/匹配骨干**已按地图名路由**,MapManager 已能按 `CopyType=Copy` 动态建/复用 Copy 场景 fiber 并按 `RecommendPlayerNum` 池化玩家。把玩家钉在 "Map2" 的只是 `C2G_EnterMapHandler` 的硬编码。所以接入只需让客户端指定地图名:
- **proto**:`C2G_EnterMap` 加 `string MapName`(login,已手改 3 端生成码)。
- **客户端**:`EnterMapHelper.EnterMapAsync(root, mapName="Map2")` 透传地图名。
- **Gate**:`C2G_EnterMapHandler` 用 `request.MapName`(空=Map2),UnitConfigId 仍 1001——进 BallBattle 的玩家用普通 1001 单位,`SetupBall(Player)` 把它变成球(无需 login 知道 1101)。
- **匹配入口**:`DlgMatchTeam`(已有假倒计时匹配)匹配完成 → `EnterMapAsync(root, "BallBattle")`。
- **MapConfig Id=5**:CopyType=Copy, RecommendPlayerNum=20(=单房间人数,池满才开新 fiber), MapResName=Map1(复用现有客户端场景资源)。

链路:Match 按钮 → 假匹配 → `EnterMapAsync("BallBattle")` → Gate → TransferHelper → MapManager 建/复用 `BallBattle@{id}` Copy → 玩家 transfer 进 → `AfterUnitCreateServer_SetupBall` 装配 Arena+玩家球 → 玩法运行。

## 待实现 / 注意
- **机器人对手**:`DlgMatchTeam` 匹配时还调 `RequestMatchRobotAsync`(机器人走自己进图流程,默认进 Map2),**不会进 BallBattle Copy**。要真人/机器人 PvP,机器人进图也需指向 BallBattle(后续)。当前单人可玩(吃食物长大)。
- **客户端球预制体 / BallBattle 场景视觉**:MapResName 复用 Map1 场景,球单位用 1001 预制体渲染;真实球形视觉是美术 gating。
- **速度移动客户端表现 / 子弹/分裂技能**(需 skill 配置导表)。
- **速度式移动**(Phase E):给 `MoveComponent` 加方向速度模式,改 `mapplay/C2M_OperationHandler` 的 Move 分支为设方向(替代 `FindPathMoveToAsync` 寻路)。
- **子弹技能**:`skill` 的 ActionEvent 发射子弹(补 `UnitFactory.CreateBullet`,子弹 = `SetupBall(Bullet)` 朝向飞行)。
- **分裂技能**:`ActionEventSplit`(HP/2 + `UnitFactory.Create` 第二个球冲刺)。
- **死亡/重生**:✅ 已实现(`OnCollisionContactHandler` 裁决 → `BallPlayerDie` 事件 → `BallDeathHelper.RespawnBall` 缩小+关碰撞+延迟随机点重生)。
- **结算/计分 + 实时排行榜**:✅ 已实现(服务端权威 + 客户端 EUI)。
  - 计分:`BallComponent` 加 `Kills/Deaths`(玩家)+ `ShooterId`(子弹归属);`BallScoreHelper.CreditKill/CreditBulletKill` 在吞噬/子弹击杀处记功(子弹经 ShooterId 反查射手)。
  - 广播:`BallArenaComponentSystem.BroadcastLeaderboard` 每 5s(`TimerInvokeType.BallLeaderboard`)取 Top5(`BallScoreHelper.GatherTopPlayers/GetKills`)→ `MapMessageHelper.NoticeClient(unit, M2C_BallLeaderboard, NoticeType.Self)` 逐个发给场景内真人(`UnitType.Player` 且有 `UnitGateInfoComponent`;机器人 Virtual 自动跳过)。
  - proto:`M2C_BallLeaderboard{repeated BallRankInfo}` + `BallRankInfo{UnitId,Hp,Kills}`(opcode 11502)。**注意 proto 文件放在 `cn.etetet.mapplay/Proto/BallBattle_C_11500.proto`,不放 ballbattle**——ballbattle 依赖 box2dsharp(Unity 原生库,不在 MainPackage.txt/独立 .NET 服务端构建),若把 ballbattle 加进 MainPackage.txt 会让它被拉进 DotNet~ 服务端构建并报 box2dsharp 找不到。proto 生成类是全局的(`cn.etetet.proto`),放哪个主包的 Proto/ 都能用。
  - 客户端 UI:`cn.etetet.statesync` 的 `DlgBallLeaderboard`(EUI 弹窗,PopUp 层,`WindowID_BallLeaderboard=1002`):标题 + 单个多行内容 Text(Top5) + 关闭按钮;`M2C_BallLeaderboardHandler`([MessageHandler(SceneType.Current)],HotfixView/Client)收到广播自动开窗(`ShowWindow<DlgBallLeaderboard>` if !visible)+ `RefreshRank`。预制体 `Assets/GameRes/EUI/Dlg/DlgBallLeaderboard.prefab`(UnityBridge 拼,字体 MinCartoon.ttf)。
  - **待验证(美术/PlayMode)**:实机进图看弹窗位置/中文字形/刷新是否正常;yooassets 是否按名收录该预制体。

## 配置(已加,需重新导表)
- 已在 `cn.etetet.map/Luban/Config/Datas/Unit.xlsx`(UnitProto 表)加两条球的 `UnitConfig`:
  - **1101 BallPlayer**(UnitType=Player):SpeedBase=6000, RadiusBase=500, MaxHPBase/HP=1000 —— 玩家球。进图后对玩家 Unit 调 `SetupBall(EBallType.Player)`(初始体型由 `RecalcFromHp` 按 HP 重算)。
  - **1102 BallFood**(UnitType=Virtual):SpeedBase=0, RadiusBase=200, MaxHPBase/HP=50 —— 食物球。`BallArenaComponent.FoodConfigId` 默认已指向 1102。
- **已导表**:已跑 `dotnet ./Bin/ET.ExcelExporter.dll` 重新导出,1101/1102 已生成进 `cn.etetet.config/CodeMode/Config/{Client,Server,ClientServer}/UnitConfigCategoryFactory_Config.cs`,运行时可取到。
  - 注:导出工具是 export-all,会重新生成所有 config 产物(Item/Map/Quest 等的 diff 是聚合刷新,非手改)。
  - 顺手修了 Luban 脚本的跨平台 bug:`LubanGen.ps1` 用 `if ($null -ne $IsMacOS)` 判 Mac(PS7 下恒为真,Windows 误选 mac 的 dotnet 路径),已改为 `if ($IsMacOS)` + 跨平台 `dotnet`(config + 4 个 startconfig 共 5 个脚本)。

## 已实现(配置驱动)
- **技能(吐球/分裂)= skill 多态配置**:✅ skill 包 `SkillConfig`(扁平,ActionEventIds 列表)+ `ActionEventConfig`(多态:`ActionEventType` 枚举 + `Params` 多态 bean `ActionEventParams_BallSpit{CostHp,BulletHp,Range}`/`_BallSplit{MinHp,Range}`)。ballbattle 的 `ActionEventBallSpit/Split` 从 `actionEvent.ActionEventConfig.Params as 子类` 读 typed 参数 → 有参 `BallSkillHelper.SpitBall/SplitBall`。`EActionEventType` 已改由 Luban `__enums__` 生成。详见 `cn.etetet.skill` 与 [[ballbattle-progress]]。
- **玩法调参 = `BallConfig` 单例表(Id=1)**:✅ `ballbattle/Luban/Config/{Base/__tables__,Datas/BallConfig}.xlsx`(已加 luban.conf、导出 `BallConfigCategory`)。迁了体型速度公式(RadiusCoef/BaseSpeed/SpeedCoef/MinSpeed)、EatRatio、BulletDamage、复活(RespawnHp/Delay)、食物(MaxFoodCount/FoodHp)、地图边界(MapMin/Max)、机器人(RobotCount/InitHp/ThinkMs/Lookahead/FleeRange/SpitRange/SpitChance)。`BallDefine` 这些字段已 `const→静态属性`读 `BallConfigCategory.Instance?.Get(1)`(缺表回退默认),所有 `BallDefine.Xxx` 调用点不变;`BallArenaComponentSystem.Awake` 从配置填 food/map 字段。结构性常量(BallMapName/Box2D 步进/配置 id/MinHp)仍 `const`。
- **改 Luban 多态/枚举表要点**:列表头必须**合并单元格**(`*fields` J1:P1、`*items` H1:L1、多态字段表头跨子列宽),否则导出报缺列;新增生成 .cs 需 UnityBridge `Refresh`+`RegenProject` 才进 csproj。详见 `et-luban`/`et-excel` skill。

## Gating(仍需补)
- **客户端球预制体**:1101/1102 的 UnitConfig 用 head_icon=Portrait 占位,客户端 `AfterUnitCreate_CreateUnitView` 需要对应可加载的球预制体(球形 + 可被 Radius watcher 缩放)。
- **实机验证**:技能/调参均编译绿色,待真机跑确认从配置读值生效(数值与原 BallDefine 常量一致,行为应不变)。
