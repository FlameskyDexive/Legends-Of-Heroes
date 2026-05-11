using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 行列分组工具。
/// </summary>
public class ExcelGroupTool : IExcelTool
{
    public string Description => @"管理 Excel 行列分组。支持 group_rows, ungroup_rows, group_columns, ungroup_columns。

使用示例:
- 分组行: excel_group(operation='group_rows', path='book.xlsx', startRow=2, endRow=5, isCollapsed=true)
- 取消行分组: excel_group(operation='ungroup_rows', path='book.xlsx', startRow=2, endRow=5)
- 分组列: excel_group(operation='group_columns', path='book.xlsx', startColumn=2, endColumn=4)
- 取消列分组: excel_group(operation='ungroup_columns', path='book.xlsx', startColumn=2, endColumn=4)";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'group_rows': 行分组 (必需: path, startRow, endRow)
- 'ungroup_rows': 取消行分组 (必需: path, startRow, endRow)
- 'group_columns': 列分组 (必需: path, startColumn, endColumn)
- 'ungroup_columns': 取消列分组 (必需: path, startColumn, endColumn)",
                @enum = new[] { "group_rows", "ungroup_rows", "group_columns", "ungroup_columns" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 默认为输入文件)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 默认0)"
            },
            startRow = new
            {
                type = "number",
                description = "起始行 (从1开始, 行操作必需)"
            },
            endRow = new
            {
                type = "number",
                description = "结束行 (从1开始, 行操作必需)"
            },
            startColumn = new
            {
                type = "number",
                description = "起始列 (从1开始, 列操作必需)"
            },
            endColumn = new
            {
                type = "number",
                description = "结束列 (从1开始, 列操作必需)"
            },
            isCollapsed = new
            {
                type = "boolean",
                description = "是否初始折叠 (group_rows/group_columns 可选, 默认 false)"
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
            "group_rows" => await GroupRowsAsync(path, outputPath, sheetIndex, arguments),
            "ungroup_rows" => await UngroupRowsAsync(path, outputPath, sheetIndex, arguments),
            "group_columns" => await GroupColumnsAsync(path, outputPath, sheetIndex, arguments),
            "ungroup_columns" => await UngroupColumnsAsync(path, outputPath, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> GroupRowsAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var (start, end) = GetRange(arguments, "startRow", "endRow");
            var collapsed = ArgumentHelper.GetBool(arguments, "isCollapsed", false);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            for (var row = start; row <= end; row++)
            {
                var excelRow = worksheet.Row(row);
                excelRow.OutlineLevel = (byte)Math.Min(8, excelRow.OutlineLevel + 1);
                excelRow.Collapsed = collapsed;
                if (collapsed) excelRow.Hidden = true;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已在工作表 {sheetIndex} 分组行 {start}-{end}. 输出: {outputPath}";
        });
    }

    private Task<string> UngroupRowsAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var (start, end) = GetRange(arguments, "startRow", "endRow");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            for (var row = start; row <= end; row++)
            {
                var excelRow = worksheet.Row(row);
                if (excelRow.OutlineLevel > 0)
                    excelRow.OutlineLevel--;
                excelRow.Collapsed = false;
                excelRow.Hidden = false;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已在工作表 {sheetIndex} 取消行 {start}-{end} 的分组. 输出: {outputPath}";
        });
    }

    private Task<string> GroupColumnsAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var (start, end) = GetRange(arguments, "startColumn", "endColumn");
            var collapsed = ArgumentHelper.GetBool(arguments, "isCollapsed", false);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            for (var column = start; column <= end; column++)
            {
                var excelColumn = worksheet.Column(column);
                excelColumn.OutlineLevel = (byte)Math.Min(8, excelColumn.OutlineLevel + 1);
                excelColumn.Collapsed = collapsed;
                if (collapsed) excelColumn.Hidden = true;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已在工作表 {sheetIndex} 分组列 {start}-{end}. 输出: {outputPath}";
        });
    }

    private Task<string> UngroupColumnsAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var (start, end) = GetRange(arguments, "startColumn", "endColumn");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            for (var column = start; column <= end; column++)
            {
                var excelColumn = worksheet.Column(column);
                if (excelColumn.OutlineLevel > 0)
                    excelColumn.OutlineLevel--;
                excelColumn.Collapsed = false;
                excelColumn.Hidden = false;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已在工作表 {sheetIndex} 取消列 {start}-{end} 的分组. 输出: {outputPath}";
        });
    }

    private static (int start, int end) GetRange(JsonObject? arguments, string startKey, string endKey)
    {
        var start = ArgumentHelper.GetInt(arguments, startKey);
        var end = ArgumentHelper.GetInt(arguments, endKey);

        if (start < 1) throw new ArgumentException($"{startKey} 必须 >= 1");
        if (end < start) throw new ArgumentException($"{startKey} 不能大于 {endKey}");

        return (start, end);
    }
}
