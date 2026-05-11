using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq;
using ET;
using OfficeOpenXml;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 批注工具 - 支持添加、编辑、删除、查询批注。
/// </summary>
public class ExcelCommentTool : IExcelTool
{
    private const string DefaultAuthor = "EpplusMCP";
    private static readonly Regex CellAddressRegex = new(@"^[A-Za-z]{1,3}\d+$", RegexOptions.Compiled);

    public string Description => @"管理 Excel 批注。支持 4 种操作: add, edit, delete, get。

使用示例:
- 添加批注: excel_comment(operation='add', path='book.xlsx', cell='A1', comment='这是批注')
- 编辑批注: excel_comment(operation='edit', path='book.xlsx', cell='A1', comment='更新后的内容')
- 删除批注: excel_comment(operation='delete', path='book.xlsx', cell='A1')
- 获取批注: excel_comment(operation='get', path='book.xlsx', cell='A1')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'add': 添加批注 (必需: path, cell, comment)
- 'edit': 编辑批注 (必需: path, cell, comment)
- 'delete': 删除批注 (必需: path, cell)
- 'get': 获取批注信息 (必需: path, 可选: cell)",
                @enum = new[] { "add", "edit", "delete", "get" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (add/edit/delete 可选, 默认为输入文件)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 默认0)"
            },
            cell = new
            {
                type = "string",
                description = "单元格地址 (如 'A1')"
            },
            comment = new
            {
                type = "string",
                description = "批注内容 (add/edit 操作必需)"
            },
            author = new
            {
                type = "string",
                description = "批注作者 (可选, 默认 EpplusMCP)"
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
            "add" => await AddCommentAsync(path, outputPath, sheetIndex, arguments),
            "edit" => await EditCommentAsync(path, outputPath, sheetIndex, arguments),
            "delete" => await DeleteCommentAsync(path, outputPath, sheetIndex, arguments),
            "get" => await GetCommentsAsync(path, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> AddCommentAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = GetCellAddress(arguments);
            var commentText = ArgumentHelper.GetString(arguments, "comment");
            var author = ArgumentHelper.GetStringNullable(arguments, "author") ?? DefaultAuthor;

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var range = worksheet.Cells[cell];

            if (range.Comment != null)
                throw new InvalidOperationException($"单元格 {cell} 已包含批注, 请使用 edit 操作。");

            range.AddComment(commentText, author);
            package.SaveAs(new FileInfo(outputPath));

            return $"已在单元格 {cell} 添加批注 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> EditCommentAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = GetCellAddress(arguments);
            var commentText = ArgumentHelper.GetString(arguments, "comment");
            var author = ArgumentHelper.GetStringNullable(arguments, "author");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var range = worksheet.Cells[cell];
            var comment = range.Comment;

            if (comment == null)
                throw new ArgumentException($"单元格 {cell} 没有批注可供编辑。");

            comment.Text = commentText;
            if (!string.IsNullOrWhiteSpace(author))
                comment.Author = author;

            package.SaveAs(new FileInfo(outputPath));
            return $"已更新单元格 {cell} 的批注 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> DeleteCommentAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = GetCellAddress(arguments);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var range = worksheet.Cells[cell];
            var comment = range.Comment;

            if (comment == null)
                throw new ArgumentException($"单元格 {cell} 不存在批注。");

            worksheet.Comments.Remove(comment);
            package.SaveAs(new FileInfo(outputPath));

            return $"已删除单元格 {cell} 的批注 (工作表 {sheetIndex}). 输出: {outputPath}";
        });
    }

    private Task<string> GetCommentsAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetStringNullable(arguments, "cell");
            if (!string.IsNullOrWhiteSpace(cell))
                ValidateCellAddress(cell);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            if (!string.IsNullOrWhiteSpace(cell))
            {
                var range = worksheet.Cells[cell];
                var comment = range.Comment;

                if (comment == null)
                {
                    var emptyResult = new
                    {
                        count = 0,
                        sheetIndex,
                        cell,
                        items = Array.Empty<object>(),
                        message = $"单元格 {cell} 未找到批注"
                    };
                    return JsonSerializer.Serialize(emptyResult, new JsonSerializerOptions { WriteIndented = true });
                }

                var singleResult = new
                {
                    count = 1,
                    sheetIndex,
                    cell,
                    items = new[]
                    {
                        new
                        {
                            cell,
                            author = comment.Author,
                            note = comment.Text
                        }
                    }
                };
                return JsonSerializer.Serialize(singleResult, new JsonSerializerOptions { WriteIndented = true });
            }

            var comments = worksheet.Comments;
            if (comments.Count == 0)
            {
                var empty = new
                {
                    count = 0,
                    sheetIndex,
                    items = Array.Empty<object>(),
                    message = "未找到批注"
                };
                return JsonSerializer.Serialize(empty, new JsonSerializerOptions { WriteIndented = true });
            }

            var commentList = new List<object>();
            foreach (ExcelComment comment in comments)
            {
                commentList.Add(new
                {
                    cell = comment.Address,
                    author = comment.Author,
                    note = comment.Text
                });
            }

            var result = new
            {
                count = comments.Count,
                sheetIndex,
                items = commentList
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private static string GetCellAddress(JsonObject? arguments)
    {
        var cell = ArgumentHelper.GetString(arguments, "cell");
        ValidateCellAddress(cell);
        return cell;
    }

    private static void ValidateCellAddress(string cell)
    {
        if (!CellAddressRegex.IsMatch(cell))
            throw new ArgumentException($"无效的单元格地址: {cell}");
    }
}
