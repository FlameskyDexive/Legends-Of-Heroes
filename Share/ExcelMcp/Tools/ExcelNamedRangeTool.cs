using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     命名范围工具 - 支持添加、删除、获取命名范围
/// </summary>
public class ExcelNamedRangeTool : IExcelTool
{
    public string Description => @"管理 Excel 命名范围。支持 3 种操作: add, delete, get。

使用示例:
- 添加命名范围: excel_named_range(operation='add', path='book.xlsx', name='MyRange', range='A1:C10')
- 带工作表引用: excel_named_range(operation='add', path='book.xlsx', name='MyRange', range='Sheet1!A1:C10')
- 删除命名范围: excel_named_range(operation='delete', path='book.xlsx', name='MyRange')
- 获取命名范围: excel_named_range(operation='get', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'add': 添加命名范围 (必需参数: path, name, range)
- 'delete': 删除命名范围 (必需参数: path, name)
- 'get': 获取所有命名范围 (必需参数: path)",
                @enum = new[] { "add", "delete", "get" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            name = new
            {
                type = "string",
                description = "范围名称。必须是有效的 Excel 名称 (用于 add/delete)"
            },
            range = new
            {
                type = "string",
                description = "单元格范围 (例如 'A1:C10' 或 'Sheet1!A1:C10', 用于 add)"
            },
            comment = new
            {
                type = "string",
                description = "命名范围的注释 (可选, 用于 add)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 可选, 默认: 0). 当范围不包含工作表引用时使用"
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
        var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);
        var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);

        return operation.ToLower() switch
        {
            "add" => await AddNamedRangeAsync(path, outputPath, sheetIndex, arguments),
            "delete" => await DeleteNamedRangeAsync(path, outputPath, arguments),
            "get" => await GetNamedRangesAsync(path),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> AddNamedRangeAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var name = ArgumentHelper.GetString(arguments, "name");
            var rangeAddress = ArgumentHelper.GetString(arguments, "range");
            var comment = ArgumentHelper.GetStringNullable(arguments, "comment");

            using var package = new ExcelPackage(new FileInfo(path));
            var names = package.Workbook.Names;

            // 检查名称是否已存在 - 使用 ContainsKey 而不是直接索引
            if (names.ContainsKey(name))
                throw new ArgumentException($"命名范围 '{name}' 已存在。");

            try
            {
                ExcelRangeBase rangeObj;

                if (rangeAddress.Contains('!'))
                {
                    // 包含工作表引用
                    var parts = rangeAddress.Split('!');
                    var sheetName = parts[0].Trim().Trim('\'');
                    var cellRange = parts[1].Trim();

                    var worksheet = package.Workbook.Worksheets[sheetName];
                    if (worksheet == null)
                        throw new ArgumentException($"工作表 '{sheetName}' 不存在。");

                    rangeObj = worksheet.Cells[cellRange];
                }
                else
                {
                    // 使用指定的工作表索引
                    var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
                    rangeObj = worksheet.Cells[rangeAddress];
                }

                // 添加命名范围
                var namedRange = names.Add(name, rangeObj);

                // EPPlus 不直接支持命名范围的注释，但我们可以将其作为信息返回

                package.SaveAs(new FileInfo(outputPath));

                return $"命名范围 '{name}' 已添加 (引用: {namedRange.Address}). 输出: {outputPath}";
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"创建命名范围 '{name}' 失败，范围 '{rangeAddress}': {ex.Message}", ex);
            }
        });
    }

    private Task<string> DeleteNamedRangeAsync(string path, string outputPath, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var name = ArgumentHelper.GetString(arguments, "name");

            using var package = new ExcelPackage(new FileInfo(path));
            var names = package.Workbook.Names;

            // 检查名称是否存在 - 使用 ContainsKey 而不是直接索引
            if (!names.ContainsKey(name))
                throw new ArgumentException($"命名范围 '{name}' 不存在。");

            names.Remove(name);
            package.SaveAs(new FileInfo(outputPath));

            return $"命名范围 '{name}' 已删除. 输出: {outputPath}";
        });
    }

    private Task<string> GetNamedRangesAsync(string path)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var names = package.Workbook.Names;

            if (names.Count == 0)
            {
                var emptyResult = new
                {
                    count = 0,
                    items = Array.Empty<object>(),
                    message = "未找到命名范围"
                };
                return JsonSerializer.Serialize(emptyResult, new JsonSerializerOptions { WriteIndented = true });
            }

            var nameList = new List<object>();
            for (var i = 0; i < names.Count; i++)
            {
                var namedRange = names[i];
                nameList.Add(new
                {
                    index = i,
                    name = namedRange.Name,
                    reference = namedRange.Address,
                    isHidden = namedRange.IsNameHidden
                });
            }

            var result = new
            {
                count = names.Count,
                items = nameList
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }
}
