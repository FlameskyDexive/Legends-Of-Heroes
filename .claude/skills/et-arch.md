# et-arch - ET框架架构和规范守护者

这个skill专门负责ET框架的架构规范检查和指导。
<!-- -->
## 使用场景

- 创建新的Entity或Component
- 编写System类
- 检查代码是否符合ECS规范
- EntityRef使用问题
- await后Entity访问问题
- Module分析器相关问题
- 程序集分层问题

## ECS架构核心原则

### 基本概念

- **Entity**: 所有游戏对象的基类，支持对象池优化
- **Component**: 数据组件，纯数据无逻辑
- **System**: 逻辑系统，通过静态扩展方法实现热更新
- **EventSystem**: 事件发布订阅系统，支持异步事件处理

### 架构原则（必须严格遵守）

- **Entity**：只包含数据，不包含方法
- **System**：只包含逻辑，不包含数据
- **Component**：Entity的组成部分，遵循组合优于继承
- **分离原则**：严格分离数据定义和业务逻辑
- **Entity跟System只做数据相关管理，以及自身的一些简单方法**：不要在System中实现复杂的业务逻辑。具体的业务逻辑应该写到XXXHelper中，例如ItemHelper

## 程序集分层结构

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

### 程序集分类规范

每个包必须支持以下四个程序集分类：

#### 1. Model 程序集 (`Scripts/Model/`)

- **用途**：服务器和客户端共享的模型层
- **内容**：Entity定义、配置数据、共享逻辑
- **特点**：不可热更新，稳定性高

#### 2. ModelView 程序集 (`Scripts/ModelView/`)

- **用途**：客户端专用的视图模型层
- **内容**：UI相关Entity、客户端专用组件
- **特点**：不可热更新，UI底层支持

#### 3. Hotfix 程序集 (`Scripts/Hotfix/`)

- **用途**：服务器和客户端共享的热更新逻辑层
- **内容**：System类、业务逻辑实现
- **特点**：可热更新，核心业务逻辑

#### 4. HotfixView 程序集 (`Scripts/HotfixView/`)

- **用途**：客户端专用的热更新视图层
- **内容**：UI System类、客户端显示逻辑
- **特点**：可热更新，UI业务逻辑

## Entity开发规范

### Entity类定义规范

```csharp
// 位置：Unity/Assets/Scripts/Model/ 或 Scripts/ModelView/
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

### Entity类要求（必须严格遵守）

- **必须**继承 `Entity` 基类
- **必须**实现 `IAwake` 接口（生命周期接口）
- **根据需要**实现其他接口：`IDestroy`、`IUpdate`、`ISerialize` 等
- **严禁**在Entity类中定义任何方法
- **必须**添加 `[ComponentOf]` 或 `[ChildOf]` 特性指定父级约束
- **Entity只能管理Entity跟struct，不允许管理非Entity class**

## System开发规范

### System类定义规范

```csharp
// 位置：Unity/Assets/Scripts/Hotfix/ 或 Scripts/HotfixView/
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

### System类要求（必须严格遵守）

- **必须**是静态类（`static`）
- **必须**包含 `partial` 关键字
- **必须**添加 `[EntitySystemOf(typeof(对应Entity类))]` 特性
- **必须**实现对应Entity的 `Awake` 生命周期函数
- **所有方法**必须是静态扩展方法
- **生命周期方法**必须添加 `[EntitySystem]` 特性并声明为 `private static`

## 命名空间规范

- `ET`：通用命名空间，用于共享代码
- `ET.Client`：客户端专用命名空间
- `ET.Server`：服务器专用命名空间

## 消息规范

### 客户端发送消息到服务器

```csharp
// Send不需要等待返回
C2M_TestRobotCase1 message1 = C2M_TestRobotCase1.Create();
fiber.Root.GetComponent<ClientSenderComponent>().Send(message1);

// Call，可以等待返回值
C2M_TestRobotCase2 message2 = C2M_TestRobotCase2.Create();
var response = await fiber.Root.GetComponent<ClientSenderComponent>().Call(message2);
```

### 消息管理

- 消息一般不需要使用对象池，也不需要调用消息的Dispose方法
- 如果要优化，可以让用户自己优化

## 重要分析器规范

### Entity Await 安全规范（重中之重！）

这是ET分析器的硬性限制，违反会导致编译错误！

#### 核心规则

- 在 async/await 环境下，任何 Entity 及其子类对象，await 之后**禁止直接访问** await 前的 Entity 变量
- 必须在 await 前创建 EntityRef，await 后通过 EntityRef 重新获取 Entity
- 只要存在执行路径可能在 await 后访问 Entity，均视为违规
<!-- - 支持 `[SkipAwaitEntityCheck]` 特性标记的方法或类可跳过此检查，但应该避免使用-->

#### 关键要点

- await后，可能Entity已经失效
- 必须在await前创建EntityRef
- await后必须通过EntityRef重新获取Entity才能使用
- 这是ET分析器的硬性限制，违反会导致编译错误
- 函数参数只允许传Entity，不允许传EntityRef

### Entity 成员引用规范

- 任何类/结构体**禁止直接声明 Entity 或其子类类型的字段或属性**，包括集合类型（如 List`<Entity>`、Dictionary<int, Entity>）
- 允许声明 `EntityRef<T>` 类型字段或属性

### EntityRef基本使用方法

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

// ❌ 错误方式
// var entity = processRef.Entity;  // 错误：不要用.Entity
// if (processRef.Entity != null) { /* 使用 */ }  // 错误：多次访问
```

### EntityRef在async/await环境下的使用规范（超级重要！）

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

## 代码质量规范

### 注释规范

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

### 编码风格

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

## 常见错误清单

检查代码时必须避免以下错误：

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
14. ❌ [StaticField]，静态字段容易导致多线程问题，应该尽量避免使用，如果要使用，必须需要用户手动确认

## 工作流程

当用户请求创建Entity或System时：

1. 先检查是否已存在类似的类（避免重复定义）
2. 确认应该放在哪个包、哪个程序集
3. 创建Entity类（Model或ModelView）
4. 创建对应的System类（Hotfix或HotfixView）
5. 检查是否符合所有规范
6. 提示用户需要编译（使用et-build skill）
