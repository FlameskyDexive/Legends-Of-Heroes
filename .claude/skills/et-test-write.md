# et-test-write - 测试编写专家

这个skill专门负责编写新的测试用例。

## 使用场景

- 编写新的测试用例
- 测试数据准备
- 配置准备

## 重要提醒

**详细的测试编写规范请查看**：`Packages/cn.etetet.test/README.md`

这个skill只提供快速要点和常见坑点，避免跟README重复。

## 快速要点

### 1. 继承基类

```csharp
public class Test_YourTest_Test: ATestHandler
```

### 2. 命名规范

`{PackageType}_{TestName}_Test`

例如：`Test_CreateRobot_Test`

### 3. 文件位置

`Packages/cn.etetet.{包名}/Scripts/Hotfix/Test/`

### 4. 返回值约定

- 成功：`return ErrorCode.ERR_Success;`
- 失败：`return ErrorCode.ERR_XXX;` 或抛出异常

### 5. 无需添加[Test]特性

父类ATestHandler已有，子类无需重复

## 测试数据准备

### 访问服务端数据

```csharp
// 获取地图Fiber
string mapName = robot.Root.CurrentScene().Name;
Fiber map = fiber.GetFiber("MapManager").GetFiber(mapName);

// 获取服务端Unit
Client.PlayerComponent playerComponent = robot.Root.GetComponent<Client.PlayerComponent>();
Unit serverUnit = map.Root.GetComponent<UnitComponent>().Get(playerComponent.MyId);
```

### 准备配置数据

**不要修改已有的json或excel**，在代码中写json串：

```csharp
string json = @"{
    ""Id"": 1,
    ""Name"": ""TestQuest"",
    ...
}";
QuestConfigCategory config = MongoHelper.FromJson<QuestConfigCategory>(json);
```

## 日志输出规范

1. **使用英文**：这是项目统一要求
2. **Log.Debug**：打印普通日志
3. **Log.Error**：打印错误
4. **Log.Console**：测试输出（需Console=1参数）
5. **不要用Log.Info**：用于重要运营日志

## 结果验证

**必须检查数据是否与预期一致**：

```csharp
// ✅ 正确：验证数据
if (serverUnit == null)
{
    Log.Error($"not found server unit");
    return ErrorCode.ERR_NotFoundUnit;
}

// ✅ 正确：验证状态
if (quest.State != QuestState.Completed)
{
    Log.Error($"quest state error, expected: Completed, actual: {quest.State}");
    return ErrorCode.ERR_QuestStateError;
}

// ❌ 错误：不验证就返回成功
return ErrorCode.ERR_Success;
```

## 常见坑点

### 1. await后Entity访问

```csharp
// ❌ 错误：await后直接使用Entity
await SomeOperation();
task.DoSomething(); // Entity可能已失效

// ✅ 正确：通过EntityRef重新获取
EntityRef<UpdateTask> taskRef = task;
await SomeOperation();
task = taskRef; // 重新获取
task.DoSomething();
```

### 2. 数据清理

**不需要清理**，每个测试用例都是独立环境

### 3. 测试日志

测试成功后，**必须检查All.log**，确认日志符合预期

### 4. 代码复用

**尽量调用已有代码**，不要重复实现逻辑

## 编写完成后

1. 编译项目：使用 `/et-build`
2. 执行测试：使用 `/et-test-run`
3. 检查日志：`cat Logs/All.log`

## 完整示例

参考 `Packages/cn.etetet.test/README.md` 中的示例代码。
