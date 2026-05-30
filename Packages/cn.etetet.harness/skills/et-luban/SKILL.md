---
name: et-luban
description: ET Luban export workflow. Use when exporting generated C# config/data code, refreshing aggregated luban.conf, validating CodeMode outputs, or troubleshooting ET.ExcelExporter, LubanGen.ps1, table definitions, beans, enums, Defines, and Luban export failures.
---

# et-luban - Luban 导出入口

## 何时使用

- 导出 Luban 生成的 C# 配置代码与 C# 数据代码
- 修改 `Packages/cn.etetet.*/Luban/**`、`__tables__.xlsx`、`__beans__.xlsx`、`__enums__.xlsx`、`Defines/` 后重新导出
- 刷新聚合后的 `luban.conf`
- 排查 `ET.ExcelExporter`、`LubanGen.ps1`、`luban.conf` 导出失败
- 核对导出后的 `CodeMode/Model/**` 与 `CodeMode/Config/**`

## 不要加载

- 只是读取/写入 Excel 单元格、批量改表，还没到导出环节（用 `et-excel`）
- 只是编译 `ET.sln`、导出 Proto、启动服务器、发布（用 `et-build`）
- 只是查询 Unity 编辑器状态或执行其它 UnityBridge 命令

## 默认动作

1. 命令必须在项目根目录执行，避免 `Packages/` 相对路径和 `LubanGen.ps1` 定位错误。
2. 当前 Luban 脚本统一走 `cs-code` 与 `cs-code-data`，导出结果是 C# 代码，不是 json 或二进制。
3. 导出前先区分“改表”和“生成产物”；只改表时先用 `et-excel`。
4. 导出过程会扫描 `Packages/cn.etetet.*/Luban/*` 并刷新聚合 `luban.conf`。
5. 导出后检查控制台、生成结果与 diff，区分真实配置变化和自动聚合变化。
6. 改/建 `__beans__`/`__enums__`/含多态字段的数据表时，"列表"表头（`*fields`/`*items`/多态字段）**必须横向合并单元格**，否则导出报 `缺失 列:'alias'`；详见 references。
7. 导出若**新增**了生成 `.cs`（新枚举/新 bean 子类），必须先 UnityBridge `Refresh`+`RegenProject` 再编译，否则 `dotnet build` 报 `CS0234`（新文件无 `.meta`、不在显式 `.csproj` 列表）。

## 优先入口

- `dotnet ./Bin/ET.ExcelExporter.dll`
- Unity 菜单：`ET/Excel/ExcelExporter`

## 按需补读

- `references/et-luban-export.md`：导出流程、结果落点、常见失败排查、**多态 bean/枚举表 authoring（合并单元格）与导出后 Unity 刷新**
