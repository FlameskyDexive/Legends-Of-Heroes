# AGENTS.md

读完这个文件，请输出确认是否做到了以下步骤:
1.是否了解Skills目录，是否读取  
2.是否了解Skills的使用场景，并且在相应的场景加载对应的Skills  

## 重要: Skill使用指南

1. 重要: 请阅读一下./Agents/skills目录，所有skills都放在这里
2. 重要: 在遇到下面描写的使用场景的时候，请自动加载对应的skills

## 项目概述

基于Unity + .NET的开源游戏框架ET8.0，专为大型多人在线游戏开发而设计。框架支持客户端服务端双端C#开发、热更新、分布式架构和高性能网络通信。

### 核心特性

- **统一开发语言**：客户端、服务端均使用C#
- **热更新支持**：代码、资源、配置三位一体热更新
- **ECS框架**：Entity-Component-System架构
- **分布式支持**：多进程、多服务器部署

## 开发环境配置

### 必需工具版本

- **Unity版本**: Unity 6000.3.34
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

## Skill使用指南

项目已将复杂的规范拆分成专门的skill，按需调用即可。

### et-arch - 架构和规范守护者

**使用场景**：
- 创建新的Entity或Component
- 编写System类
- 检查代码是否符合ECS规范
- EntityRef使用问题
- await后Entity访问问题
- Module分析器相关问题

**调用方式**：
```
/et-arch
```

### et-build - 编译构建专家

**使用场景**：
- 编译项目（dotnet build ET.sln）
- 导出Excel配置
- 导出Proto文件
- 启动服务器
- 发布版本
- 资源打包

**调用方式**：
```
/et-build
```

### et-test-run - 测试执行专家（高频）

**使用场景**：
- 执行测试（全部/指定）
- 查看测试日志
- 调试测试失败

**调用方式**：
```
/et-test-run
```

### et-test-write - 测试编写专家（低频）

**使用场景**：
- 编写新的测试用例
- 测试数据准备
- 配置准备

**调用方式**：
```
/et-test-write
```

**注意**：详细编写规范请查看 `Packages/cn.etetet.test/README.md`

## Claude AI 使用规范

### 命令执行规范

**重要：Claude在此项目中执行的所有命令都必须使用PowerShell**

- 原因：ET框架工具链完全基于PowerShell
- 方式：使用 `pwsh -Command "具体命令"` 格式
- 兼容性：确保与项目构建脚本和工具链一致

### 其他规范

- 分析器编译要使用ET.sln
- Singleton类（如RobotCaseDispatcher）可以包含方法，不需要创建System类

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
