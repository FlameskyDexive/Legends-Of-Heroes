using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;

namespace ET.Tools.Excel;

/// <summary>
///     提供 Excel 保护和锁定相关的工具 (工作表/工作簿保护与单元格锁定)。
/// </summary>
public class ExcelProtectTool : IExcelTool
{
    public string Description => @"Excel 保护工具。支持 6 种操作:
- protect_sheet: 启用工作表保护并可配置权限
- unprotect_sheet: 关闭工作表保护
- protect_workbook: 锁定工作簿结构或窗口
- unprotect_workbook: 取消工作簿保护
- lock_cells: 将指定单元格/区域设置为锁定
- unlock_cells: 将指定单元格/区域设置为未锁定";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'protect_sheet': 保护工作表 (必需: path)
- 'unprotect_sheet': 取消工作表保护 (必需: path)
- 'protect_workbook': 保护工作簿结构/窗口 (必需: path)
- 'unprotect_workbook': 取消工作簿保护 (必需: path)
- 'lock_cells': 锁定指定单元格/区域 (必需: path, range/cell/ranges)
- 'unlock_cells': 取消锁定指定单元格/区域 (必需: path, range/cell/ranges)",
                @enum = new[]
                {
                    "protect_sheet", "unprotect_sheet", "protect_workbook",
                    "unprotect_workbook", "lock_cells", "unlock_cells"
                }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (写操作可选, 默认为输入文件)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 针对工作表/单元格操作, 默认0)"
            },
            password = new
            {
                type = "string",
                description = "保护/取消保护时使用的密码 (可选)"
            },
            lockStructure = new
            {
                type = "boolean",
                description = "保护工作簿结构 (protect_workbook 可用, 默认 true)"
            },
            lockWindows = new
            {
                type = "boolean",
                description = "锁定工作簿窗口位置 (protect_workbook 可用, 默认 false)"
            },
            lockRevision = new
            {
                type = "boolean",
                description = "锁定工作簿修订 (protect_workbook 可用, 默认 false)"
            },
            range = new
            {
                type = "string",
                description = "单个单元格或区域 (lock/unlock 可选)"
            },
            cell = new
            {
                type = "string",
                description = "单个单元格地址 (lock/unlock 可选)"
            },
            ranges = new
            {
                type = "array",
                description = "多个单元格或区域 (lock/unlock 可选)",
                items = new { type = "string" }
            },
            allowSelectLockedCells = new
            {
                type = "boolean",
                description = "允许选择被锁定单元格 (protect_sheet 可用)"
            },
            allowSelectUnlockedCells = new
            {
                type = "boolean",
                description = "允许选择未锁定单元格 (protect_sheet 可用)"
            },
            allowSorting = new
            {
                type = "boolean",
                description = "允许排序 (protect_sheet 可用)"
            },
            allowFiltering = new
            {
                type = "boolean",
                description = "允许筛选 (protect_sheet 可用)"
            },
            allowEditingObjects = new
            {
                type = "boolean",
                description = "允许编辑对象 (protect_sheet 可用)"
            },
            allowEditingScenarios = new
            {
                type = "boolean",
                description = "允许编辑方案 (protect_sheet 可用)"
            },
            allowFormattingCells = new
            {
                type = "boolean",
                description = "允许格式化单元格 (protect_sheet 可用)"
            },
            allowFormattingRows = new
            {
                type = "boolean",
                description = "允许格式化行 (protect_sheet 可用)"
            },
            allowFormattingColumns = new
            {
                type = "boolean",
                description = "允许格式化列 (protect_sheet 可用)"
            },
            allowInsertRows = new
            {
                type = "boolean",
                description = "允许插入行 (protect_sheet 可用)"
            },
            allowInsertColumns = new
            {
                type = "boolean",
                description = "允许插入列 (protect_sheet 可用)"
            },
            allowDeleteRows = new
            {
                type = "boolean",
                description = "允许删除行 (protect_sheet 可用)"
            },
            allowDeleteColumns = new
            {
                type = "boolean",
                description = "允许删除列 (protect_sheet 可用)"
            },
            allowInsertHyperlinks = new
            {
                type = "boolean",
                description = "允许插入超链接 (protect_sheet 可用)"
            },
            allowPivotTables = new
            {
                type = "boolean",
                description = "允许使用数据透视表 (protect_sheet 可用)"
            }
        },
        required = new[] { "operation", "path" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation").ToLowerInvariant();
        var path = ArgumentHelper.GetAndValidatePath(arguments);

        return operation switch
        {
            "protect_sheet" => await ProtectSheetAsync(path, arguments),
            "unprotect_sheet" => await UnprotectSheetAsync(path, arguments),
            "protect_workbook" => await ProtectWorkbookAsync(path, arguments),
            "unprotect_workbook" => await UnprotectWorkbookAsync(path, arguments),
            "lock_cells" => await UpdateCellLockAsync(path, arguments, true),
            "unlock_cells" => await UpdateCellLockAsync(path, arguments, false),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> ProtectSheetAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var password = ArgumentHelper.GetStringNullable(arguments, "password");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var protection = worksheet.Protection;

            protection.IsProtected = true;
            if (!string.IsNullOrWhiteSpace(password)) protection.SetPassword(password);

            ApplySheetProtectionOptions(arguments, protection);

            package.SaveAs(new FileInfo(outputPath));
            return $"工作表 {sheetIndex} 已启用保护. 输出: {outputPath}";
        });
    }

    private Task<string> UnprotectSheetAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            worksheet.Protection.IsProtected = false;
            package.SaveAs(new FileInfo(outputPath));

            return $"工作表 {sheetIndex} 的保护已取消. 输出: {outputPath}";
        });
    }

    private Task<string> ProtectWorkbookAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);
            var password = ArgumentHelper.GetStringNullable(arguments, "password");
            var lockStructure = ArgumentHelper.GetBool(arguments, "lockStructure", true);
            var lockWindows = ArgumentHelper.GetBool(arguments, "lockWindows", false);
            var lockRevision = ArgumentHelper.GetBool(arguments, "lockRevision", false);

            using var package = new ExcelPackage(new FileInfo(path));
            var protection = package.Workbook.Protection;

            if (!string.IsNullOrWhiteSpace(password)) protection.SetPassword(password);
            protection.LockStructure = lockStructure;
            protection.LockWindows = lockWindows;
            protection.LockRevision = lockRevision;

            package.SaveAs(new FileInfo(outputPath));
            return $"工作簿保护已启用 (结构={lockStructure}, 窗口={lockWindows}). 输出: {outputPath}";
        });
    }

    private Task<string> UnprotectWorkbookAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var protection = package.Workbook.Protection;
            protection.LockStructure = false;
            protection.LockWindows = false;
            protection.LockRevision = false;

            package.SaveAs(new FileInfo(outputPath));
            return $"工作簿保护已关闭. 输出: {outputPath}";
        });
    }

    private Task<string> UpdateCellLockAsync(string path, JsonObject? arguments, bool locked)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);
            var targets = GetTargetRanges(arguments);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            foreach (var address in targets)
                worksheet.Cells[address].Style.Locked = locked;

            package.SaveAs(new FileInfo(outputPath));
            var action = locked ? "锁定" : "解锁";
            return $"已{action} {targets.Count} 个范围 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private static void ApplySheetProtectionOptions(JsonObject? arguments, ExcelSheetProtection protection)
    {
        void Apply(string key, Action<bool> setter)
        {
            var value = ArgumentHelper.GetBoolNullable(arguments, key);
            if (value.HasValue) setter(value.Value);
        }

        Apply("allowSelectLockedCells", v => protection.AllowSelectLockedCells = v);
        Apply("allowSelectUnlockedCells", v => protection.AllowSelectUnlockedCells = v);
        Apply("allowEditingObjects", v => protection.AllowEditObject = v);
        Apply("allowEditingScenarios", v => protection.AllowEditScenarios = v);
        Apply("allowFormattingCells", v => protection.AllowFormatCells = v);
        Apply("allowFormattingRows", v => protection.AllowFormatRows = v);
        Apply("allowFormattingColumns", v => protection.AllowFormatColumns = v);
        Apply("allowInsertRows", v => protection.AllowInsertRows = v);
        Apply("allowInsertColumns", v => protection.AllowInsertColumns = v);
        Apply("allowInsertHyperlinks", v => protection.AllowInsertHyperlinks = v);
        Apply("allowDeleteRows", v => protection.AllowDeleteRows = v);
        Apply("allowDeleteColumns", v => protection.AllowDeleteColumns = v);
        Apply("allowSorting", v => protection.AllowSort = v);
        Apply("allowFiltering", v => protection.AllowAutoFilter = v);
        Apply("allowPivotTables", v => protection.AllowPivotTables = v);
    }

    private static List<string> GetTargetRanges(JsonObject? arguments)
    {
        var ranges = new List<string>();

        void AddAddress(string? address)
        {
            if (!string.IsNullOrWhiteSpace(address))
                ranges.Add(address.Trim());
        }

        AddAddress(ArgumentHelper.GetStringNullable(arguments, "range"));
        AddAddress(ArgumentHelper.GetStringNullable(arguments, "cell"));

        if (arguments?["ranges"] is JsonArray array)
        {
            SecurityHelper.ValidateArraySize(array, "ranges", 50);
            foreach (var node in array)
            {
                if (node == null) continue;
                AddAddress(node.GetValue<string>());
            }
        }

        if (ranges.Count == 0)
            throw new ArgumentException("必须提供 range、cell 或 ranges 参数以指定目标区域。");

        return ranges;
    }
}
