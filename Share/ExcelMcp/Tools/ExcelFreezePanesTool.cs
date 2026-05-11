using System.Text.Json;
using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 冻结窗格工具。
/// </summary>
public class ExcelFreezePanesTool : IExcelTool
{
    public string Description => @"冻结、取消冻结或查询窗格状态。支持 freeze、unfreeze、get。

使用示例:
- 冻结: excel_freeze_panes(operation='freeze', path='book.xlsx', row=1, column=1)
- 取消冻结: excel_freeze_panes(operation='unfreeze', path='book.xlsx')
- 查询状态: excel_freeze_panes(operation='get', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'freeze': 冻结窗格 (必需: path, row, column)
- 'unfreeze': 取消冻结 (必需: path)
- 'get': 获取冻结状态 (必需: path)",
                @enum = new[] { "freeze", "unfreeze", "get" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (freeze/unfreeze 可选, 默认为输入文件)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 默认0)"
            },
            row = new
            {
                type = "number",
                description = "冻结的行数 (0 表示不冻结行)"
            },
            column = new
            {
                type = "number",
                description = "冻结的列数 (0 表示不冻结列)"
            }
        },
        required = new[] { "operation", "path" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation").ToLowerInvariant();
        var path = ArgumentHelper.GetAndValidatePath(arguments);
        var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);
        var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);

        return operation switch
        {
            "freeze" => await FreezeAsync(path, outputPath, sheetIndex, arguments),
            "unfreeze" => await UnfreezeAsync(path, outputPath, sheetIndex),
            "get" => await GetStatusAsync(path, sheetIndex),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> FreezeAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var row = Math.Max(0, ArgumentHelper.GetInt(arguments, "row"));
            var column = Math.Max(0, ArgumentHelper.GetInt(arguments, "column"));

            if (row == 0 && column == 0)
                throw new ArgumentException("row 和 column 不能同时为 0。");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var topRow = row > 0 ? row + 1 : 1;
            var leftColumn = column > 0 ? column + 1 : 1;
            worksheet.View.FreezePanes(topRow, leftColumn);

            package.SaveAs(new FileInfo(outputPath));
            return $"已冻结前 {row} 行、{column} 列 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> UnfreezeAsync(string path, string outputPath, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            worksheet.View.UnFreezePanes();
            package.SaveAs(new FileInfo(outputPath));
            return $"已取消工作表 {sheetIndex} 的冻结窗格. 输出: {outputPath}";
        });
    }

    private Task<string> GetStatusAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var pane = worksheet.View.PaneSettings;
            var isFrozen = pane.State is ePaneState.Frozen or ePaneState.FrozenSplit;

            int? frozenRows = null;
            int? frozenColumns = null;
            if (isFrozen)
            {
                frozenRows = (int)Math.Round(pane.YSplit);
                frozenColumns = (int)Math.Round(pane.XSplit);
                if (frozenRows == 0) frozenRows = null;
                if (frozenColumns == 0) frozenColumns = null;
            }

            var result = new
            {
                worksheetName = worksheet.Name,
                isFrozen,
                frozenRow = frozenRows,
                frozenColumn = frozenColumns,
                frozenRows,
                frozenColumns,
                status = isFrozen ? "已冻结" : "未冻结"
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }
}
