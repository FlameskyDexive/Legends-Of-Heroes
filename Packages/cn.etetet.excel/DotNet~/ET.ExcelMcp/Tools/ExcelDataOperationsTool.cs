using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     数据操作工具 - 支持排序、查找替换、批量写入、获取内容、统计信息等
/// </summary>
public class ExcelDataOperationsTool : IExcelTool
{
    public string Description => @"Excel 数据操作。支持 6 种操作: sort, find_replace, batch_write, get_content, get_statistics, get_used_range。

使用示例:
- 排序数据: excel_data_operations(operation='sort', path='book.xlsx', range='A1:C10', sortColumn=0)
- 查找替换: excel_data_operations(operation='find_replace', path='book.xlsx', findText='old', replaceText='new')
- 批量写入: excel_data_operations(operation='batch_write', path='book.xlsx', data={'A1':'Value1','B1':'Value2'})
- 获取内容: excel_data_operations(operation='get_content', path='book.xlsx', range='A1:C10')
- 获取统计: excel_data_operations(operation='get_statistics', path='book.xlsx', range='A1:A10')
- 获取已用范围: excel_data_operations(operation='get_used_range', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'sort': 排序范围内的数据 (必需参数: path, range, sortColumn)
- 'find_replace': 查找并替换文本 (必需参数: path, findText, replaceText)
- 'batch_write': 批量写入多个值 (必需参数: path, data)
- 'get_content': 获取单元格内容 (必需参数: path)
- 'get_statistics': 获取范围统计信息 (必需参数: path, range)
- 'get_used_range': 获取已用范围信息 (必需参数: path)",
                @enum = new[]
                    { "sort", "find_replace", "batch_write", "get_content", "get_statistics", "get_used_range" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 用于 sort/find_replace/batch_write 操作, 默认为输入路径)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 可选, 默认: 0)"
            },
            range = new
            {
                type = "string",
                description = "单元格范围 (例如 'A1:C10', 用于 sort 必需, get_content 可选)"
            },
            sortColumn = new
            {
                type = "number",
                description = "排序依据的列索引 (从0开始, 相对于范围起始, 用于 sort 必需)"
            },
            ascending = new
            {
                type = "boolean",
                description = "true 为升序, false 为降序 (可选, 用于 sort, 默认: true)"
            },
            hasHeader = new
            {
                type = "boolean",
                description = "范围是否有标题行 (可选, 用于 sort, 默认: false)"
            },
            findText = new
            {
                type = "string",
                description = "要查找的文本 (用于 find_replace 必需)"
            },
            replaceText = new
            {
                type = "string",
                description = "替换为的文本 (用于 find_replace 必需)"
            },
            matchCase = new
            {
                type = "boolean",
                description = "是否区分大小写 (可选, 用于 find_replace, 默认: false)"
            },
            matchEntireCell = new
            {
                type = "boolean",
                description = "是否匹配整个单元格内容 (可选, 用于 find_replace, 默认: false)"
            },
            data = new
            {
                type = "array",
                description = @"批量写入的数据。支持两种格式:
(1) 数组: [{""cell"":""A1"",""value"":""val1""},...]
(2) 对象: {""A1"":""val1"",""B1"":""val2""} - 更紧凑",
                items = new
                {
                    type = "object",
                    properties = new
                    {
                        cell = new { type = "string" },
                        value = new { type = "string" }
                    },
                    required = new[] { "cell", "value" }
                }
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
            "sort" => await SortDataAsync(path, outputPath, sheetIndex, arguments),
            "find_replace" => await FindReplaceAsync(path, outputPath, sheetIndex, arguments),
            "batch_write" => await BatchWriteAsync(path, outputPath, sheetIndex, arguments),
            "get_content" => await GetContentAsync(path, sheetIndex, arguments),
            "get_statistics" => await GetStatisticsAsync(path, sheetIndex, arguments),
            "get_used_range" => await GetUsedRangeAsync(path, sheetIndex),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> SortDataAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var sortColumn = ArgumentHelper.GetInt(arguments, "sortColumn");
            var ascending = ArgumentHelper.GetBool(arguments, "ascending", true);
            var hasHeader = ArgumentHelper.GetBool(arguments, "hasHeader", false);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var rangeObj = worksheet.Cells[range];
            var startRow = hasHeader ? rangeObj.Start.Row + 1 : rangeObj.Start.Row;
            var endRow = rangeObj.End.Row;
            var startCol = rangeObj.Start.Column;
            var endCol = rangeObj.End.Column;

            // 读取所有数据到列表
            var rows = new List<List<object?>>();

            if (hasHeader)
            {
                // 保存标题行
                var headerRow = new List<object?>();
                for (var col = startCol; col <= endCol; col++)
                    headerRow.Add(worksheet.Cells[rangeObj.Start.Row, col].Value);
                rows.Add(headerRow);
            }

            // 读取数据行
            for (var row = startRow; row <= endRow; row++)
            {
                var rowData = new List<object?>();
                for (var col = startCol; col <= endCol; col++)
                    rowData.Add(worksheet.Cells[row, col].Value);
                rows.Add(rowData);
            }

            // 排序数据行 (跳过标题行)
            var dataRows = hasHeader ? rows.Skip(1).ToList() : rows;
            dataRows.Sort((a, b) =>
            {
                var aVal = a[sortColumn];
                var bVal = b[sortColumn];

                if (aVal == null && bVal == null) return 0;
                if (aVal == null) return ascending ? -1 : 1;
                if (bVal == null) return ascending ? 1 : -1;

                var comparison = Comparer<object>.Default.Compare(aVal, bVal);
                return ascending ? comparison : -comparison;
            });

            // 重建行列表
            if (hasHeader)
            {
                rows = [rows[0]];
                rows.AddRange(dataRows);
            }
            else
            {
                rows = dataRows;
            }

            // 写回数据
            for (var i = 0; i < rows.Count; i++)
            {
                var rowData = rows[i];
                var targetRow = rangeObj.Start.Row + i;
                for (var j = 0; j < rowData.Count; j++)
                    worksheet.Cells[targetRow, startCol + j].Value = rowData[j];
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已排序范围 {range}, 按列 {sortColumn} ({(ascending ? "升序" : "降序")}). 输出: {outputPath}";
        });
    }

    private Task<string> FindReplaceAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var findText = ArgumentHelper.GetString(arguments, "findText");
            var replaceText = ArgumentHelper.GetString(arguments, "replaceText");
            var matchCase = ArgumentHelper.GetBool(arguments, "matchCase", false);
            var matchEntireCell = ArgumentHelper.GetBool(arguments, "matchEntireCell", false);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var totalReplacements = 0;
            var dimension = worksheet.Dimension;

            if (dimension != null)
            {
                for (var row = dimension.Start.Row; row <= dimension.End.Row; row++)
                for (var col = dimension.Start.Column; col <= dimension.End.Column; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    var cellValue = cell.Value?.ToString();

                    if (string.IsNullOrEmpty(cellValue)) continue;

                    var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                    if (matchEntireCell)
                    {
                        if (cellValue.Equals(findText, comparison))
                        {
                            cell.Value = replaceText;
                            totalReplacements++;
                        }
                    }
                    else
                    {
                        if (cellValue.Contains(findText, comparison))
                        {
                            var newValue = cellValue.Replace(findText, replaceText, comparison);
                            cell.Value = newValue;
                            totalReplacements++;
                        }
                    }
                }
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已将 '{findText}' 替换为 '{replaceText}' (共 {totalReplacements} 处替换). 输出: {outputPath}";
        });
    }

    private Task<string> BatchWriteAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var writeCount = 0;

            if (arguments != null && arguments.TryGetPropertyValue("data", out var dataNode))
            {
                if (dataNode is JsonArray dataArray)
                {
                    foreach (var item in dataArray)
                    {
                        var itemObj = item?.AsObject();
                        if (itemObj != null)
                        {
                            var cell = itemObj["cell"]?.GetValue<string>();
                            var value = itemObj["value"]?.GetValue<string>() ?? "";
                            if (!string.IsNullOrEmpty(cell))
                            {
                                var cellObj = worksheet.Cells[cell];
                                ExcelHelper.SetCellValue(cellObj, value);
                                writeCount++;
                            }
                        }
                    }
                }
                else if (dataNode is JsonObject dataObject)
                {
                    foreach (var kvp in dataObject)
                    {
                        var cell = kvp.Key;
                        var value = kvp.Value?.GetValue<string>() ?? "";
                        if (!string.IsNullOrEmpty(cell))
                        {
                            var cellObj = worksheet.Cells[cell];
                            ExcelHelper.SetCellValue(cellObj, value);
                            writeCount++;
                        }
                    }
                }
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"批量写入完成 (共写入 {writeCount} 个单元格到工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> GetContentAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetStringNullable(arguments, "range");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            int startRow, endRow, startCol, endCol;

            if (!string.IsNullOrEmpty(range))
            {
                var rangeObj = worksheet.Cells[range];
                startRow = rangeObj.Start.Row;
                endRow = rangeObj.End.Row;
                startCol = rangeObj.Start.Column;
                endCol = rangeObj.End.Column;
            }
            else
            {
                var dimension = worksheet.Dimension;
                if (dimension == null)
                    return "[]";

                startRow = dimension.Start.Row;
                endRow = dimension.End.Row;
                startCol = dimension.Start.Column;
                endCol = dimension.End.Column;
            }

            var rows = new List<Dictionary<string, object?>>();
            for (var row = startRow; row <= endRow; row++)
            {
                var rowDict = new Dictionary<string, object?>();
                for (var col = startCol; col <= endCol; col++)
                {
                    var colName = ExcelCellBase.GetAddressCol(col);
                    var value = worksheet.Cells[row, col].Value;
                    rowDict[colName] = value;
                }
                rows.Add(rowDict);
            }

            return JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = false });
        });
    }

    private Task<string> GetStatisticsAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetStringNullable(arguments, "range");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var baseStats = new Dictionary<string, object>
            {
                ["index"] = sheetIndex,
                ["name"] = worksheet.Name,
                ["maxDataRow"] = worksheet.Dimension?.End.Row ?? 0,
                ["maxDataColumn"] = worksheet.Dimension?.End.Column ?? 0
            };

            // 如果指定了范围，计算详细统计
            if (!string.IsNullOrEmpty(range))
            {
                var rangeObj = worksheet.Cells[range];
                var numericValues = new List<double>();
                var nonNumericCount = 0;
                var emptyCount = 0;

                for (var row = rangeObj.Start.Row; row <= rangeObj.End.Row; row++)
                for (var col = rangeObj.Start.Column; col <= rangeObj.End.Column; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    var value = cell.Value;

                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                        emptyCount++;
                    else if (value is double || value is int || value is float || value is decimal)
                        numericValues.Add(Convert.ToDouble(value));
                    else if (double.TryParse(value.ToString(), NumberStyles.Any,
                                 CultureInfo.InvariantCulture, out var numValue))
                        numericValues.Add(numValue);
                    else
                        nonNumericCount++;
                }

                var rangeStats = new Dictionary<string, object>
                {
                    ["range"] = range,
                    ["totalCells"] = (rangeObj.End.Row - rangeObj.Start.Row + 1) *
                                    (rangeObj.End.Column - rangeObj.Start.Column + 1),
                    ["numericCells"] = numericValues.Count,
                    ["nonNumericCells"] = nonNumericCount,
                    ["emptyCells"] = emptyCount
                };

                if (numericValues.Count > 0)
                {
                    numericValues.Sort();
                    rangeStats["sum"] = Math.Round(numericValues.Sum(), 2);
                    rangeStats["average"] = Math.Round(numericValues.Sum() / numericValues.Count, 2);
                    rangeStats["min"] = Math.Round(numericValues[0], 2);
                    rangeStats["max"] = Math.Round(numericValues[^1], 2);
                    rangeStats["count"] = numericValues.Count;
                }

                baseStats["rangeStatistics"] = rangeStats;
            }

            return JsonSerializer.Serialize(baseStats, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> GetUsedRangeAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var dimension = worksheet.Dimension;
            string? rangeAddress = null;

            if (dimension != null)
            {
                rangeAddress = dimension.Address;
            }

            var result = new
            {
                worksheetName = worksheet.Name,
                sheetIndex,
                firstRow = dimension?.Start.Row ?? 0,
                lastRow = dimension?.End.Row ?? 0,
                firstColumn = dimension?.Start.Column ?? 0,
                lastColumn = dimension?.End.Column ?? 0,
                range = rangeAddress
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }
}
