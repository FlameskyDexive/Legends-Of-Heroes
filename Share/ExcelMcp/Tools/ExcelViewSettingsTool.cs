using System.Text.Json;
using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 视图设置工具。
/// </summary>
public class ExcelViewSettingsTool : IExcelTool
{
    private const string OperationSet = "set";
    private const string OperationGet = "get";

    public string Description =>
        @"获取或更新工作表的视图设置，包括缩放、网格线、标题与视图模式。

使用示例:
- 设置: excel_view_settings(operation='set', path='book.xlsx', sheetIndex=0, zoom=140, showGridLines=false)
- 查询: excel_view_settings(operation='get', path='book.xlsx', sheetIndex=0)";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'set': 更新视图设置 (需要 path，可选 outputPath)
- 'get': 获取视图设置",
                // 2 operations
                // ReSharper disable once StringLiteralTypo
                @enum = new[] { OperationSet, OperationGet }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (set 操作可选，默认覆盖原文件)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (默认 0)"
            },
            zoom = new
            {
                type = "number",
                description = "缩放级别 (10-400，可选)"
            },
            showGridLines = new
            {
                type = "boolean",
                description = "是否显示网格线 (set 操作可选)"
            },
            showHeadings = new
            {
                type = "boolean",
                description = "是否显示行列标题 (set 操作可选)"
            },
            viewType = new
            {
                type = "string",
                description = "视图模式: normal, pagebreak, pagelayout",
                @enum = new[] { "normal", "pagebreak", "pagelayout" }
            }
        },
        required = new[] { "operation", "path" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation").ToLowerInvariant();
        var path = ArgumentHelper.GetAndValidatePath(arguments);
        var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);

        return operation switch
        {
            OperationSet => await SetViewSettingsAsync(path,
                ArgumentHelper.GetAndValidateOutputPath(arguments, path), sheetIndex, arguments),
            OperationGet => await GetViewSettingsAsync(path, sheetIndex),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> SetViewSettingsAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var view = worksheet.View;

            var zoom = ArgumentHelper.GetIntNullable(arguments, "zoom");
            if (zoom.HasValue)
                view.ZoomScale = Math.Clamp(zoom.Value, 10, 400);

            SetBoolIfProvided(arguments, "showGridLines", value => view.ShowGridLines = value);
            SetBoolIfProvided(arguments, "showHeadings", value => view.ShowHeaders = value);

            var viewType = ArgumentHelper.GetStringNullable(arguments, "viewType");
            if (!string.IsNullOrEmpty(viewType))
                SetViewType(view, viewType);

            package.SaveAs(new FileInfo(outputPath));
            return $"已更新工作表 {worksheet.Name} 的视图设置，输出: {outputPath}";
        });
    }

    private Task<string> GetViewSettingsAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var view = worksheet.View;
            var zoomScale = view.ZoomScale <= 0 ? 100 : view.ZoomScale;

            var result = new
            {
                worksheet = worksheet.Name,
                zoom = zoomScale,
                showGridLines = view.ShowGridLines,
                showHeadings = view.ShowHeaders,
                viewType = GetViewMode(view)
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private static void SetViewType(ExcelWorksheetView view, string viewType)
    {
        switch (viewType.ToLowerInvariant())
        {
            case "normal":
                view.PageBreakView = false;
                view.PageLayoutView = false;
                break;
            case "pagebreak":
                view.PageLayoutView = false;
                view.PageBreakView = true;
                break;
            case "pagelayout":
                view.PageBreakView = false;
                view.PageLayoutView = true;
                break;
            default:
                throw new ArgumentException($"不支持的视图类型: {viewType}");
        }
    }

    private static string GetViewMode(ExcelWorksheetView view)
    {
        if (view.PageLayoutView)
            return "PageLayout";
        if (view.PageBreakView)
            return "PageBreak";
        return "Normal";
    }

    private static void SetBoolIfProvided(JsonObject? arguments, string key, Action<bool> setter)
    {
        var value = ArgumentHelper.GetBoolNullable(arguments, key);
        if (value.HasValue)
            setter(value.Value);
    }
}
