using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using System.Linq;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 超链接工具 - 支持添加、编辑、删除、获取超链接。
/// </summary>
public class ExcelHyperlinkTool : IExcelTool
{
    public string Description => @"管理 Excel 超链接。支持 4 种操作: add, edit, delete, get。

使用示例:
- 添加超链接: excel_hyperlink(operation='add', path='book.xlsx', cell='A1', url='https://example.com', displayText='点击')
- 编辑超链接: excel_hyperlink(operation='edit', path='book.xlsx', cell='A1', url='https://new.com')
- 删除超链接: excel_hyperlink(operation='delete', path='book.xlsx', cell='A1')
- 获取超链接: excel_hyperlink(operation='get', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'add': 添加超链接 (必需: path, cell, url)
- 'edit': 编辑超链接 (必需: path, cell 或 hyperlinkIndex)
- 'delete': 删除超链接 (必需: path, cell 或 hyperlinkIndex)
- 'get': 获取所有超链接 (必需: path)",
                @enum = new[] { "add", "edit", "delete", "get" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
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
                description = "单元格地址 (例如 'A1')"
            },
            url = new
            {
                type = "string",
                description = "链接地址 (add 必需, edit 可选)"
            },
            displayText = new
            {
                type = "string",
                description = "显示文本 (可选)"
            },
            hyperlinkIndex = new
            {
                type = "number",
                description = "超链接索引 (基于当前工作表的排序, edit/delete 可用)"
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
            "add" => await AddHyperlinkAsync(path, outputPath, sheetIndex, arguments),
            "edit" => await EditHyperlinkAsync(path, outputPath, sheetIndex, arguments),
            "delete" => await DeleteHyperlinkAsync(path, outputPath, sheetIndex, arguments),
            "get" => await GetHyperlinksAsync(path, sheetIndex),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> AddHyperlinkAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetString(arguments, "cell");
            var url = ArgumentHelper.GetString(arguments, "url");
            var displayText = ArgumentHelper.GetStringNullable(arguments, "displayText");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var range = worksheet.Cells[cell];

            if (range == null)
                throw new ArgumentException($"无法定位单元格 {cell}");

            if (range.Hyperlink != null)
                throw new ArgumentException($"单元格 {cell} 已存在超链接, 请使用 edit 操作。");

            range.Hyperlink = CreateHyperlink(url);
            if (!string.IsNullOrEmpty(displayText))
                range.Value = displayText;
            else if (range.Value == null)
                range.Value = url;

            package.SaveAs(new FileInfo(outputPath));
            return $"已在单元格 {cell} 添加超链接: {url}. 输出: {outputPath}";
        });
    }

    private Task<string> EditHyperlinkAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var hyperlinkIndex = ArgumentHelper.GetIntNullable(arguments, "hyperlinkIndex");
            var cell = ArgumentHelper.GetStringNullable(arguments, "cell");
            var url = ArgumentHelper.GetStringNullable(arguments, "url");
            var displayText = ArgumentHelper.GetStringNullable(arguments, "displayText");

            if (!hyperlinkIndex.HasValue && string.IsNullOrWhiteSpace(cell))
                throw new ArgumentException("edit 操作需要提供 'cell' 或 'hyperlinkIndex'。");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var targetCell = hyperlinkIndex.HasValue
                ? GetHyperlinkCells(worksheet).ElementAtOrDefault(hyperlinkIndex.Value)
                : worksheet.Cells[cell!];

            if (targetCell == null || targetCell.Hyperlink == null)
                throw new ArgumentException("未找到指定的超链接。");

            var changes = new List<string>();
            if (!string.IsNullOrWhiteSpace(url))
            {
                targetCell.Hyperlink = CreateHyperlink(url);
                changes.Add($"url={url}");
                if (targetCell.Value == null)
                    targetCell.Value = url;
            }

            if (!string.IsNullOrWhiteSpace(displayText))
            {
                targetCell.Value = displayText;
                changes.Add($"displayText={displayText}");
            }

            package.SaveAs(new FileInfo(outputPath));
            var info = changes.Count == 0 ? "未做任何修改" : string.Join(", ", changes);
            return $"已更新单元格 {targetCell.Address} 的超链接 ({info}). 输出: {outputPath}";
        });
    }

    private Task<string> DeleteHyperlinkAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var hyperlinkIndex = ArgumentHelper.GetIntNullable(arguments, "hyperlinkIndex");
            var cell = ArgumentHelper.GetStringNullable(arguments, "cell");

            if (!hyperlinkIndex.HasValue && string.IsNullOrWhiteSpace(cell))
                throw new ArgumentException("delete 操作需要提供 'cell' 或 'hyperlinkIndex'。");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var targetCell = hyperlinkIndex.HasValue
                ? GetHyperlinkCells(worksheet).ElementAtOrDefault(hyperlinkIndex.Value)
                : worksheet.Cells[cell!];

            if (targetCell == null || targetCell.Hyperlink == null)
                throw new ArgumentException("未找到指定的超链接。");

            var address = targetCell.Address;
            targetCell.Hyperlink = null;

            package.SaveAs(new FileInfo(outputPath));
            return $"已删除单元格 {address} 的超链接. 输出: {outputPath}";
        });
    }

    private Task<string> GetHyperlinksAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var hyperlinkCells = GetHyperlinkCells(worksheet);
            if (hyperlinkCells.Count == 0)
            {
                var empty = new
                {
                    count = 0,
                    worksheetName = worksheet.Name,
                    items = Array.Empty<object>(),
                    message = "未找到超链接"
                };
                return JsonSerializer.Serialize(empty, new JsonSerializerOptions { WriteIndented = true });
            }

            var items = hyperlinkCells
                .Select((cell, index) => new
                {
                    index,
                    cell = cell.Address,
                    url = cell.Hyperlink is Uri uri ? uri.ToString() : cell.Hyperlink?.ToString(),
                    displayText = cell.Text
                }).ToList();

            var result = new
            {
                count = items.Count,
                worksheetName = worksheet.Name,
                items
            };
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private static ExcelHyperLink? CreateHyperlink(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var absolute))
            return new ExcelHyperLink(absolute.ToString());

        if (Uri.TryCreate(url, UriKind.Relative, out var relative))
            return new ExcelHyperLink(relative.ToString());

        return new ExcelHyperLink(url);
    }

    private static List<ExcelRangeBase> GetHyperlinkCells(ExcelWorksheet worksheet)
    {
        var result = new List<ExcelRangeBase>();
        if (worksheet.Dimension == null)
            return result;

        foreach (var cell in worksheet.Cells[worksheet.Dimension.Address])
        {
            if (cell.Hyperlink != null)
                result.Add(cell);
        }

        return result
            .OrderBy(c => c.Start.Row)
            .ThenBy(c => c.Start.Column)
            .ToList();
    }
}
