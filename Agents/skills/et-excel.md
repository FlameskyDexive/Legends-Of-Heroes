# et-excel - Excel 配置编辑规范

这个 skill 用于在本项目里通过 `ET.ExcelMcp` 安全地读写 Excel 配置表。

适用场景：
- 新增/修改配置表数据
- 修复表格中的中文乱码
- 批量更新单元格、表头、注释、描述
- 新建配置表并接入 `__tables__.xlsx`

---

## 核心原则

1. 涉及 Excel 配置修改时，优先使用 `ET.ExcelMcp`
2. 写入中文时，必须避免让 PowerShell 直接承载原始中文参数
3. 修改配置表后，要按需导出受影响的配置数据/代码
4. 不能只看终端输出判断是否乱码，必要时要检查 `xlsx` 原始内容
5. 资源类配置字段统一只填写资源名字，不填写完整路径和文件后缀；如果表里出现 `Assets/.../xxx.prefab` 这类完整路径，修改时应优先收敛为 `xxx`

---

## 这次乱码问题的根因

在 Windows PowerShell 环境里，如果把中文直接写进命令字符串、here-string 或 JSON 字符串，再传给 `ET.ExcelMcp`，中文可能先被控制台代码页污染，变成 `?`，最终被真实写进 Excel。

也就是说：
- 有时候不是 `ET.ExcelMcp` 写坏了
- 而是中文在进入 `ET.ExcelMcp` 之前，就已经被 PowerShell 破坏了

典型错误做法：
- 在 `powershell.exe -Command "...中文..."` 里直接拼中文 JSON
- 在 PowerShell here-string 里直接写中文，再传给 CLI
- 只根据控制台打印结果判断 Excel 是否已经写对

---

## 正确写法

### 方案一：推荐方案

用 Python 包一层调用 `ET.ExcelMcp`，并在 Python 内部构造 UTF-8 字符串后再传给 CLI。

推荐模式：

```powershell
@'
import subprocess, json
payload = {
    "operation": "batch_write",
    "path": "Unity/Assets/Config/Excel/Datas/SomeConfig.xlsx",
    "data": [
        {"cell": "B3", "value": "配置ID"},
        {"cell": "C3", "value": "生命值"}
    ]
}
cmd = [
    'dotnet', r'.\Bin\ET.ExcelMcp.dll',
    'cli', 'excel_data_operations',
    json.dumps(payload, ensure_ascii=False)
]
res = subprocess.run(cmd, capture_output=True)
print(res.stdout.decode('utf-8', 'ignore'), end='')
print(res.stderr.decode('utf-8', 'ignore'), end='')
raise SystemExit(res.returncode)
'@ | python -
```

### 方案二：最稳妥的中文写法

如果当前 shell/终端环境不可靠，不要把中文直接写进脚本字面量。
可以先写 ASCII 安全的 `\uXXXX`，再在 Python 里转成真正的 Unicode：

```powershell
@'
import subprocess, json
u = lambda s: s.encode('ascii').decode('unicode_escape')

payload = {
    "operation": "batch_write",
    "path": "Unity/Assets/Config/Excel/Datas/SomeConfig.xlsx",
    "data": [
        {"cell": "B3", "value": u('\\u914d\\u7f6eID')},
        {"cell": "C3", "value": u('\\u751f\\u547d\\u503c')}
    ]
}

cmd = [
    'dotnet', r'.\Bin\ET.ExcelMcp.dll',
    'cli', 'excel_data_operations',
    json.dumps(payload, ensure_ascii=False)
]
res = subprocess.run(cmd, capture_output=True)
print(res.stdout.decode('utf-8', 'ignore'), end='')
print(res.stderr.decode('utf-8', 'ignore'), end='')
raise SystemExit(res.returncode)
'@ | python -
```

这个方案在当前项目环境里是最可靠的中文修复方案。

---

## 工具选型建议

### 1. 零散改单元格

优先用：
- `excel_cell`
- `excel_data_operations` 的 `batch_write`

适合：
- 修正表头
- 修正文案
- 修正少量配置值

### 2. 整块重写规则区域

优先用：
- `excel_range` 的 `write`

适合：
- 新建一整张配置表
- 重写规则头（`##var / ##type / ##`）
- 保证 used range 正确扩到新列

### 3. 新增配置表

必须同时处理：
- 新建 `xxx.xlsx`
- 在 `__tables__.xlsx` 注册 `Category / value_type / input / index / group`
- 导出受影响的服务端/客户端配置

---

## 中文修复后的验证规范

修改含中文的表格后，至少做下面两步之一：

### 验证方式 A：再次用 `ET.ExcelMcp` 读回内容

适合快速确认结构和值。

### 验证方式 B：检查 `xlsx` 原始 `sharedStrings.xml`

这是判断“有没有真的写成问号”的最可靠方法。

示例：

```powershell
@'
import zipfile, re
path = 'Unity/Assets/Config/Excel/Datas/SomeConfig.xlsx'
with zipfile.ZipFile(path, 'r') as zf:
    data = zf.read('xl/sharedStrings.xml').decode('utf-8', 'ignore')
    items = re.findall(r'<t[^>]*>(.*?)</t>', data)
    print([ascii(x) for x in items[-20:]])
'@ | python -
```

判断标准：
- 如果看到 `\u751f\u547d\u503c` 这类转义，说明底层是正确中文
- 如果看到 `????`，说明文件里真的已经坏了，需要重新写入

---

## 配置变更后的导出要求

修改 Excel 配置后，不要只改表，不导配置。

至少导出受影响的配置：
- `Config/Excel/s/GameConfig`
- `Config/Json/s/GameConfig`
- 如果客户端也依赖，再导出 `c/cs`
- 如果新增字段/新表影响生成代码，还要重新生成 `Unity/Assets/Scripts/Model/Generate/...`

可优先做“定向导出”，避免全量导出带来的超时。

---

## 本项目里的执行约束

1. 命令必须通过 PowerShell 发起
2. 但中文内容不要直接依赖 PowerShell 字面量传递
3. 优先采用：`PowerShell -> Python -> ET.ExcelMcp`
4. 写中文后必须做读回或原始 XML 验证
5. 新增配置表时，必须同步注册 `__tables__.xlsx`

---

## 推荐工作流

1. 先确认要改的是哪张表、哪个范围
2. 通过 `ET.ExcelMcp` 读取现状
3. 用 Python 包装 `ET.ExcelMcp` 写入
4. 对中文内容做二次验证
5. 导出受影响的配置代码/数据
6. 运行 `dotnet build ET.sln` 验证代码侧是否正常

---

## 结论

以后在这个项目里，只要是“Excel 表格 + 中文写入”：

- 不要把中文直接塞进 PowerShell 命令字符串
- 一律优先用 `Python -> ET.ExcelMcp` 的方式写入
- 必要时用 `\uXXXX -> unicode_escape` 转换
- 写完后检查 `sharedStrings.xml`，不要只信终端输出

这条规范是为了避免再次出现中文被写成 `?` 的问题。
