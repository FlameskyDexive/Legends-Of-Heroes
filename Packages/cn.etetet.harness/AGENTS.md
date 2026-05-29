# cn.etetet.harness

## 概述

AI Harness 技能分发包，包含 ET 项目使用的技能路由索引、轻量入口、详细规则引用，以及本项目的主要 AI 开发规范。

基于 Unity + .NET 的开源游戏框架 ET10（代号“昭君”），采用模块化 Package 架构，专为大型多人在线游戏开发而设计。框架支持客户端服务端双端 C# 开发、热更新、分布式架构和高性能网络通信。

## 使用方式

- 根目录 `AGENTS.md` 只保留最小入口；主要规范以本文件为准。
- 技能索引位于 `skills/index.md`。开始处理具体任务前，先读取该索引并匹配 1 个主 skill；只有跨域任务才叠加其它 skill。
- 测试相关 skill 的轻量路由在 `Packages/cn.etetet.test/AGENTS.md`；harness 命中测试任务后先读取该文件，由 test 包说明继续分流到具体 skill。
- UnityBridge 相关 skill 的轻量路由在 `Packages/cn.etetet.unitybridge/AGENTS.md`；harness 命中 UnityBridge 任务后先读取该文件，由 unitybridge 包说明继续分流到具体 skill。
- 如果项目仍保留根目录 `skills`，请以包内内容为分发源，按需同步到根目录。

## 核心特性

- **统一开发语言**：客户端、服务端均使用 C#。
- **热更新支持**：代码、资源、配置三位一体热更新。
- **模块化架构**：Package 管理，松耦合设计。
- **ECS 框架**：Entity-Component-System 架构。
- **分布式支持**：多进程、多服务器部署。

## 重要提醒

1. **管理员权限**：运行服务器需要管理员权限。
2. **PowerShell 必需**：ET 工具链完全基于 PowerShell。
3. **唯一编译入口**：项目只有一个编译命令，必须使用 `dotnet build ET.sln`。

## 文件结构

```text
WOW/
├── Assets/           # Unity项目资源
├── Packages/         # ET模块化包
├── Bin/              # 编译输出目录
├── Scripts/          # 游戏逻辑代码
├── Book/             # 开发文档和教程
├── Luban/            # Excel配置
├── Proto/            # 消息的proto定义
└── Logs/             # 运行日志
```

## AI 使用规范

- 请使用全中文跟我沟通（代码除外）。
- 每次执行任何操作前，请先说明要做什么，以及为什么要这么做。
- 在本项目中执行的所有命令都必须使用 `pwsh`（PowerShell 7），绝对不要使用 Windows 自带的 `powershell.exe`。
- 分析器编译要使用 `ET.sln`。
- Singleton 类（如 `RobotCaseDispatcher`）可以包含方法，不需要创建 System 类。
- 每次做出决定前先检查是否违反规定，执行完任务后再次检查是否违反规定。

## 技能入口

具体技能路由先读 `skills/index.md`，禁止先完整读取所有 skill 正文。
Codex 规范 skill 均采用 `skills/{skill-name}/SKILL.md` 结构，且 `SKILL.md` 只使用 `name`、`description` frontmatter。

### et-code - 代码编写入口

**使用场景**：

- 新建或修改 Entity、Component、System、Helper。
- 新建或移动 C# 文件，检查 `.meta` 与包内落点。
- 新增消息、模块、包依赖。
- 处理 ECS 分层、组件存在性契约、Module 分析器、分析器报错。

### et-async - Async 异步专家

**使用场景**：

- 新增、修改或 review `async` / `await` / `ETTask` / `ETTask<T>`。
- 判断某段逻辑是否应该异步化。
- `await` 后 Entity 访问、`EntityRef<T>` 安全。
- 设计并发等待、`ETCancellationToken`、`NewContext(...)`。

### et-git - Git 工作流

**使用场景**：

- 准备提交本次改动。
- 检查 `git status` / `git diff`，筛除无关文件。
- 编写中文提交信息。
- 与远端同步时禁止 merge，使用 rebase。

### et-excel - Excel 操作专家

**使用场景**：

- 读取/写入 Excel 文件（xlsx）。
- 单元格操作、批量数据导入导出。
- 设置样式、公式、图表。
- 工作表管理、合并单元格。
- Luban 配置表操作。

### et-luban - Luban 导出专家

**使用场景**：

- 导出 Excel 配置。
- 导出 Luban 生成的 C# 配置代码与 C# 数据代码。
- 修改 `Packages/cn.etetet.*/Luban/**` 下的表、`__tables__.xlsx`、`__beans__.xlsx`、`__enums__.xlsx`、`Defines/` 后重新导出。
- 刷新聚合后的 `luban.conf`。
- 排查 `ET.ExcelExporter` / `LubanGen.ps1` / `luban.conf` 导出失败。
- 核对导出后的 `CodeMode/Model/**` 与 `CodeMode/Config/**`。

### et-eui - 客户端 EUI 专家

**使用场景**：

- 新建或修改 `DlgXxx` 窗口、`ESXxx` 子 UI、`ItemXxx` 循环项。
- 编写 `[AUIEvent]` 处理器、注册 `WindowID`、绑定 `E*` 控件、接 ShowWindow / Hide / Close 链路。
- 排查 UI 打不开、控件 null、`uiTransform is null`、循环列表不刷新。
- 注意：EUI 框架在 `cn.etetet.eui`，无代码生成菜单，按 5 文件模板手写；编辑器验收走 `et-unitybridge`。

### et-build - 编译构建专家

**使用场景**：

- 编译项目（`dotnet build ET.sln`）。
- 导出 Proto 文件。
- 启动服务器。
- 发布版本。
- 资源打包。

### et-test-run - 测试执行专家

**使用场景**：

- 执行测试（全部/指定）。
- 查看测试日志。
- 调试测试失败。
- 详细规则请读取 `Packages/cn.etetet.test/AGENTS.md`。

### et-test-write - 测试编写专家

**使用场景**：

- 编写或修改测试用例。
- 详细规则请读取 `Packages/cn.etetet.test/AGENTS.md`。

### et-tdd - 测试驱动规范

**使用场景**：

- 使用 TDD 测试驱动编写代码。
- 详细规则请读取 `Packages/cn.etetet.test/AGENTS.md`。

### et-unitybridge - UnityBridge 命令调用专家

**使用场景**：

- 查询 UnityBridge 宿主是否在线。
- 实时轮询 Unity 心跳与宿主状态。
- 查询 Unity 编译状态、PlayMode 状态、CodeMode、Unity 版本。
- 执行 UnityBridge 命令（Ping / HostState / Compile / Refresh / RegenProject / EnterPlay / ExitPlay / Reload）。
- 排查 UnityBridge 返回的错误信息。
- 详细规则请读取 `Packages/cn.etetet.unitybridge/AGENTS.md`。

## 包的依赖规范

### 依赖管理原则

1. 依赖的配置在包的 `package.json` 中。
2. 包之间不能相互依赖，只能单向依赖。
3. 包中只能访问自己包或者依赖包的符号。
4. 请注意要递归依赖，修改依赖时要把依赖的依赖全部递归加上去。
5. 通常只能高层包依赖低层包；跨包访问必须显式声明依赖，不存在同层访问例外。
6. 假如 A 包依赖了 B 包，那么 B 包永远不能访问 A 包，这样可以强制处理逻辑相互依赖问题。

### 包层级关系

#### 第5层

```text
├── cn.etetet.wow           (游戏入口)
├── cn.etetet.btnode        (btnode)
├── cn.etetet.test          (测试系统)
├── cn.etetet.robot         (机器人系统)        依赖console
├── cn.etetet.login         (登录系统)
├── cn.etetet.mapplay       (地图玩法系统)      依赖map、spell、item、quest
```

#### 第4层

```text
├── cn.etetet.actorlocation (location消息系统) 依赖netinner
├── cn.etetet.aoi           (数值系统) 依赖unit，numeric
├── cn.etetet.map           (地图基础系统) 依赖aoi、unit、netinner、move、numeric
```

#### 第3层

```text
├── cn.etetet.numeric       (数值系统) 依赖unit
├── cn.etetet.move          (移动系统) 依赖unit
├── cn.etetet.nav2d         (寻路系统) 依赖unit
├── cn.etetet.netinner      (内网消息系统) 依赖startconfig
├── cn.etetet.router        (软路由系统) 依赖startconfig,http
├── cn.etetet.item          (道具背包系统) 依赖unit
```

#### 第2层

```text
├── cn.etetet.unit          (单位系统)
├── cn.etetet.behaviortree  (行为树系统)
├── cn.etetet.http          (http系统)
├── cn.etetet.startconfig   (服务器配置系统)
├── cn.etetet.console       (控制台系统)
├── cn.etetet.yooassets     (资源加载系统)
├── cn.etetet.yiuiframework (yiuiframework)
├── cn.etetet.yiuiinvoke    (yiuiinvoke)
├── cn.etetet.yiuigm        (yiuigm)
├── cn.etetet.yiuiloopscrollrectasync (yiuiloopscrollrectasync)
├── cn.etetet.yiuiyooassets (yiuiyooassets)
├── cn.etetet.yiui          (yiui)
├── cn.etetet.yiuireddot    (yiuireddot)
├── cn.etetet.yiuitips      (yiuitips)
├── cn.etetet.yiuidamagetips(yiuidamagetips)
├── cn.etetet.yiui3ddisplay (yiui3ddisplay)
├── cn.etetet.yiuieffect    (yiuieffect)
```

#### 第1层

```text
├── cn.etetet.core          (核心框架)
├── cn.etetet.excel         (excel)
├── cn.etetet.proto         (协议定义)
├── cn.etetet.loader        (加载器)
```

## 包结构规范

- **所有代码文件**必须创建在 `Packages/cn.etetet.*` 目录下。
- **包命名规范**：`cn.etetet.{功能模块名}`。
- 每个 package 中都有 `packagegit.json` 文件，每个 `packagegit.json` 中的 `Id` 是项目唯一的。
- 每个 package 中都有 `Scripts/Model/Share/PackageType.cs` 文件，里面的编号就是 `packagegit.json` 中的 `Id`。
- 每个包都有 `AGENTS.md` 文件；如果没有请创建。修改前先读取并理解包内规范，可以参考 `cn.etetet.test` 包。

## 核心开发原则

### 绝对禁止事项

- **绝对禁止 hard code**。
- **项目只有一个编译**：`dotnet build ET.sln`，无论什么都用这个编译。
- **每次做出决定前先检查是否违反规定，执行完任务后再次检查是否违反规定**。

### 质量保证要求

严格遵循以上规范，确保：

1. **架构清晰**：Entity 负责数据，System 负责逻辑。
2. **模块化**：功能按包组织，程序集合理分离。
3. **可维护**：代码规范统一，注释详细。
4. **可扩展**：遵循框架设计原则，易于扩展。
5. **高质量**：充分的错误处理和性能优化。
6. **代码一致性**：字段命名统一、Entity 完整性、无重复定义。
7. **EntityRef 安全**：正确管理 Entity 引用，遵循 async/await 规范。
8. **Handler 规范**：`MessageLocationHandler` / `MessageHandler<T>` / `MessageSessionHandler` 的 `Run` 入口无需额外判空实体；如有 `await`，必须在 `await` 后通过 `EntityRef` 重新获取再使用。

## 维护规则

- 修改技能入口或引用文档前，先读取 `skills/index.md`。
- 新增技能时，同步更新 `skills/index.md` 的路由场景。
- 测试相关 skill 路由以 `Packages/cn.etetet.test/AGENTS.md` 为入口；harness 中不要重复维护测试细节。
- UnityBridge 相关 skill 路由以 `Packages/cn.etetet.unitybridge/AGENTS.md` 为入口；harness 中不要重复维护 UnityBridge 细节。
- 本包不承载运行时代码，不应新增业务依赖。
- 不手工生成 `.meta` 或修改 `.csproj`，需要时通过 Unity 刷新生成。
