using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 合并单元格工具 (merge, unmerge, get_merged)
/// </summary>
public class ExcelMergeCellsTool : IExcelTool
{
    public string Description => @"Excel 合并单元格操作。支持 3 种操作: merge, unmerge, get_merged。

使用示例:
- 合并单元格: excel_merge_cells(operation='merge', path='book.xlsx', range='A1:C3')
- 取消合并: excel_merge_cells(operation='unmerge', path='book.xlsx', range='A1:C3')
- 获取合并区域: excel_merge_cells(operation='get_merged', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'merge': 合并单元格 (必需参数: path, range)
- 'unmerge': 取消合并单元格 (必需参数: path, range)
- 'get_merged': 获取所有合并区域信息 (必需参数: path)",
                @enum = new[] { "merge", "unmerge", "get_merged" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 默认为输入路径)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 默认0)"
            },
            range = new
            {
                type = "string",
                description = "要合并或取消合并的单元格范围 (如 'A1:C3', merge/unmerge 操作必需)"
            },
            keepValue = new
            {
                type = "boolean",
                description = "取消合并时是否保留值 (unmerge 操作可选, 默认true)"
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
            "merge" => await MergeCellsAsync(path, arguments),
            "unmerge" => await UnmergeCellsAsync(path, arguments),
            "get_merged" => await GetMergedCellsAsync(path, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> MergeCellsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            // 合并单元格
            worksheet.Cells[range].Merge = true;

            package.SaveAs(new FileInfo(outputPath));
            return $"已合并单元格范围 '{range}' (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> UnmergeCellsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var keepValue = ArgumentHelper.GetBool(arguments, "keepValue", true);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var cells = worksheet.Cells[range];

            // 保存原始值（如果需要）
            object? originalValue = null;
            if (keepValue && cells.Merge)
            {
                originalValue = cells.Value;
            }

            // 取消合并
            cells.Merge = false;

            // 如果需要保留值，将值填充到所有单元格
            if (keepValue && originalValue != null)
            {
                for (int row = cells.Start.Row; row <= cells.End.Row; row++)
                {
                    for (int col = cells.Start.Column; col <= cells.End.Column; col++)
                    {
                        worksheet.Cells[row, col].Value = originalValue;
                    }
                }
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已取消合并单元格范围 '{range}' (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> GetMergedCellsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var mergedCells = new List<object>();

            // 遍历所有合并的单元格区域
            foreach (var mergedAddress in worksheet.MergedCells)
            {
                var mergedRange = worksheet.Cells[mergedAddress];
                // 获取合并区域的起始单元格的值
                var startCell = worksheet.Cells[mergedRange.Start.Row, mergedRange.Start.Column];

                var mergeInfo = new
                {
                    address = mergedAddress,
                    startRow = mergedRange.Start.Row,
                    startColumn = mergedRange.Start.Column,
                    endRow = mergedRange.End.Row,
                    endColumn = mergedRange.End.Column,
                    rowCount = mergedRange.End.Row - mergedRange.Start.Row + 1,
                    columnCount = mergedRange.End.Column - mergedRange.Start.Column + 1,
                    value = startCell.Value?.ToString() ?? ""
                };

                mergedCells.Add(mergeInfo);
            }

            var result = new
            {
                worksheetName = worksheet.Name,
                sheetIndex,
                mergedCellsCount = mergedCells.Count,
                mergedCells
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }
}
