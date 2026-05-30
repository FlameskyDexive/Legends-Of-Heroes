# ET Luban 参考

## 默认原则

- 命令必须在项目根目录执行。
- 本项目 Luban 导出入口：`dotnet ./Bin/ET.ExcelExporter.dll`。
- 当前脚本统一走 `cs-code` 与 `cs-code-data`，产物是 C# 代码。
- 只改表内容时先用 `et-excel`；真正生成产物时再用 `et-luban`。

## 导出入口

```powershell
dotnet ./Bin/ET.ExcelExporter.dll
```

Unity 菜单入口：

- `ET/Excel/ExcelExporter`

## 导出流程

1. 确认本次任务是否修改了 `Packages/cn.etetet.*/Luban/**`、`__tables__.xlsx`、`__beans__.xlsx`、`__enums__.xlsx` 或 `Defines/`。
2. 在项目根目录执行 `dotnet ./Bin/ET.ExcelExporter.dll`。
3. 检查控制台是否出现导出开始信息和 `excelexporter ok!`。
4. 用 `git diff --stat` 检查生成范围。
5. 核对 `CodeMode/Model/**`、`CodeMode/Config/**`、聚合后的 `luban.conf` 是否符合预期。
6. 如果导出后还要编译、启动、发布，再叠加 `et-build`。

## 常见落点

- 表定义：`Packages/cn.etetet.*/Luban/**`
- 聚合配置：`luban.conf`
- 代码产物：`Packages/cn.etetet.*/CodeMode/Model/**`
- 配置产物：`Packages/cn.etetet.*/CodeMode/Config/**`

## 常见问题

- 把产物理解成 json/bin：当前脚本实际导出 C# 代码产物。
- `[ERROR] 源文件 luban.conf 不存在`：对应配置集合缺少基准 `luban.conf`，需要先补齐或初始化。
- 导出后 diff 很大：先确认是不是 `schemaFiles` 聚合刷新，不要误判成手工逻辑改动。
- 只想维护表内容：不要直接导出，先用 `et-excel`。

## 多态 bean / 枚举表 authoring（关键：列表头必须合并单元格）

改 / 建 `__beans__.xlsx`、`__enums__.xlsx` 或含多态字段的数据表时，**"列表"的表头单元格必须横向合并、跨整个子列宽**，否则 Luban 只识别第一子列、丢掉 `alias`/`type` 等子列，导出报：

```
bean:'__intern__.__FieldInfo__' 缺失 列:'alias'，请检查是否写错或者遗漏
```

源码依据：`Packages/cn.etetet.yiuiluban/DontNet~/luban/src/Luban.DataLoader.Builtin/Excel/SheetLoadUtil.cs` 的 `ParseSubTitles`——`*` 前缀字段在非合并时只取单格 `FromIndex==ToIndex`，唯有合并单元格才把 `ToIndex` 撑到末列。**这是手工/脚本写 Luban 多态表最易踩、且只读单元格值发现不了的坑。**

必须合并的三处：

- `__beans__.xlsx`：第 1 行的 `*fields` 合并 `J1:P1`（覆盖字段子列 name/alias/type/group/comment/tags/variants）。
- `__enums__.xlsx`：第 1 行的 `*items` 合并 `H1:L1`（覆盖枚举项子列 name/alias/value/comment/tags）。
- 数据表的多态字段（如 `Params`）：表头四行 `##var` / `##type` / `##group` / `##` **各自**合并，跨“1 个类型名列 + N 个最宽子类字段列”（quest 范例 `G1:J1`～`G4:J4`，即 1 类型名 + 3 字段）。

校对合并：`dotnet ./Bin/ET.ExcelMcp.dll cli excel_merge_cells '{"operation":"get_merged","path":"...","sheetIndex":0}'`（只读单元格值看不到合并，必须查 merged）。

布局规则（镜像 quest `QuestObjectiveParams` 范例）：

- `__beans__`：3 行表头（`##var` 列名行 / `##var` 子列名行 / `##` 中文行）。base 行只填 `full_name`(B)、不填字段；子类行 B=子类全名、C(parent)=基类全名、字段从 J 列起；**多字段子类 B/C 仅写在首字段行，后续字段行只填 J 起的字段列**；不同 bean 之间用**真正的空行**分隔（用 `batch_write` 只写非空格，别用二维数组写满空字符串）。
- `__enums__`：每个枚举一个 sheet（sheet 名 = 枚举短名），3 行表头；数据区第一行 B=`ET.XxxType`、C(flags)=`FALSE`（只写一次），其后每个枚举项一行，H=项名、I=别名、J=值。
- 数据表多态字段：`##type` 行填基类短名（如 `ActionEventParams`）；数据格该列填子类全名（带 `ET.`，如 `ET.ActionEventParams_BallSpit`），紧邻右侧若干列按子类字段顺序**横向占列**填值（无独立表头、不用分隔符）。枚举字段：`##type` 填枚举短名，数据格填枚举项名（字符串，非数字）。

`luban.conf`：`LubanGen.ps1` 只**消费** conf、不自动聚合；新增 `__beans__`/`__enums__` 要在 `schemaFiles` 手动加 `bean`/`enum` 条目（紧跟同包 `table` 条目后）。

写表只用 `ET.ExcelMcp`（**绝不用 openpyxl 从零写或复制别包模板**——列对不齐 + 别包 bean 泄漏导致 `duplicate` 连带整个 config 导出失败）；含中文走 `Write 落 UTF-8 Python 脚本 → python 调 ET.ExcelMcp`（PowerShell 管道会把中文写成乱码）；读回文本用 `ascii()` 避开 cp1252 stdout。

## 导出后：新增生成 .cs 需 Unity 刷新（否则 CS0234）

Luban 新生成的 `.cs`（如新枚举、新 bean 子类）没有 `.meta`，而 `ET.Model.csproj` / `ET.Hotfix.csproj` 等是 Unity 生成的**显式 `<Compile Include>` 列表（非通配）**，新文件不在列表里 → `dotnet build ET.sln` 报 `CS0234 类型或命名空间不存在`（即便文件就在同目录）。修法：先用 UnityBridge 刷新再编译——

```powershell
dotnet ./Bin/ET.UnityBridge.dll '{"_t":"Refresh"}'      # 导入新资源、生成 .meta
dotnet ./Bin/ET.UnityBridge.dll '{"_t":"RegenProject"}' # 重生 sln/csproj（CLI 可能报 timeout，但 Unity 实际已完成，核对 .meta 已生成、csproj 已纳入即可）
dotnet build ET.sln -t:Rebuild
```
