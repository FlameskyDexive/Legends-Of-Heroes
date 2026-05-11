using System.Text.Json.Nodes;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 工作表操作工具 (create, delete, rename, list)
/// </summary>
public class ExcelSheetTool : IExcelTool
{
    public string Description => @"Excel 工作表操作。支持 4 种操作: create, delete, rename, list。

使用示例:
- 创建工作表: excel_sheet(operation='create', path='book.xlsx', sheetName='NewSheet')
- 删除工作表: excel_sheet(operation='delete', path='book.xlsx', sheetIndex=1)
- 重命名工作表: excel_sheet(operation='rename', path='book.xlsx', sheetIndex=0, newName='Renamed')
- 列出工作表: excel_sheet(operation='list', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'create': 创建新工作表 (必需参数: path, sheetName)
- 'delete': 删除工作表 (必需参数: path, sheetIndex)
- 'rename': 重命名工作表 (必需参数: path, sheetIndex, newName)
- 'list': 列出所有工作表 (必需参数: path)",
                @enum = new[] { "create", "delete", "rename", "list" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
            },
            sheetName = new
            {
                type = "string",
                description = "工作表名称 (create操作必需)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, delete/rename操作必需)"
            },
            newName = new
            {
                type = "string",
                description = "新工作表名称 (rename操作必需)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 默认为输入路径)"
            }
        },
        required = new[] { "operation", "path" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation");
        var path = ArgumentHelper.GetAndValidatePath(arguments);

        return operation.ToLower() switch
        {
            "create" => await CreateSheetAsync(path, arguments),
            "delete" => await DeleteSheetAsync(path, arguments),
            "rename" => await RenameSheetAsync(path, arguments),
            "list" => await ListSheetsAsync(path),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> CreateSheetAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetName = ArgumentHelper.GetString(arguments, "sheetName");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            package.Workbook.Worksheets.Add(sheetName);

            package.SaveAs(new FileInfo(outputPath));
            return $"工作表 '{sheetName}' 已创建. 输出: {outputPath}";
        });
    }

    private Task<string> DeleteSheetAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));

            if (sheetIndex < 0 || sheetIndex >= package.Workbook.Worksheets.Count)
                throw new ArgumentException($"无效的工作表索引: {sheetIndex}");

            var sheetName = package.Workbook.Worksheets[sheetIndex].Name;
            package.Workbook.Worksheets.Delete(sheetIndex);

            package.SaveAs(new FileInfo(outputPath));
            return $"工作表 '{sheetName}' (索引{sheetIndex}) 已删除. 输出: {outputPath}";
        });
    }

    private Task<string> RenameSheetAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex");
            var newName = ArgumentHelper.GetString(arguments, "newName");
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));

            if (sheetIndex < 0 || sheetIndex >= package.Workbook.Worksheets.Count)
                throw new ArgumentException($"无效的工作表索引: {sheetIndex}");

            var oldName = package.Workbook.Worksheets[sheetIndex].Name;
            package.Workbook.Worksheets[sheetIndex].Name = newName;

            package.SaveAs(new FileInfo(outputPath));
            return $"工作表 '{oldName}' 已重命名为 '{newName}'. 输出: {outputPath}";
        });
    }

    private Task<string> ListSheetsAsync(string path)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));

            var sheets = package.Workbook.Worksheets
                .Select((ws, index) => new
                {
                    index,
                    name = ws.Name,
                    rowCount = ws.Dimension?.Rows ?? 0,
                    columnCount = ws.Dimension?.Columns ?? 0
                })
                .ToArray();

            return System.Text.Json.JsonSerializer.Serialize(sheets, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        });
    }
}
