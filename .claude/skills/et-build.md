# et-build - ET框架编译构建专家

这个skill专门负责ET框架的编译、构建、导出、打包等开发工作流。

## 使用场景

- 编译项目
- 导出Excel配置
- 导出Proto文件
- 启动服务器
- 发布版本
- 资源打包
- 热更新文件生成

## 核心开发流程

### 标准开发流程

```
修改代码 → dotnet build ET.sln → 重启进程 → 测试
```

**重要**：项目只有一个编译命令：`dotnet build ET.sln`，无论什么都用这个编译！

## 命令执行规范

**重要：Claude在此项目中执行的所有命令都必须使用PowerShell**

- 原因：ET框架工具链完全基于PowerShell
- 方式：使用 `pwsh -Command "具体命令"` 格式
- 兼容性：确保与项目构建脚本和工具链一致

## 编译相关

### 编译整个解决方案

```powershell
dotnet build ET.sln
```

**注意事项**：
- 需要全局翻墙下载NuGet包
- 分析器编译也要使用ET.sln
- Model和Hotfix程序集不能用IDE编译，必须用Unity编译（F6）

### 发布Linux版本

```powershell
pwsh -ExecutionPolicy Bypass -File Scripts/Publish.ps1
```

## Excel配置导出

### 命令行方式

```powershell
dotnet Bin/ET.ExcelExporter.dll
```

**功能说明**：
- 导出Excel配置为Luban格式
- 处理内容：Luban配置、游戏数据表、启动配置等
- 特性：支持实时输出和Ctrl+C正常终止

## Proto文件导出

### 命令行方式

```powershell
dotnet Bin/ET.Proto2CS.dll
```

**功能说明**：
- 导出proto文件为C#文件
- 处理内容：Protocol Buffers消息定义转换为C#类
- 用途：网络通信协议、数据序列化结构
- proto生成文件会生成在proto包中
- proto文件名带的编号是唯一的，是100的倍数


## 服务器启动

### 命令行启动

```powershell
dotnet Bin/ET.App.dll --Console=1
```

**注意事项**：
- 运行目录是Unity根目录，不是Bin目录
- 需要管理员权限

### 日志查看

- 查看 `Logs/`目录获取详细日志信息
- **重要**：运行前记得删除Logs目录

## 常见问题

### 编译错误

1. 检查是否使用了正确的编译命令（dotnet build ET.sln）

### 启动失败

1. 检查是否有管理员权限
2. 检查运行目录是否正确
3. 检查日志文件

## 工作流程

当用户需要编译或构建时：

1. 确认修改了哪些内容
2. 选择合适的编译/导出命令
3. 执行命令
4. 检查输出是否有错误
5. 如果有错误，提供解决方案
