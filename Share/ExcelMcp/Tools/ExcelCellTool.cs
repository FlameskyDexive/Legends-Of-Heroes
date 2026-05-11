using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     Unified tool for managing Excel cells (write, edit, get, clear)
/// </summary>
public class ExcelCellTool : IExcelTool
{
    private static readonly Regex CellAddressRegex = new(@"^[A-Za-z]{1,3}\d+$", RegexOptions.Compiled);

    public string Description => @"管理 Excel 单元格。支持 4 种操作: write, edit, get, clear。

使用示例:
- 写入单元格: excel_cell(operation='write', path='book.xlsx', cell='A1', value='Hello')
- 编辑单元格: excel_cell(operation='edit', path='book.xlsx', cell='A1', value='Updated')
- 读取单元格: excel_cell(operation='get', path='book.xlsx', cell='A1')
- 清空单元格: excel_cell(operation='clear', path='book.xlsx', cell='A1')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'write': 写入单元格值 (必需参数: path, cell, value)
- 'edit': 编辑单元格值 (必需参数: path, cell, value)
- 'get': 读取单元格值 (必需参数: path, cell)
- 'clear': 清空单元格 (必需参数: path, cell)",
                @enum = new[] { "write", "edit", "get", "clear" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 可选, 默认: 0)"
            },
            cell = new
            {
                type = "string",
                description = "单元格引用 (例如: 'A1', 'B2', 'AA100')"
            },
            value = new
            {
                type = "string",
                description = "要写入的值 (write和edit操作必需). 支持字符串、数字、布尔值和日期格式"
            },
            formula = new
            {
                type = "string",
                description = "要设置的公式 (可选, 用于edit, 会覆盖value)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 用于write/edit/clear操作, 默认为输入路径)"
            }
        },
        required = new[] { "operation", "path", "cell" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation");
        var path = ArgumentHelper.GetAndValidatePath(arguments);
        var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);
        var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);

        return operation.ToLower() switch
        {
            "write" => await WriteCellAsync(path, outputPath, sheetIndex, arguments),
            "edit" => await EditCellAsync(path, outputPath, sheetIndex, arguments),
            "get" => await GetCellAsync(path, sheetIndex, arguments),
            "clear" => await ClearCellAsync(path, outputPath, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private static void ValidateCellAddress(string cell)
    {
        if (!CellAddressRegex.IsMatch(cell))
            throw new ArgumentException(
                $"单元格地址格式无效: '{cell}'. 预期格式如 'A1', 'B2', 'AA100'");
    }

    private Task<string> WriteCellAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetString(arguments, "cell");
            var value = ArgumentHelper.GetString(arguments, "value");

            ValidateCellAddress(cell);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var cellObj = worksheet.Cells[cell];

            ExcelHelper.SetCellValue(cellObj, value);

            package.SaveAs(new FileInfo(outputPath));
            return $"单元格 {cell} 已写入值 '{value}' (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> EditCellAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetString(arguments, "cell");
            var value = ArgumentHelper.GetStringNullable(arguments, "value");
            var formula = ArgumentHelper.GetStringNullable(arguments, "formula");

            ValidateCellAddress(cell);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var cellObj = worksheet.Cells[cell];

            if (!string.IsNullOrEmpty(formula))
                cellObj.Formula = formula;
            else if (!string.IsNullOrEmpty(value))
                ExcelHelper.SetCellValue(cellObj, value);
            else
                throw new ArgumentException("必须提供 value 或 formula");

            package.SaveAs(new FileInfo(outputPath));
            return $"单元格 {cell} 已编辑 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> GetCellAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetString(arguments, "cell");

            ValidateCellAddress(cell);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var cellObj = worksheet.Cells[cell];

            var resultObj = new
            {
                cell,
                value = cellObj.Value?.ToString() ?? "(空)",
                formula = !string.IsNullOrEmpty(cellObj.Formula) ? cellObj.Formula : null,
                address = cellObj.Address
            };

            return JsonSerializer.Serialize(resultObj, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> ClearCellAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetString(arguments, "cell");

            ValidateCellAddress(cell);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var cellObj = worksheet.Cells[cell];

            cellObj.Clear();

            package.SaveAs(new FileInfo(outputPath));
            return $"单元格 {cell} 已清空 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }
}
