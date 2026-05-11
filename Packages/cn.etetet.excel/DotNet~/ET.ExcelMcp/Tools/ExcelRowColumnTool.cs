using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 行列操作工具 (insert_rows, insert_columns, delete_rows, delete_columns, hide, show, set_size, auto_fit, get_info)
/// </summary>
public class ExcelRowColumnTool : IExcelTool
{
    public string Description => @"Excel 行列操作。支持 9 种操作: insert_rows, insert_columns, delete_rows, delete_columns, hide_rows, hide_columns, show_rows, show_columns, set_size, auto_fit, get_info。

使用示例:
- 插入行: excel_row_column(operation='insert_rows', path='book.xlsx', startRow=2, count=3)
- 插入列: excel_row_column(operation='insert_columns', path='book.xlsx', startColumn=2, count=2)
- 删除行: excel_row_column(operation='delete_rows', path='book.xlsx', startRow=5, count=2)
- 删除列: excel_row_column(operation='delete_columns', path='book.xlsx', startColumn=3, count=1)
- 隐藏行: excel_row_column(operation='hide_rows', path='book.xlsx', startRow=2, endRow=5)
- 隐藏列: excel_row_column(operation='hide_columns', path='book.xlsx', startColumn=2, endColumn=4)
- 显示行: excel_row_column(operation='show_rows', path='book.xlsx', startRow=2, endRow=5)
- 显示列: excel_row_column(operation='show_columns', path='book.xlsx', startColumn=2, endColumn=4)
- 设置大小: excel_row_column(operation='set_size', path='book.xlsx', rowIndex=1, rowHeight=30, columnIndex=1, columnWidth=20)
- 自动调整: excel_row_column(operation='auto_fit', path='book.xlsx', rows=[1,2,3], columns=[1,2,3])
- 获取信息: excel_row_column(operation='get_info', path='book.xlsx', rowIndex=1, columnIndex=1)";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'insert_rows': 插入行 (必需: path, startRow, count)
- 'insert_columns': 插入列 (必需: path, startColumn, count)
- 'delete_rows': 删除行 (必需: path, startRow, count)
- 'delete_columns': 删除列 (必需: path, startColumn, count)
- 'hide_rows': 隐藏行 (必需: path, startRow, endRow)
- 'hide_columns': 隐藏列 (必需: path, startColumn, endColumn)
- 'show_rows': 显示行 (必需: path, startRow, endRow)
- 'show_columns': 显示列 (必需: path, startColumn, endColumn)
- 'set_size': 设置行高列宽 (必需: path, 可选: rowIndex, rowHeight, columnIndex, columnWidth)
- 'auto_fit': 自动调整大小 (必需: path, 可选: rows, columns)
- 'get_info': 获取行列信息 (必需: path, 可选: rowIndex, columnIndex)",
                @enum = new[] { "insert_rows", "insert_columns", "delete_rows", "delete_columns", "hide_rows", "hide_columns", "show_rows", "show_columns", "set_size", "auto_fit", "get_info" }
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
            startRow = new
            {
                type = "number",
                description = "起始行号 (从1开始, insert_rows/delete_rows/hide_rows/show_rows 操作使用)"
            },
            endRow = new
            {
                type = "number",
                description = "结束行号 (从1开始, hide_rows/show_rows 操作使用)"
            },
            startColumn = new
            {
                type = "number",
                description = "起始列号 (从1开始, insert_columns/delete_columns/hide_columns/show_columns 操作使用)"
            },
            endColumn = new
            {
                type = "number",
                description = "结束列号 (从1开始, hide_columns/show_columns 操作使用)"
            },
            count = new
            {
                type = "number",
                description = "插入或删除的行数/列数 (insert_rows/insert_columns/delete_rows/delete_columns 操作必需)"
            },
            rowIndex = new
            {
                type = "number",
                description = "行索引 (从1开始, set_size/get_info 操作使用)"
            },
            rowHeight = new
            {
                type = "number",
                description = "行高 (set_size 操作使用)"
            },
            columnIndex = new
            {
                type = "number",
                description = "列索引 (从1开始, set_size/get_info 操作使用)"
            },
            columnWidth = new
            {
                type = "number",
                description = "列宽 (set_size 操作使用)"
            },
            rows = new
            {
                type = "array",
                description = "要自动调整的行号数组 (auto_fit 操作使用)"
            },
            columns = new
            {
                type = "array",
                description = "要自动调整的列号数组 (auto_fit 操作使用)"
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
            "insert_rows" => await InsertRowsAsync(path, arguments),
            "insert_columns" => await InsertColumnsAsync(path, arguments),
            "delete_rows" => await DeleteRowsAsync(path, arguments),
            "delete_columns" => await DeleteColumnsAsync(path, arguments),
            "hide_rows" => await HideRowsAsync(path, arguments),
            "hide_columns" => await HideColumnsAsync(path, arguments),
            "show_rows" => await ShowRowsAsync(path, arguments),
            "show_columns" => await ShowColumnsAsync(path, arguments),
            "set_size" => await SetSizeAsync(path, arguments),
            "auto_fit" => await AutoFitAsync(path, arguments),
            "get_info" => await GetInfoAsync(path, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> InsertRowsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var startRow = ArgumentHelper.GetInt(arguments, "startRow");
            var count = ArgumentHelper.GetInt(arguments, "count");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            worksheet.InsertRow(startRow, count);

            package.SaveAs(new FileInfo(outputPath));
            return $"在第 {startRow} 行处插入了 {count} 行 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> InsertColumnsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var startColumn = ArgumentHelper.GetInt(arguments, "startColumn");
            var count = ArgumentHelper.GetInt(arguments, "count");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            worksheet.InsertColumn(startColumn, count);

            package.SaveAs(new FileInfo(outputPath));
            return $"在第 {startColumn} 列处插入了 {count} 列 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> DeleteRowsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var startRow = ArgumentHelper.GetInt(arguments, "startRow");
            var count = ArgumentHelper.GetInt(arguments, "count");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            worksheet.DeleteRow(startRow, count);

            package.SaveAs(new FileInfo(outputPath));
            return $"从第 {startRow} 行开始删除了 {count} 行 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> DeleteColumnsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var startColumn = ArgumentHelper.GetInt(arguments, "startColumn");
            var count = ArgumentHelper.GetInt(arguments, "count");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            worksheet.DeleteColumn(startColumn, count);

            package.SaveAs(new FileInfo(outputPath));
            return $"从第 {startColumn} 列开始删除了 {count} 列 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> HideRowsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var startRow = ArgumentHelper.GetInt(arguments, "startRow");
            var endRow = ArgumentHelper.GetInt(arguments, "endRow");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            for (int row = startRow; row <= endRow; row++)
            {
                worksheet.Row(row).Hidden = true;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已隐藏第 {startRow} 到 {endRow} 行 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> HideColumnsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var startColumn = ArgumentHelper.GetInt(arguments, "startColumn");
            var endColumn = ArgumentHelper.GetInt(arguments, "endColumn");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            for (int col = startColumn; col <= endColumn; col++)
            {
                worksheet.Column(col).Hidden = true;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已隐藏第 {startColumn} 到 {endColumn} 列 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> ShowRowsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var startRow = ArgumentHelper.GetInt(arguments, "startRow");
            var endRow = ArgumentHelper.GetInt(arguments, "endRow");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            for (int row = startRow; row <= endRow; row++)
            {
                worksheet.Row(row).Hidden = false;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已显示第 {startRow} 到 {endRow} 行 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> ShowColumnsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var startColumn = ArgumentHelper.GetInt(arguments, "startColumn");
            var endColumn = ArgumentHelper.GetInt(arguments, "endColumn");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            for (int col = startColumn; col <= endColumn; col++)
            {
                worksheet.Column(col).Hidden = false;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已显示第 {startColumn} 到 {endColumn} 列 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> SetSizeAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var rowIndex = ArgumentHelper.GetIntNullable(arguments, "rowIndex");
            var rowHeight = ArgumentHelper.GetIntNullable(arguments, "rowHeight");
            var columnIndex = ArgumentHelper.GetIntNullable(arguments, "columnIndex");
            var columnWidth = ArgumentHelper.GetIntNullable(arguments, "columnWidth");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            if (rowIndex.HasValue && rowHeight.HasValue)
            {
                worksheet.Row(rowIndex.Value).Height = rowHeight.Value;
            }

            if (columnIndex.HasValue && columnWidth.HasValue)
            {
                worksheet.Column(columnIndex.Value).Width = columnWidth.Value;
            }

            package.SaveAs(new FileInfo(outputPath));

            var message = "";
            if (rowIndex.HasValue && rowHeight.HasValue)
                message += $"第 {rowIndex.Value} 行高设置为 {rowHeight.Value}. ";
            if (columnIndex.HasValue && columnWidth.HasValue)
                message += $"第 {columnIndex.Value} 列宽设置为 {columnWidth.Value}. ";

            return message + $"(工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> AutoFitAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var rowsArray = ArgumentHelper.GetArray(arguments, "rows", false);
            var columnsArray = ArgumentHelper.GetArray(arguments, "columns", false);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            if (rowsArray != null && rowsArray.Count > 0)
            {
                foreach (var rowNode in rowsArray)
                {
                    if (int.TryParse(rowNode?.ToString(), out int rowIndex))
                    {
                        // EPPlus 没有直接的 AutoFitRow，但可以设置为默认高度或使用内容计算
                        // 这里使用默认值
                        worksheet.Row(rowIndex).Height = 15; // 默认行高
                    }
                }
            }

            if (columnsArray != null && columnsArray.Count > 0)
            {
                foreach (var colNode in columnsArray)
                {
                    if (int.TryParse(colNode?.ToString(), out int colIndex))
                    {
                        worksheet.Column(colIndex).AutoFit();
                    }
                }
            }

            // 如果没有指定行列，则自动调整所有列
            if ((rowsArray == null || rowsArray.Count == 0) && (columnsArray == null || columnsArray.Count == 0))
            {
                if (worksheet.Dimension != null)
                {
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已自动调整大小 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> GetInfoAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var rowIndex = ArgumentHelper.GetIntNullable(arguments, "rowIndex");
            var columnIndex = ArgumentHelper.GetIntNullable(arguments, "columnIndex");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var result = new Dictionary<string, object>();

            if (rowIndex.HasValue)
            {
                var row = worksheet.Row(rowIndex.Value);
                result["row"] = new
                {
                    index = rowIndex.Value,
                    height = row.Height,
                    hidden = row.Hidden,
                    customHeight = row.CustomHeight
                };
            }

            if (columnIndex.HasValue)
            {
                var column = worksheet.Column(columnIndex.Value);
                result["column"] = new
                {
                    index = columnIndex.Value,
                    width = column.Width,
                    hidden = column.Hidden
                };
            }

            // 如果都没有指定，返回工作表信息
            if (!rowIndex.HasValue && !columnIndex.HasValue)
            {
                if (worksheet.Dimension != null)
                {
                    result["worksheet"] = new
                    {
                        name = worksheet.Name,
                        totalRows = worksheet.Dimension.Rows,
                        totalColumns = worksheet.Dimension.Columns
                    };
                }
            }

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }
}
