# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

基于Unity + .NET的开源游戏框架ET8.0，专为大型多人在线游戏开发而设计。框架支持客户端服务端双端C#开发、热更新、分布式架构和高性能网络通信。

### 核心特性

- **统一开发语言**：客户端、服务端均使用C#
- **热更新支持**：代码、资源、配置三位一体热更新
- **ECS框架**：Entity-Component-System架构
- **分布式支持**：多进程、多服务器部署

## 开发环境配置

### 必需工具版本

- **Unity版本**: Unity 6000.3.4（其他版本报错请自行处理）
- **.NET版本**: .NET 8（必须）
- **PowerShell**: 必须安装，ET工具链基于PowerShell

### 重要提醒

1. **必须翻墙**：开发过程需要全局翻墙访问GitHub Package和NuGet
2. **管理员权限**：运行服务器需要管理员权限
3. **版本严格**：新手必须使用指定Unity版本
4. **Clone新工程**：必须clone全新工程，不要使用已有工程
5. **PowerShell必需**：ET工具链完全基于PowerShell

### 文件结构

```
Legends-Of-Heroes/
├── Bin/             # 编译输出目录
├── Book/            # 开发文档和教程
├── Config/            # 开发文档和教程
├── Document/        # 策划文档、截图等
├── Dotnet/          # 服务端代码目录
├── Share/           # 公共代码库
├── Tools/           # 工具库
├── Unity            # Unity工程目录
    ├── Assets/           # Unity项目资源
    ├── Packages/         # ET模块化包
    ├── Scripts/         # 游戏逻辑代码
    ├── Luban/           # Excel配置
    ├── Proto/           # 消息的proto定义
    └── Logs/            # 运行日志
```

## Claude AI 使用规范

### 命令执行规范

**重要：Claude在此项目中执行的所有命令都必须使用PowerShell**

- 原因：ET框架工具链完全基于PowerShell
- 方式：使用 `pwsh -Command "具体命令"` 格式
- 兼容性：确保与项目构建脚本和工具链一致

### 信息记录规则

当用户说"请记住"时，将信息记录在此CLAUDE.md文件中。

### 日志输出规范

1. 打印的日志请使用英文，这是项目统一要求
2. 一般不允许使用Log.Info，Log.Info用于输出重要运营日志
3. 打印普通日志使用Log.Debug，打印错误使用Log.Error
4. 测试用例输出到Console使用Log.Console，进程启动参数需要Console=1

### 其他规范

- 分析器编译要使用ET.sln
- Singleton类（如RobotCaseDispatcher）可以包含方法，不需要创建System类

## 开发工作流程

### 标准开发流程

```
修改代码 → dotnet build ET.sln → 重启进程 → 测试
```

### 常用开发命令

#### 编译相关

```powershell
# 编译整个解决方案（需翻墙下载NuGet包）
dotnet build ET.sln

# 发布Linux版本
pwsh -ExecutionPolicy Bypass -File Scripts/Publish.ps1

# Unity内编译（快捷键F6）
# 注意：Model和Hotfix程序集不能用IDE编译，必须用Unity编译
```

#### 热更新相关

```powershell
# Unity菜单操作（无直接命令行）
# ET -> Reload 或按F7进行热重载
# HybridCLR -> Generate -> All 生成热更新相关文件
# ET -> HybridCLR -> CopyAotDlls 复制AOT DLL
```

#### 资源和配置

```powershell
# 导出Excel配置
# Unity菜单: ET -> Excel -> ExcelExport

# 导出ScriptableObject配置
# Unity菜单: ET -> WOW -> ExportScriptableObject

# YooAsset资源打包
# 通过YooAsset -> AssetBundle Builder窗口操作
```

#### 服务器相关

```powershell
# 独立启动服务器（需管理员权限）
# Unity菜单: ET -> Server Tools -> Start Server(Single Process)

# 手动启动服务器
dotnet Bin/ET.App.dll --Console=1
# 注意：运行目录是Unity根目录，不是Bin目录
```

### 重要开发工具

#### Excel导出工具

- 命令：`dotnet Bin/ET.ExcelExporter.dll`
- 功能：导出Excel配置为Luban格式
- 处理内容：Luban配置、游戏数据表、启动配置等
- 特性：支持实时输出和Ctrl+C正常终止

#### Proto导出工具

- 命令：`dotnet Bin/ET.Proto2CS.dll`
- 功能：导出proto文件为C#文件
- 处理内容：Protocol Buffers消息定义转换为C#类
- 用途：网络通信协议、数据序列化结构
- proto生成文件会生成在proto包中
- proto文件名带的编号是唯一的，是100的倍数

### 调试技巧

- 开启 `ENABLE_VIEW`宏可在Unity Hierarchy中查看所有Entity
- 使用Unity Profiler监控性能
- 查看 `Logs/`目录获取详细日志信息，运行前记得删除Logs目录
- 服务端支持REPL模式进行动态调试

## ET框架核心架构

### 程序集分层结构

```
ET.HotfixView (UI热更新层)
    ↓
ET.Hotfix (逻辑热更新层)
    ↓
ET.ModelView (UI模型层)
    ↓
ET.Model (游戏逻辑模型层)
    ↓
ET.Loader (加载器和启动层)
    ↓
ET.Core (框架核心层)
```

### ECS架构核心

- **Entity**: 所有游戏对象的基类，支持对象池优化
- **Component**: 数据组件，纯数据无逻辑
- **System**: 逻辑系统，通过静态扩展方法实现热更新
- **EventSystem**: 事件发布订阅系统，支持异步事件处理


### 热更新机制

- **代码热更**: 基于HybridCLR，支持C#代码运行时更新
- **资源热更**: 基于YooAsset，支持资源增量更新
- **配置热更**: 基于Luban配置系统，支持配置实时更新

## ET框架开发规范

### Entity-Component-System (ECS) 架构原则

- **Entity**：只包含数据，不包含方法
- **System**：只包含逻辑，不包含数据
- **Component**：Entity的组成部分，遵循组合优于继承
- **分离原则**：严格分离数据定义和业务逻辑

### 程序集分类规范

业务逻辑代码按以下四个程序集分类：

##### 1. Model 程序集 (`Assets/Scripts/Model/`)

- **用途**：服务器和客户端共享的模型层
- **内容**：Entity定义、配置数据、共享逻辑
- **特点**：不可热更新，稳定性高

##### 2. ModelView 程序集 (`Assets/Scripts/ModelView/`)

- **用途**：客户端专用的视图模型层
- **内容**：UI相关Entity、客户端专用组件
- **特点**：不可热更新，UI底层支持

##### 3. Hotfix 程序集 (`Assets/Scripts/Hotfix/`)

- **用途**：服务器和客户端共享的热更新逻辑层
- **内容**：System类、业务逻辑实现
- **特点**：可热更新，核心业务逻辑

##### 4. HotfixView 程序集 (`Assets/Scripts/HotfixView/`)

- **用途**：客户端专用的热更新视图层
- **内容**：UI System类、客户端显示逻辑
- **特点**：可热更新，UI业务逻辑

### Entity 开发规范

#### Entity 类定义规范

```csharp
// 位置：Assets/Scripts/Model/ 或 Assets/Scripts/ModelView/
namespace ET  // 或 ET.Client, ET.Server
{
    /// <summary>
    /// 详细的中文描述
    /// </summary>
    [ComponentOf(typeof(ParentEntityType))]  // 指定父实体类型（如适用）
    public class ExampleComponent : Entity, IAwake, IDestroy
    {
        // 只包含数据字段，不包含方法
        public int SomeValue;
        public string SomeName;
        public List<int> SomeList;
        // 注意：Entity只能管理Entity跟struct，不允许管理非Entity class
    }
}
```

#### Entity 类要求

- **必须**继承 `Entity` 基类
- **必须**实现 `IAwake` 接口（生命周期接口）
- **根据需要**实现其他接口：`IDestroy`、`IUpdate`、`ISerialize` 等
- **严禁**在Entity类中定义任何方法
- **必须**添加 `[ComponentOf]` 或 `[ChildOf]` 特性指定父级约束
- **Entity只能管理Entity跟struct，不允许管理非Entity class**

#### System 类定义规范

```csharp
// 位置：Assets/Scripts/Hotfix/ 或 Assets/Scripts/HotfixView/
namespace ET  // 或 ET.Client, ET.Server
{
    /// <summary>
    /// 详细的中文描述
    /// </summary>
    [EntitySystemOf(typeof(ExampleComponent))]     // 指定对应的Entity类型
    public static partial class ExampleComponentSystem
    {
        #region 生命周期方法

        [EntitySystem]
        private static void Awake(this ExampleComponent self)
        {
            // 初始化逻辑
        }

        [EntitySystem]
        private static void Destroy(this ExampleComponent self)
        {
            // 销毁清理逻辑
        }

        #endregion

        #region 业务方法

        /// <summary>
        /// 业务方法的中文描述
        /// </summary>
        public static void DoSomething(this ExampleComponent self, int param)
        {
            // 业务逻辑实现
        }

        #endregion
    }
}
```

#### System 类要求

- **必须**是静态类（`static`）
- **必须**包含 `partial` 关键字
- **必须**添加 `[EntitySystemOf(typeof(对应Entity类))]` 特性
- **必须**实现对应Entity的 `Awake` 生命周期函数
- **所有方法**必须是静态扩展方法
- **生命周期方法**必须添加 `[EntitySystem]` 特性并声明为 `private static`

### 命名空间规范

- `ET`：通用命名空间，用于共享代码
- `ET.Client`：客户端专用命名空间
- `ET.Server`：服务器专用命名空间

### 消息规范

#### 客户端发送消息到服务器

```csharp
// Send不需要等待返回
C2M_TestRobotCase1 message1 = C2M_TestRobotCase1.Create();
fiber.Root.GetComponent<ClientSenderComponent>().Send(message1);

// Call，可以等待返回值
C2M_TestRobotCase2 message2 = C2M_TestRobotCase2.Create();
var response = await fiber.Root.GetComponent<ClientSenderComponent>().Call(message2);
```

#### 消息管理

- 消息一般不需要使用对象池，也不需要调用消息的Dispose方法
- 如果要优化，可以让用户自己优化

### 代码质量规范

#### 注释规范

```csharp
/// <summary>
/// 类的详细中文描述
/// 说明功能、用途和注意事项
/// </summary>
public class ExampleComponent : Entity, IAwake
{
    /// <summary>
    /// 字段的中文描述
    /// </summary>
    public int Value;
}

/// <summary>
/// 方法的详细中文描述
/// </summary>
/// <param name="self">当前组件实例</param>
/// <param name="value">参数的中文描述</param>
/// <returns>返回值的中文描述</returns>
public static bool DoSomething(this ExampleComponent self, int value)
{
    // 重要逻辑的中文注释
    return true;
}
```

#### 编码风格

```csharp
// 命名规范
public class PlayerComponent        // 类名：PascalCase
public static void GetItem()       // 方法名：PascalCase  
public string playerName;          // 字段名：camelCase
private string internalField;     // 私有字段：camelCase
public const int MAX_PLAYERS = 100; // 常量：UPPER_SNAKE_CASE

// 代码格式
if (condition) 
{   // 大括号不能省
    // tab缩进
}
```

## 重要分析器规范

### Entity Await 安全规范

- 在 async/await 环境下，任何 Entity 及其子类对象，await 之后**禁止直接访问** await 前的 Entity 变量
- 必须在 await 前创建 EntityRef，await 后通过 EntityRef 重新获取 Entity
- 只要存在执行路径可能在 await 后访问 Entity，均视为违规
<!-- - 支持 `[SkipAwaitEntityCheck]` 特性标记的方法或类可跳过此检查，但应该避免使用 -->

**关键要点：**

- await后，可能Entity已经失效
- 必须在await前创建EntityRef
- await后必须通过EntityRef重新获取Entity才能使用
- 这是ET分析器的硬性限制，违反会导致编译错误
- 函数参数只允许传Entity，不允许传EntityRef

### Entity 成员引用规范

- 任何类/结构体**禁止直接声明 Entity 或其子类类型的字段或属性**，包括集合类型（如 List`<Entity>`、Dictionary<int, Entity>）
- 允许声明 `EntityRef<T>` 类型字段或属性

#### EntityRef基本使用方法

```csharp
// Entity字段中使用EntityRef
public Dictionary<int, EntityRef<ProcessInfo>> ProcessDict { get; set; }

// 创建EntityRef引用
EntityRef<ProcessInfo> processRef = processInfo;

// 正确的Entity对象访问和检查方式
ProcessInfo entity = processRef;  // 直接赋值，不用.Entity
if (entity != null)
{
    // 安全使用Entity
}

// 错误方式
// var entity = processRef.Entity;  // 错误：不要用.Entity
// if (processRef.Entity != null) { /* 使用 */ }  // 错误：多次访问
```

#### EntityRef在async/await环境下的使用规范（重要！）

**这是ET分析器的严格限制，必须遵循：**

```csharp
// ✅ 正确：await后使用Entity需要通过EntityRef重新获取
public static async ETTask ProcessUpdate(this UpdateCoordinatorComponent self, UpdateTask task)
{
    // 1. 在await前创建EntityRef引用
    EntityRef<UpdateCoordinatorComponent> selfRef = self;
    EntityRef<UpdateTask> taskRef = task;
  
    foreach (int processId in task.TargetProcessIds)
    {
        // 2. 在每次使用前通过EntityRef重新获取Entity
        task = taskRef;
        task.UpdateProgress(processId, "开始处理");
      
        // 3. await后需要重新获取所有Entity
        await SomeAsyncOperation();
      
        // 4. await后必须重新获取才能安全使用
        self = selfRef;
        task = taskRef;
      
        // 现在可以安全使用Entity
        task.UpdateProgress(processId, "处理完成");
    }
}
```

### Module分析器规范

- Model, ModelView, Hotfix, HotfixView中的类可以指定模块
- Module(ModuleName.A)，这个加在类上，表示是属于A模块
- A模块调用B模块的方法，那么B模块就不能调用A模块的方法
- A模块不能访问B模块的字段
- ModuleName定义是partial，每个Package可以定义自己的Module
- 如果没有Module标签，那么该类属于Global模块，该类可以被其它Module调用，也可以调用其它Module

## 包的依赖规范

### 依赖管理原则

1. 依赖的配置在包的package.json中
2. 包之间不能相互依赖，只能单向依赖
3. 包中只能访问自己包或者依赖包的符号
4. 请注意要递归依赖，修改依赖时要把依赖的依赖全部递归加上去
5. 通常只能高层包依赖低层包，但如果A包的packagegit.json中加了"AllowSameLevelAccess": true，那么允许没有被A包依赖的同层包访问
6. 假如A包依赖了B包，那么B包永远不能访问A包，这样可以强制处理逻辑相互依赖问题

## 机器人测试系统

### 机器人测试流程

1. 启动测试进程: `dotnet ./Bin/ET.App.dll --Process=1 --SceneName=RobotTest --StartConfig=Localhost --Console=1 --SingleThread=1`
2. 进程会输出 `>`等待输入
3. 测试所有用例: 输入 `Case --Id=0` //--Id=0指执行所有用例
4. 测试指定用例X: 输入 `Case --Id=X` //X是RobotCaseType的成员变量

### 完整的命令行执行方式

#### 单个测试用例执行

```bash
printf "Case --Id=0\n" | pwsh -Command "dotnet ./Bin/ET.App.dll --Process=1 --SceneName=RobotTest --StartConfig=Localhost --Console=1 --SingleThread=1"
```

#### 单进程多用例连续执行（推荐）

```bash
printf "Case --Id=1\nCase --Id=2\n" | pwsh -Command "dotnet ./Bin/ET.App.dll --Process=1 --SceneName=RobotTest --StartConfig=Localhost --Console=1 --SingleThread=1"
```

#### 执行流程说明

- 启动测试进程，显示 `>` 等待输入
- 依次执行多个测试用例，无需重启进程
- 成功输出：`case run success: X` (X为用例ID)
- 失败输出：`case run failed: X` 加详细错误信息
- 检测到EOF时程序正常退出

#### 优势

- 一次启动多次测试，提高效率
- 保持进程状态，减少启动开销
- 支持连续测试不同用例
- 实时查看每个用例的执行结果

### 机器人测试用例编写流程

1. **继承基类**: ARobotCaseHandler是测试用例的父类，继承它来写一个用例，用例名参考RobotCase_001_CreateRobot_Handler
2. **环境准备**: 每个用例运行前都创建了服务端环境，用例使用真实的消息与服务端交互
3. **测试数据准备**: 直接访问服务端Fiber，直接修改数据， 参考RobotCase_001_CreateRobot_Handler，例如:

```csharp
    // 也可以直接访问服务器的数据，直接设置数据
    string mapName = robot.Root.CurrentScene().Name;
  
    Fiber map = fiber.GetFiber("MapManager").GetFiber(mapName);
    if (map == null)
    {
        Log.Error($"not found robot map {mapName}");
    }
  
    // 获取Unit的Id
    Client.PlayerComponent playerComponent = robot.Root.GetComponent<Client.PlayerComponent>();
  
    // 获取服务端Unit
    Unit serverUnit = map.Root.GetComponent<UnitComponent>().Get(playerComponent.MyId);
```

4. **配置准备**: 如果需要准备配置文件，请自己在代码中写json串，然后使用MongoHelper.FromJson来反序列化成配置数据，不要修改已有的json或者excel：

```csharp
QuestConfigCategory config = MongoHelper.FromJson<QuestConfigCategory>(json); // json写在代码中
```

1. **日志管理**: 日志目录是Logs，测试前请删除，方便查找问题，All.log是整个用例的日志
2. **代码复用**: 写用例时应该尽量调用客户端跟服务端已有的代码
3. **结果验证**: 测试用例要检查返回的数据或者客户端的数据是否与预期一致，如果与预期不一致需要抛出异常
4. **日志检查**: 测试跑成功后，应该去检查All.log，看日志是否与预期一致
5. **结果判断**: 看到输出case run success则表示用例成功，看到case run fail则表示失败，可以立即结束用例进程
6. **数据清理**: 测试用例的数据不需要清理，每个测试用例都是独立的环境

## 常见错误避免

### 开发错误清单

1. ❌ Entity中定义方法
2. ❌ System忘记加特性
3. ❌ 生命周期方法不是private static
4. ❌ 忘记实现IAwake接口
5. ❌ 消息类字段使用camelCase（应该用PascalCase）
6. ❌ Entity字段不完整（缺少System中使用的字段）
7. ❌ 重复定义相同的类（检查是否已存在）
8. ❌ HTTP消息类字段名与使用处不匹配
9. ❌ 直接存储Entity引用（应该用EntityRef）
10. ❌ 使用EntityRef.Entity属性访问（应该直接赋值）
11. ❌ 不检查Entity的IsDisposed状态
12. ❌ 将EntityRef当作Entity直接使用
13. ❌ await后直接使用Entity（违反ET分析器规则）
14. ❌ [StaticField]，静态字段容易导致多线程问题，应该尽量避免使用，如果要使用，必须需要我手动确认

## 核心开发原则

### 绝对禁止事项

- **绝对禁止hard code**
- **项目只有一个编译**: `dotnet build ET.sln` 无论什么都用这个编译
- **每次做出决定前先检查是否违反规定，执行完任务后再次检查是否违反规定**

### 质量保证要求

严格遵循以上规范，确保：

1. **架构清晰**：Entity负责数据，System负责逻辑
2. **模块化**：功能按包组织，程序集合理分离
3. **可维护**：代码规范统一，注释详细
4. **可扩展**：遵循框架设计原则，易于扩展
5. **高质量**：充分的错误处理和性能优化
6. **代码一致性**：字段命名统一、Entity完整性、无重复定义
7. **EntityRef安全**：正确管理Entity引用，遵循async/await规范

这些规范是ET框架高效开发的基础，请严格遵循执行。
