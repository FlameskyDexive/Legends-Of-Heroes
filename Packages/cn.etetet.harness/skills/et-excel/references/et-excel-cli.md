# ET Excel 参考

## 快速分流

- 工具名不确定：先执行 `list` 或 `help <工具名>`。
- 文件与转换：`excel_file_operations`。
- 单格读写：`excel_cell`。
- 二维区域：`excel_range`。
- 批量数据、筛选、排序：`excel_data_operations`。
- 样式：`excel_style`。
- 公式：`excel_formula`。
- 图表：`excel_chart`。
- 工作表：`excel_sheet`。
- 合并单元格：`excel_merge_cells`。
- 行列处理：`excel_row_column`。

## 构建 / 找不到 dll

- 涉及 Excel 必须走 excelmcp（`ET.ExcelMcp`），统一在仓库根调用 `dotnet ./Bin/ET.ExcelMcp.dll ...`。
- 源工程：`./Packages/cn.etetet.config/DotNet~/ET.ExcelMcp`。
- 若 `./Bin/ET.ExcelMcp.dll` 不存在，先编译再操作（产物落到仓库根 `./Bin`）：

```powershell
dotnet build "./Packages/cn.etetet.config/DotNet~/ET.ExcelMcp/ET.ExcelMcp.csproj" -c Debug
```

## 高频命令

```powershell
dotnet ./Bin/ET.ExcelMcp.dll cli list
dotnet ./Bin/ET.ExcelMcp.dll cli help excel_range
```

```powershell
dotnet ./Bin/ET.ExcelMcp.dll cli excel_sheet '{"operation":"list","path":"C:\\Temp\\demo.xlsx"}'
```

```powershell
dotnet ./Bin/ET.ExcelMcp.dll cli excel_range '{"operation":"write","path":"C:\\Temp\\demo.xlsx","range":"A1:B2","data":[["A","B"],["C","D"]]}'
```

```powershell
dotnet ./Bin/ET.ExcelMcp.dll cli excel_formula '{"operation":"add","path":"C:\\Temp\\demo.xlsx","cell":"C1","formula":"=SUM(A1:B1)"}'
```

## pwsh 传参

正确写法：外层单引号，内部双引号。

```powershell
dotnet ./Bin/ET.ExcelMcp.dll cli excel_range '{"operation":"get","path":"C:\\Temp\\demo.xlsx","range":"A1:B2"}'
```

错误写法：`pwsh` 不把 `\"` 当成通用转义。

```powershell
dotnet ./Bin/ET.ExcelMcp.dll cli excel_range "{\"operation\":\"get\"}"
```

`--%` 停止解析时，JSON 不加外层引号。

```powershell
dotnet ./Bin/ET.ExcelMcp.dll cli --% excel_range {"operation":"get","path":"C:\\Temp\\demo.xlsx","range":"A1:B2"}
```

复杂 JSON 优先用 `ConvertTo-Json -Compress`。

```powershell
$json = @{
    operation = "write"
    path = "C:\Temp\demo.xlsx"
    range = "A1:B2"
    data = @(@("A","B"), @("C","D"))
} | ConvertTo-Json -Compress

dotnet ./Bin/ET.ExcelMcp.dll cli excel_range $json
```

## 中文写入防乱码（重要）

根因：在 Windows PowerShell 里把**原始中文**直接拼进命令字符串 / here-string / JSON，再传给 `ET.ExcelMcp`，中文可能先被控制台代码页污染成 `?`，最终被真的写进 Excel。很多时候不是 `ET.ExcelMcp` 写坏，而是中文进 CLI 之前就被 PowerShell 破坏了。

规避：**不要让 PowerShell 字面量直接承载中文**，改用 `PowerShell → Python → ET.ExcelMcp`，在 Python 内构造 UTF-8 后再传 CLI。

```powershell
@'
import subprocess, json
payload = {
    "operation": "batch_write",
    "path": "Unity/Assets/Config/Excel/Datas/SomeConfig.xlsx",
    "data": [{"cell": "B3", "value": "配置ID"}, {"cell": "C3", "value": "生命值"}]
}
cmd = ['dotnet', r'.\Bin\ET.ExcelMcp.dll', 'cli', 'excel_data_operations',
       json.dumps(payload, ensure_ascii=False)]
res = subprocess.run(cmd, capture_output=True)
print(res.stdout.decode('utf-8', 'ignore'), end='')
print(res.stderr.decode('utf-8', 'ignore'), end='')
raise SystemExit(res.returncode)
'@ | python -
```

环境不可靠时，最稳的做法是先写 ASCII 安全的 `\uXXXX`，再在 Python 里转 Unicode：

```powershell
@'
import subprocess, json
u = lambda s: s.encode('ascii').decode('unicode_escape')
payload = {"operation": "batch_write",
           "path": "Unity/Assets/Config/Excel/Datas/SomeConfig.xlsx",
           "data": [{"cell": "B3", "value": u('\\u914d\\u7f6eID')}]}
cmd = ['dotnet', r'.\Bin\ET.ExcelMcp.dll', 'cli', 'excel_data_operations',
       json.dumps(payload, ensure_ascii=False)]
res = subprocess.run(cmd, capture_output=True)
print(res.stdout.decode('utf-8', 'ignore'), end=''); print(res.stderr.decode('utf-8', 'ignore'), end='')
raise SystemExit(res.returncode)
'@ | python -
```

写完中文必须二次验证（**不要只信终端输出**），二选一：
- 用 `ET.ExcelMcp` 再读回内容确认结构与值。
- 直接检查 `xlsx` 内的 `xl/sharedStrings.xml`：看到 `生命值` 这类是正确中文，看到 `????` 说明文件已写坏需重写。

```powershell
@'
import zipfile, re
path = 'Unity/Assets/Config/Excel/Datas/SomeConfig.xlsx'
with zipfile.ZipFile(path) as zf:
    data = zf.read('xl/sharedStrings.xml').decode('utf-8', 'ignore')
    print([ascii(x) for x in re.findall(r'<t[^>]*>(.*?)</t>', data)[-20:]])
'@ | python -
```

## 常见检查

- 工具名不确定时，先看 `help`。
- 写中文优先 `PowerShell → Python → ET.ExcelMcp`，写完做读回或 `sharedStrings.xml` 校验。
- 能批量处理时，不要逐格写入。
- Windows 路径在 JSON 字符串内反斜杠需要双写。
- 如果问题其实是导出、编译或运行链路，转 `et-luban` 或 `et-build`。
