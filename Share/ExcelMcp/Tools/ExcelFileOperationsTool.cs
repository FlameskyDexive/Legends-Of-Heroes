using System.Text.Json.Nodes;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 文件操作工具 (create, convert)
/// </summary>
public class ExcelFileOperationsTool : IExcelTool
{
    public string Description => @"Excel 文件操作。支持 2 种操作: create, convert。

使用示例:
- 创建工作簿: excel_file_operations(operation='create', path='new.xlsx')
- 转换格式: excel_file_operations(operation='convert', inputPath='book.xlsx', outputPath='book.csv', format='csv')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'create': 创建新工作簿 (必需参数: path 或 outputPath)
- 'convert': 转换工作簿格式 (必需参数: inputPath, outputPath, format)",
                @enum = new[] { "create", "convert" }
            },
            path = new
            {
                type = "string",
                description = "文件路径 (create操作的输出路径)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 用于create, 也可使用path)"
            },
            inputPath = new
            {
                type = "string",
                description = "输入文件路径 (convert操作必需)"
            },
            sheetName = new
            {
                type = "string",
                description = "初始工作表名称 (可选, 用于create)"
            },
            format = new
            {
                type = "string",
                description = "输出格式: 'csv', 'html', 'txt' (convert操作必需)",
                @enum = new[] { "csv", "html", "txt" }
            }
        },
        required = new[] { "operation" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation");

        return operation.ToLower() switch
        {
            "create" => await CreateWorkbookAsync(arguments),
            "convert" => await ConvertWorkbookAsync(arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> CreateWorkbookAsync(JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var path = ArgumentHelper.GetString(arguments, "path", "outputPath", "path 或 outputPath");
            var sheetName = ArgumentHelper.GetStringNullable(arguments, "sheetName");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName ?? "Sheet1");

            package.SaveAs(new FileInfo(path));
            return $"Excel 工作簿已创建. 输出: {path}";
        });
    }

    private Task<string> ConvertWorkbookAsync(JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var inputPath = ArgumentHelper.GetAndValidatePath(arguments, "inputPath");
            var outputPath = ArgumentHelper.GetString(arguments, "outputPath");
            var format = ArgumentHelper.GetString(arguments, "format").ToLower();

            using var package = new ExcelPackage(new FileInfo(inputPath));

            // EPPlus 主要支持 CSV 导出,其他格式需要手动实现
            if (format == "csv")
            {
                var worksheet = package.Workbook.Worksheets[0];
                using var writer = new StreamWriter(outputPath);

                for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                {
                    var values = new List<string>();
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        var cell = worksheet.Cells[row, col];
                        values.Add(cell.Text ?? "");
                    }
                    writer.WriteLine(string.Join(",", values.Select(v => $"\"{v}\"")));
                }

                return $"工作簿已转换为 {format} 格式. 输出: {outputPath}";
            }
            else if (format == "html" || format == "txt")
            {
                throw new NotImplementedException($"格式 '{format}' 暂不支持. 请使用 'csv'");
            }
            else
            {
                throw new ArgumentException($"不支持的格式: {format}");
            }
        });
    }
}
