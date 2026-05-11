using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 范围操作工具 (write, edit, get, clear, copy, move, copy_format)
/// </summary>
public class ExcelRangeTool : IExcelTool
{
    private const string OperationWrite = "write";
    private const string OperationEdit = "edit";
    private const string OperationGet = "get";
    private const string OperationClear = "clear";
    private const string OperationCopy = "copy";
    private const string OperationMove = "move";
    private const string OperationCopyFormat = "copy_format";

    public string Description => @"Excel 范围操作。支持 7 种操作: write, edit, get, clear, copy, move, copy_format。

使用示例:
- 批量写入: excel_range(operation='write', path='book.xlsx', range='A1:B2', data=[['A','B'],['C','D']])
- 读取范围: excel_range(operation='get', path='book.xlsx', range='A1:C3')
- 清空范围: excel_range(operation='clear', path='book.xlsx', range='A1:B2')
- 复制范围: excel_range(operation='copy', path='book.xlsx', sourceRange='A1:B2', destRange='C1')
- 移动范围: excel_range(operation='move', path='book.xlsx', sourceRange='A1:B2', destRange='C1')
- 复制格式: excel_range(operation='copy_format', path='book.xlsx', sourceRange='A1:B2', destRange='C1:D3')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'write': 批量写入数据到范围
- 'edit': 编辑范围数据或公式
- 'get': 读取范围数据
- 'clear': 清空范围
- 'copy': 复制范围到另一位置
- 'move': 移动范围到另一位置
- 'copy_format': 仅复制格式到另一范围",
                @enum = new[] { OperationWrite, OperationEdit, OperationGet, OperationClear, OperationCopy, OperationMove, OperationCopyFormat }
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
                description = "单元格范围 (如 'A1:B2', write/edit/get/clear操作使用)"
            },
            data = new
            {
                type = "array",
                description = @"要写入的数据 (write/edit操作使用)。
支持两种格式:
1. 二维数组: [['A','B'],['C','D']]
2. 对象数组: [{""cell"":""A1"",""value"":""10""},{""cell"":""A2"",""formula"":""=A1*2""}]"
            },
            sourceRange = new
            {
                type = "string",
                description = "源范围 (copy/move/copy_format操作使用)"
            },
            destRange = new
            {
                type = "string",
                description = "目标范围 (copy/move/copy_format操作使用)"
            },
            sourceSheetIndex = new
            {
                type = "number",
                description = "源工作表索引 (copy/move操作可选, 默认与sheetIndex相同)"
            },
            destSheetIndex = new
            {
                type = "number",
                description = "目标工作表索引 (copy/move操作可选, 默认与sheetIndex相同)"
            },
            copyOption = new
            {
                type = "string",
                description = "复制选项: 'all'(全部), 'values'(仅值), 'formats'(仅格式), 'formulas'(仅公式)",
                @enum = new[] { "all", "values", "formats", "formulas" }
            },
            includeFormulas = new
            {
                type = "boolean",
                description = "是否包含公式 (get操作可选, 默认false)"
            },
            includeFormat = new
            {
                type = "boolean",
                description = "是否包含格式信息 (get操作可选, 默认false)"
            },
            calculateFormulas = new
            {
                type = "boolean",
                description = "是否计算公式 (get操作可选, 默认true)"
            },
            clearRange = new
            {
                type = "boolean",
                description = "编辑前是否清空范围 (edit操作可选, 默认false)"
            },
            clearContent = new
            {
                type = "boolean",
                description = "是否清空内容 (clear操作可选, 默认true)"
            },
            clearFormat = new
            {
                type = "boolean",
                description = "是否清空格式 (clear操作可选, 默认false)"
            },
            copyValue = new
            {
                type = "boolean",
                description = "复制格式时是否同时复制值 (copy_format操作可选, 默认false)"
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
            OperationWrite => await WriteRangeAsync(path, arguments),
            OperationEdit => await EditRangeAsync(path, arguments),
            OperationGet => await GetRangeAsync(path, arguments),
            OperationClear => await ClearRangeAsync(path, arguments),
            OperationCopy => await CopyRangeAsync(path, arguments),
            OperationMove => await MoveRangeAsync(path, arguments),
            OperationCopyFormat => await CopyFormatAsync(path, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> WriteRangeAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var clearRange = ArgumentHelper.GetBool(arguments, "clearRange", false);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);
            var dataNode = arguments?["data"];

            if (dataNode == null)
                throw new ArgumentNullException(nameof(dataNode), "参数 'data' 是必需的");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            // 如果需要，先清空范围
            if (clearRange)
            {
                worksheet.Cells[range].Clear();
            }

            // 支持两种数据格式
            if (dataNode is JsonArray dataArray)
            {
                // 检查是否为二维数组格式
                if (dataArray.Count > 0 && dataArray[0] is JsonArray)
                {
                    // 二维数组格式: [['A','B'],['C','D']]
                    WriteFromTwoDimensionArray(worksheet, range, dataArray);
                }
                else
                {
                    // 对象数组格式: [{"cell":"A1","value":"10"}]
                    WriteFromObjectArray(worksheet, dataArray);
                }
            }
            else
            {
                throw new ArgumentException("data 必须是数组格式");
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"范围 '{range}' 数据已写入. 输出: {outputPath}";
        });
    }

    private void WriteFromTwoDimensionArray(ExcelWorksheet worksheet, string range, JsonArray dataArray)
    {
        var rangeAddress = new OfficeOpenXml.ExcelAddress(range);
        int startRow = rangeAddress.Start.Row;
        int startCol = rangeAddress.Start.Column;

        for (int i = 0; i < dataArray.Count; i++)
        {
            if (dataArray[i] is JsonArray rowArray)
            {
                for (int j = 0; j < rowArray.Count; j++)
                {
                    var cell = worksheet.Cells[startRow + i, startCol + j];
                    var value = rowArray[j];

                    if (value != null)
                    {
                        SetCellValue(cell, value.ToString());
                    }
                }
            }
        }
    }

    private void WriteFromObjectArray(ExcelWorksheet worksheet, JsonArray dataArray)
    {
        foreach (var item in dataArray)
        {
            if (item is JsonObject obj)
            {
                var cellAddr = obj["cell"]?.ToString();
                if (string.IsNullOrEmpty(cellAddr))
                    continue;

                var cell = worksheet.Cells[cellAddr];

                // 优先使用 formula，其次 value
                if (obj.ContainsKey("formula"))
                {
                    cell.Formula = obj["formula"]?.ToString() ?? "";
                }
                else if (obj.ContainsKey("value"))
                {
                    var valueStr = obj["value"]?.ToString();
                    if (valueStr != null)
                    {
                        SetCellValue(cell, valueStr);
                    }
                }
            }
        }
    }

    private void SetCellValue(OfficeOpenXml.ExcelRange cell, string valueStr)
    {
        // 处理单引号前缀（防止单元格引用歧义）
        if (valueStr.StartsWith("'"))
        {
            cell.Value = valueStr.Substring(1);
            return;
        }

        // 尝试类型转换
        var parsedValue = ArgumentHelper.ParseValue(valueStr);
        cell.Value = parsedValue;
    }

    private Task<string> EditRangeAsync(string path, JsonObject? arguments)
    {
        // Edit 操作与 Write 类似，但用于修改现有数据
        return WriteRangeAsync(path, arguments);
    }

    private Task<string> GetRangeAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var includeFormulas = ArgumentHelper.GetBool(arguments, "includeFormulas", false);
            var includeFormat = ArgumentHelper.GetBool(arguments, "includeFormat", false);
            var calculateFormulas = ArgumentHelper.GetBool(arguments, "calculateFormulas", true);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            if (calculateFormulas)
            {
                worksheet.Calculate();
            }

            var rangeObj = worksheet.Cells[range];
            var rangeAddress = rangeObj.Address;

            var items = new List<object>();

            for (int row = rangeObj.Start.Row; row <= rangeObj.End.Row; row++)
            {
                for (int col = rangeObj.Start.Column; col <= rangeObj.End.Column; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    var cellAddress = cell.Address;

                    var item = new Dictionary<string, object?>
                    {
                        ["cell"] = cellAddress,
                        ["value"] = cell.Text
                    };

                    if (includeFormulas && !string.IsNullOrEmpty(cell.Formula))
                    {
                        item["formula"] = cell.Formula;
                    }

                    if (includeFormat)
                    {
                        var style = cell.Style;
                        var formatData = new Dictionary<string, object?>
                        {
                            ["fontName"] = style.Font.Name,
                            ["fontSize"] = style.Font.Size,
                            ["bold"] = style.Font.Bold,
                            ["italic"] = style.Font.Italic,
                            ["underline"] = style.Font.UnderLine,
                            ["strikethrough"] = style.Font.Strike,
                            ["fontColor"] = style.Font.Color.Rgb,
                            ["backgroundColor"] = style.Fill.BackgroundColor.Rgb,
                            ["patternType"] = style.Fill.PatternType.ToString(),
                            ["horizontalAlignment"] = style.HorizontalAlignment.ToString(),
                            ["verticalAlignment"] = style.VerticalAlignment.ToString(),
                            ["numberFormat"] = style.Numberformat.Format
                        };
                        item["format"] = formatData;
                    }

                    items.Add(item);
                }
            }

            var result = new
            {
                range = rangeAddress,
                rowCount = rangeObj.End.Row - rangeObj.Start.Row + 1,
                columnCount = rangeObj.End.Column - rangeObj.Start.Column + 1,
                items
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> ClearRangeAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var clearContent = ArgumentHelper.GetBool(arguments, "clearContent", true);
            var clearFormat = ArgumentHelper.GetBool(arguments, "clearFormat", false);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var cells = worksheet.Cells[range];

            if (clearContent && clearFormat)
            {
                // 清空所有内容和格式
                cells.Clear();
            }
            else if (clearContent)
            {
                // 仅清空内容，保留格式
                for (int row = cells.Start.Row; row <= cells.End.Row; row++)
                {
                    for (int col = cells.Start.Column; col <= cells.End.Column; col++)
                    {
                        var cell = worksheet.Cells[row, col];
                        cell.Value = null;
                        cell.Formula = null;
                    }
                }
            }
            else if (clearFormat)
            {
                // 仅清空格式，保留内容
                cells.Style.Font.Name = "Calibri";
                cells.Style.Font.Size = 11;
                cells.Style.Font.Bold = false;
                cells.Style.Font.Italic = false;
                cells.Style.Font.UnderLine = false;
                cells.Style.Font.Strike = false;
                cells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                cells.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                cells.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                cells.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                cells.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                cells.Style.Numberformat.Format = "";
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"范围 '{range}' 已清空. 输出: {outputPath}";
        });
    }

    private Task<string> CopyRangeAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sourceRange = ArgumentHelper.GetString(arguments, "sourceRange");
            var destRange = ArgumentHelper.GetString(arguments, "destRange");
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var sourceSheetIndex = ArgumentHelper.GetInt(arguments, "sourceSheetIndex", sheetIndex);
            var destSheetIndex = ArgumentHelper.GetInt(arguments, "destSheetIndex", sheetIndex);
            var copyOption = ArgumentHelper.GetString(arguments, "copyOption", "all").ToLowerInvariant();
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var sourceWorksheet = ExcelHelper.GetWorksheet(package, sourceSheetIndex);
            var destWorksheet = ExcelHelper.GetWorksheet(package, destSheetIndex);

            var sourceRangeObj = sourceWorksheet.Cells[sourceRange];
            var destRangeObj = destWorksheet.Cells[destRange];

            // 根据复制选项执行不同的复制操作
            switch (copyOption)
            {
                case "all":
                    sourceRangeObj.Copy(destRangeObj);
                    break;
                case "values":
                    CopyValues(sourceRangeObj, destRangeObj);
                    break;
                case "formats":
                    sourceRangeObj.Copy(destRangeObj, ExcelRangeCopyOptionFlags.ExcludeFormulas);
                    break;
                case "formulas":
                    CopyFormulas(sourceRangeObj, destRangeObj);
                    break;
                default:
                    throw new ArgumentException($"不支持的复制选项: {copyOption}");
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"范围已复制: {sourceRange} -> {destRange} (选项: {copyOption}). 输出: {outputPath}";
        });
    }

    private void CopyValues(OfficeOpenXml.ExcelRange source, OfficeOpenXml.ExcelRange dest)
    {
        int rowCount = source.End.Row - source.Start.Row + 1;
        int colCount = source.End.Column - source.Start.Column + 1;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                var sourceCell = source.Worksheet.Cells[source.Start.Row + i, source.Start.Column + j];
                var destCell = dest.Worksheet.Cells[dest.Start.Row + i, dest.Start.Column + j];
                destCell.Value = sourceCell.Value;
            }
        }
    }

    private void CopyFormulas(OfficeOpenXml.ExcelRange source, OfficeOpenXml.ExcelRange dest)
    {
        int rowCount = source.End.Row - source.Start.Row + 1;
        int colCount = source.End.Column - source.Start.Column + 1;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                var sourceCell = source.Worksheet.Cells[source.Start.Row + i, source.Start.Column + j];
                var destCell = dest.Worksheet.Cells[dest.Start.Row + i, dest.Start.Column + j];

                if (!string.IsNullOrEmpty(sourceCell.Formula))
                {
                    destCell.Formula = sourceCell.Formula;
                }
                else
                {
                    destCell.Value = sourceCell.Value;
                }
            }
        }
    }

    private Task<string> MoveRangeAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sourceRange = ArgumentHelper.GetString(arguments, "sourceRange");
            var destRange = ArgumentHelper.GetString(arguments, "destRange");
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var sourceSheetIndex = ArgumentHelper.GetInt(arguments, "sourceSheetIndex", sheetIndex);
            var destSheetIndex = ArgumentHelper.GetInt(arguments, "destSheetIndex", sheetIndex);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var sourceWorksheet = ExcelHelper.GetWorksheet(package, sourceSheetIndex);
            var destWorksheet = ExcelHelper.GetWorksheet(package, destSheetIndex);

            var sourceRangeObj = sourceWorksheet.Cells[sourceRange];
            var destRangeObj = destWorksheet.Cells[destRange];

            // 复制到目标位置
            sourceRangeObj.Copy(destRangeObj);

            // 清空源位置
            sourceRangeObj.Clear();

            package.SaveAs(new FileInfo(outputPath));
            return $"范围已移动: {sourceRange} -> {destRange}. 输出: {outputPath}";
        });
    }

    private Task<string> CopyFormatAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sourceRange = ArgumentHelper.GetString(arguments, "sourceRange");
            var destRange = ArgumentHelper.GetString(arguments, "destRange");
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var sourceSheetIndex = ArgumentHelper.GetInt(arguments, "sourceSheetIndex", sheetIndex);
            var destSheetIndex = ArgumentHelper.GetInt(arguments, "destSheetIndex", sheetIndex);
            var copyValue = ArgumentHelper.GetBool(arguments, "copyValue", false);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var sourceWorksheet = ExcelHelper.GetWorksheet(package, sourceSheetIndex);
            var destWorksheet = ExcelHelper.GetWorksheet(package, destSheetIndex);

            var sourceRangeObj = sourceWorksheet.Cells[sourceRange];
            var destRangeObj = destWorksheet.Cells[destRange];

            if (copyValue)
            {
                // 复制格式和值
                sourceRangeObj.Copy(destRangeObj, ExcelRangeCopyOptionFlags.ExcludeFormulas);
            }
            else
            {
                // 仅复制格式，不复制值和公式
                sourceRangeObj.Copy(destRangeObj, ExcelRangeCopyOptionFlags.ExcludeFormulas);

                // 清除目标范围的值（保留格式）
                int rowCount = destRangeObj.End.Row - destRangeObj.Start.Row + 1;
                int colCount = destRangeObj.End.Column - destRangeObj.Start.Column + 1;

                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        var cell = destWorksheet.Cells[destRangeObj.Start.Row + i, destRangeObj.Start.Column + j];
                        cell.Value = null;
                    }
                }
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"格式已复制: {sourceRange} -> {destRange}. 输出: {outputPath}";
        });
    }
}
