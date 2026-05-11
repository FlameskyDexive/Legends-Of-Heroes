using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using OfficeOpenXml.Filter;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     筛选工具 - 支持应用、移除、设置筛选条件等操作
/// </summary>
public class ExcelFilterTool : IExcelTool
{
    public string Description => @"管理 Excel 筛选。支持 4 种操作: apply, remove, filter, get_status。

使用示例:
- 应用自动筛选: excel_filter(operation='apply', path='book.xlsx', range='A1:C10')
- 移除筛选: excel_filter(operation='remove', path='book.xlsx')
- 按值筛选: excel_filter(operation='filter', path='book.xlsx', range='A1:C10', columnIndex=0, criteria='Completed')
- 获取筛选状态: excel_filter(operation='get_status', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'apply': 应用自动筛选下拉按钮 (必需参数: path, range)
- 'remove': 完全移除自动筛选 (必需参数: path)
- 'filter': 对列应用筛选条件 (必需参数: path, range, columnIndex, criteria)
- 'get_status': 获取筛选状态详情 (必需参数: path)",
                @enum = new[] { "apply", "remove", "filter", "get_status" }
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
            range = new
            {
                type = "string",
                description = "应用筛选的单元格范围 (例如 'A1:C10', 用于 apply/filter)"
            },
            columnIndex = new
            {
                type = "number",
                description = "筛选范围内的列索引 (从0开始, 用于 filter)"
            },
            criteria = new
            {
                type = "string",
                description = "筛选条件值 (用于 filter 操作)"
            },
            filterOperator = new
            {
                type = "string",
                description = "筛选运算符 (可选, 默认: 'Equal'). 用于数字/日期条件",
                @enum = new[]
                {
                    "Equal", "NotEqual", "GreaterThan", "GreaterOrEqual", "LessThan", "LessOrEqual"
                }
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 用于 apply/remove/filter 操作, 默认为输入路径)"
            }
        },
        required = new[] { "operation", "path" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation");
        var path = ArgumentHelper.GetAndValidatePath(arguments);
        var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);
        var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);

        return operation.ToLower() switch
        {
            "apply" => await ApplyFilterAsync(path, outputPath, sheetIndex, arguments),
            "remove" => await RemoveFilterAsync(path, outputPath, sheetIndex),
            "filter" => await FilterByValueAsync(path, outputPath, sheetIndex, arguments),
            "get_status" => await GetFilterStatusAsync(path, sheetIndex),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> ApplyFilterAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            // EPPlus 设置自动筛选
            worksheet.Cells[range].AutoFilter = true;

            package.SaveAs(new FileInfo(outputPath));

            return $"已应用自动筛选到范围 {range} (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> RemoveFilterAsync(string path, string outputPath, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            // 移除自动筛选
            if (worksheet.AutoFilter.Address != null)
            {
                worksheet.Cells[worksheet.AutoFilter.Address.Address].AutoFilter = false;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已移除工作表 {sheetIndex} 的自动筛选. 输出: {outputPath}";
        });
    }

    private Task<string> FilterByValueAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var columnIndex = ArgumentHelper.GetInt(arguments, "columnIndex");
            var criteria = ArgumentHelper.GetString(arguments, "criteria");
            var filterOperatorStr = ArgumentHelper.GetStringNullable(arguments, "filterOperator") ?? "Equal";

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            // 首先确保自动筛选已启用
            worksheet.Cells[range].AutoFilter = true;

            // 如果该列已存在筛选,先清除
            var existingColumn = worksheet.AutoFilter.Columns[columnIndex];
            if (existingColumn != null)
            {
                worksheet.AutoFilter.Columns.Remove(existingColumn);
            }

            // EPPlus 使用不同的方式添加筛选
            switch (filterOperatorStr.ToLower())
            {
                case "equal":
                    var valueColumn = worksheet.AutoFilter.Columns.AddValueFilterColumn(columnIndex);
                    valueColumn.Filters.Add(criteria);
                    break;
                case "notequal":
                case "greaterthan":
                case "greaterorequal":
                case "lessthan":
                case "lessorequal":
                    // 对于数值比较，使用 CustomFilter
                    var customColumn = worksheet.AutoFilter.Columns.AddCustomFilterColumn(columnIndex);
                    var op = filterOperatorStr.ToLower() switch
                    {
                        "notequal" => eFilterOperator.NotEqual,
                        "greaterthan" => eFilterOperator.GreaterThan,
                        "greaterorequal" => eFilterOperator.GreaterThanOrEqual,
                        "lessthan" => eFilterOperator.LessThan,
                        "lessorequal" => eFilterOperator.LessThanOrEqual,
                        _ => eFilterOperator.Equal
                    };
                    customColumn.Filters.Add(new ExcelFilterCustomItem(criteria, op));
                    break;
                default:
                    throw new ArgumentException($"不支持的筛选运算符: {filterOperatorStr}");
            }

            package.SaveAs(new FileInfo(outputPath));

            return $"已对列 {columnIndex} 应用筛选条件 '{criteria}' (运算符: {filterOperatorStr}). 输出: {outputPath}";
        });
    }

    private Task<string> GetFilterStatusAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var autoFilter = worksheet.AutoFilter;
            var isFilterEnabled = autoFilter.Address != null;
            var filterRange = autoFilter.Address?.Address;

            var filterColumnsCount = 0;
            if (isFilterEnabled && autoFilter.Columns != null)
            {
                filterColumnsCount = autoFilter.Columns.Count;
            }

            var result = new
            {
                worksheetName = worksheet.Name,
                isFilterEnabled,
                hasActiveFilters = filterColumnsCount > 0,
                status = isFilterEnabled
                    ? filterColumnsCount > 0
                        ? "自动筛选已启用，有筛选条件"
                        : "自动筛选已启用 (无筛选条件)"
                    : "自动筛选未启用",
                filterRange = isFilterEnabled ? filterRange : null,
                filterColumnsCount
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }
}
