---
name: et-excel
description: ET Excel MCP workflow for reading, writing, styling, transforming, and inspecting xlsx data. Use when operating Excel cells, ranges, sheets, formulas, charts, merges, batch data import/export, or editing Luban source spreadsheets before export.
---

# et-excel - ET Excel 入口

## 强制规则（重要）

- **只要涉及 Excel/xlsx 的任何读写操作，都必须调用 excelmcp（`ET.ExcelMcp`）完成；禁止手写脚本或用其它库/工具直接改 xlsx。**
- excelmcp 源工程位置：`./Packages/cn.etetet.config/DotNet~/ET.ExcelMcp`。
- 运行入口（编译产物）：仓库根目录 `./Bin/ET.ExcelMcp.dll`，统一在仓库根用 `dotnet ./Bin/ET.ExcelMcp.dll ...` 调用。
- **若 `./Bin/ET.ExcelMcp.dll` 不存在（没有可执行文件），先编译工程再操作**：

  ```powershell
  dotnet build "./Packages/cn.etetet.config/DotNet~/ET.ExcelMcp/ET.ExcelMcp.csproj" -c Debug
  ```

  产物会输出到 `./Bin/ET.ExcelMcp.dll`（csproj 的 `OutputPath = $(RepoRoot)Bin`，`RepoRoot` 即仓库根）。

## 何时使用

- 通过 `Bin/ET.ExcelMcp.dll` 读写 Excel
- 处理单元格、区域、样式、公式、图表、工作表
- 做批量数据导入导出、筛选、排序、合并单元格
- 维护 Luban 配置表内容，但还没有进入导出阶段

## 不要加载

- 只是编译、跑测试、做 Unity 编辑器操作
- 只是执行 Luban 导出，不需要实际读写 Excel（用 `et-luban`）
- 只是讨论表结构，不需要实际读写 Excel

## 默认动作

1. 先 `cli list` 或 `cli help <工具名>`，不要硬背完整工具表。
2. 能批量就批量，优先 `excel_range` / `excel_data_operations`，不要逐格操作。
3. JSON 参数只传本次调用最小字段集；`pwsh` 中优先外层单引号。
4. 写操作优先使用绝对路径；覆盖原文件前先确认目标文件。
5. 维护 Luban `__beans__`/`__enums__`/多态数据表时：用 `excel_data_operations batch_write` 只写非空格（空行真空）；写完**必须** `excel_merge_cells merge` 合并"列表"表头（`*fields` `J1:P1`、`*items` `H1:L1`、多态字段表头跨子列宽），否则 Luban 导出报缺 `alias` 列；含中文走 `Write→UTF-8 Python→ExcelMcp`，读回校验合并用 `get_merged`、文本用 `ascii()`。完整格式见 `et-luban`。
6. 改完表如果需要生成配置产物，再叠加 `et-luban`。

## 优先入口

- `dotnet ./Bin/ET.ExcelMcp.dll cli list`
- `dotnet ./Bin/ET.ExcelMcp.dll cli help <工具名>`
- `dotnet ./Bin/ET.ExcelMcp.dll cli <工具名> '<JSON参数>'`

## 按需补读

- `references/et-excel-cli.md`：工具选择、`pwsh` JSON 传参、常用示例
