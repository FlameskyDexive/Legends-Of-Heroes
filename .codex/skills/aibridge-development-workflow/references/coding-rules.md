# C# 与 Unity 编码规则

## C# 版本

代码必须兼容 C# 9.0。禁止使用 C# 10.0+ 语法，例如：

- 文件范围命名空间：`namespace Foo;`
- 全局 using：`global using System;`
- 原始字符串字面量：`"""..."""`
- `required` 成员
- 主构造函数：`class Foo(int value)`
- 常量插值字符串：`const string s = $"{nameof(Foo)}";`

可使用兼容写法：

```csharp
const string PropertyName = nameof(Foo) + "." + nameof(Bar);
```

## Unity 对象判空

Unity 对象必须显式判空：

```csharp
if (target != null)
{
    target.SetActive(true);
}
```

禁止对 Unity 对象使用空条件访问：

```csharp
target?.SetActive(true);
```

## 硬编码

- 业务配置、资源路径、命令路径、魔法数字不得散落硬编码。
- 稳定常量应定义为 `const` 或 `static readonly`。
- UI 文案、日志文本、测试样例可按上下文保留字面量。
- 不要为了消除一个局部字面量而制造过度抽象。

## 重复代码

- 同一业务规则或复杂逻辑重复出现时，提取公共方法或工具类。
- 仅形状相似但语义不同的代码，不强行抽象。
- 优先复用项目已有 Helper、Command、Installer、Patch 工具。

## 注释

- 复杂逻辑、兼容性处理、Unity 序列化限制处用简体中文添加必要注释。
- 不写解释显而易见赋值或流程的空泛注释。
