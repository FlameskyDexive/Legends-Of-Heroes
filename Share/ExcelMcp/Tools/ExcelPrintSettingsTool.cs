using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 打印设置工具。
/// </summary>
public class ExcelPrintSettingsTool : IExcelTool
{
    private const string OperationPageSetup = "page_setup";
    private const string OperationHeaderFooter = "header_footer";
    private const string OperationPrintArea = "print_area";
    private const string OperationGet = "get";

    private static readonly Dictionary<string, ePaperSize> PaperSizes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["letter"] = ePaperSize.Letter,
        ["legal"] = ePaperSize.Legal,
        ["a3"] = ePaperSize.A3,
        ["a4"] = ePaperSize.A4,
        ["a5"] = ePaperSize.A5,
        ["b4"] = ePaperSize.B4,
        ["b5"] = ePaperSize.B5,
        ["executive"] = ePaperSize.Executive,
        ["tabloid"] = ePaperSize.Tabloid,
        ["statement"] = ePaperSize.Statement
    };

    public string Description =>
        @"管理工作表的打印设置，包括页面方向/纸张、页眉页脚、打印区域等。

使用示例:
- 页面: excel_print_settings(operation='page_setup', path='book.xlsx', orientation='landscape', paperSize='A4')
- 页眉页脚: excel_print_settings(operation='header_footer', path='book.xlsx', header={""center"":""销售报告""})
- 打印区域: excel_print_settings(operation='print_area', path='book.xlsx', range='A1:D50')
- 获取: excel_print_settings(operation='get', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作:
- 'page_setup': 配置页面方向、纸张、缩放等
- 'header_footer': 设置页眉页脚文本
- 'print_area': 设置或清除打印区域
- 'get': 获取当前打印设置",
                @enum = new[] { OperationPageSetup, OperationHeaderFooter, OperationPrintArea, OperationGet }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (写操作可选, 默认为输入文件)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (默认0)"
            },
            orientation = new
            {
                type = "string",
                description = "portrait 或 landscape (page_setup 可选)",
                @enum = new[] { "portrait", "landscape" }
            },
            paperSize = new
            {
                type = "string",
                description = "纸张大小 (如 A4、Letter，page_setup 可选)"
            },
            scale = new
            {
                type = "number",
                description = "缩放百分比 10-400 (page_setup 可选)"
            },
            fitToPagesWide = new
            {
                type = "number",
                description = "宽度页数 (page_setup 可选)"
            },
            fitToPagesTall = new
            {
                type = "number",
                description = "高度页数 (page_setup 可选)"
            },
            fitToPage = new
            {
                type = "boolean",
                description = "是否启用按页适应 (page_setup 可选)"
            },
            margins = new
            {
                type = "object",
                description = "页边距 (top/bottom/left/right/header/footer, 单位英寸)"
            },
            header = new
            {
                type = "object",
                description = "页眉文本 (header_footer 操作可选)"
            },
            footer = new
            {
                type = "object",
                description = "页脚文本 (header_footer 操作可选)"
            },
            range = new
            {
                type = "string",
                description = "打印区域 (print_area 操作可选)"
            },
            clear = new
            {
                type = "boolean",
                description = "是否清除打印区域 (print_area 操作可选)"
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
            OperationPageSetup => await ConfigurePageSetupAsync(path,
                ArgumentHelper.GetAndValidateOutputPath(arguments, path), sheetIndex, arguments),
            OperationHeaderFooter => await ConfigureHeaderFooterAsync(path,
                ArgumentHelper.GetAndValidateOutputPath(arguments, path), sheetIndex, arguments),
            OperationPrintArea => await ConfigurePrintAreaAsync(path,
                ArgumentHelper.GetAndValidateOutputPath(arguments, path), sheetIndex, arguments),
            OperationGet => await GetPrintSettingsAsync(path, sheetIndex),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> ConfigurePageSetupAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var settings = worksheet.PrinterSettings;

            var orientation = ArgumentHelper.GetStringNullable(arguments, "orientation");
            if (!string.IsNullOrEmpty(orientation))
                settings.Orientation = orientation.ToLowerInvariant() switch
                {
                    "portrait" => eOrientation.Portrait,
                    "landscape" => eOrientation.Landscape,
                    _ => throw new ArgumentException($"未知页面方向: {orientation}")
                };

            var paperSize = ArgumentHelper.GetStringNullable(arguments, "paperSize");
            if (!string.IsNullOrEmpty(paperSize))
                settings.PaperSize = ParsePaperSize(paperSize);

            var scale = ArgumentHelper.GetIntNullable(arguments, "scale");
            if (scale.HasValue)
                settings.Scale = (int)Math.Clamp(scale.Value, 10, 400);

            var fitToPage = ArgumentHelper.GetBoolNullable(arguments, "fitToPage");
            if (fitToPage.HasValue)
                settings.FitToPage = fitToPage.Value;

            var fitWide = ArgumentHelper.GetIntNullable(arguments, "fitToPagesWide");
            if (fitWide.HasValue)
            {
                settings.FitToWidth = Math.Max(0, fitWide.Value);
                settings.FitToPage = true;
            }

            var fitTall = ArgumentHelper.GetIntNullable(arguments, "fitToPagesTall");
            if (fitTall.HasValue)
            {
                settings.FitToHeight = Math.Max(0, fitTall.Value);
                settings.FitToPage = true;
            }

            var margins = arguments?["margins"] as JsonObject;
            if (margins != null)
            {
                settings.TopMargin = GetMarginOrDefault(margins, "top", settings.TopMargin);
                settings.BottomMargin = GetMarginOrDefault(margins, "bottom", settings.BottomMargin);
                settings.LeftMargin = GetMarginOrDefault(margins, "left", settings.LeftMargin);
                settings.RightMargin = GetMarginOrDefault(margins, "right", settings.RightMargin);
                settings.HeaderMargin = GetMarginOrDefault(margins, "header", settings.HeaderMargin);
                settings.FooterMargin = GetMarginOrDefault(margins, "footer", settings.FooterMargin);
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已更新工作表 {worksheet.Name} 的页面设置，输出: {outputPath}";
        });
    }

    private Task<string> ConfigureHeaderFooterAsync(string path, string outputPath, int sheetIndex,
        JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var headerFooter = worksheet.HeaderFooter;

            var header = arguments?["header"] as JsonObject;
            if (header != null)
            {
                SetHeaderFooterSection(headerFooter.OddHeader, header);
                SetHeaderFooterSection(headerFooter.EvenHeader, header);
            }

            var footer = arguments?["footer"] as JsonObject;
            if (footer != null)
            {
                SetHeaderFooterSection(headerFooter.OddFooter, footer);
                SetHeaderFooterSection(headerFooter.EvenFooter, footer);
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"已更新工作表 {worksheet.Name} 的页眉页脚，输出: {outputPath}";
        });
    }

    private Task<string> ConfigurePrintAreaAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var clear = ArgumentHelper.GetBoolNullable(arguments, "clear") ?? false;

            if (clear)
            {
                worksheet.PrinterSettings.PrintArea = null;
            }
            else
            {
                var range = ArgumentHelper.GetString(arguments, "range", "range");
                worksheet.PrinterSettings.PrintArea = worksheet.Cells[range];
            }

            package.SaveAs(new FileInfo(outputPath));
            return clear
                ? $"已清除工作表 {worksheet.Name} 的打印区域，输出: {outputPath}"
                : $"已设置工作表 {worksheet.Name} 的打印区域，输出: {outputPath}";
        });
    }

    private Task<string> GetPrintSettingsAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var settings = worksheet.PrinterSettings;
            var headerFooter = worksheet.HeaderFooter;
            var fitWidth = settings.FitToWidth < 0 ? 0 : settings.FitToWidth;
            var fitHeight = settings.FitToHeight < 0 ? 0 : settings.FitToHeight;

            var result = new
            {
                worksheet = worksheet.Name,
                orientation = settings.Orientation.ToString(),
                paperSize = settings.PaperSize.ToString(),
                scale = settings.Scale,
                fitToWidth = fitWidth,
                fitToHeight = fitHeight,
                fitToPage = settings.FitToPage,
                printArea = settings.PrintArea?.Address,
                margins = new
                {
                    top = settings.TopMargin,
                    bottom = settings.BottomMargin,
                    left = settings.LeftMargin,
                    right = settings.RightMargin,
                    header = settings.HeaderMargin,
                    footer = settings.FooterMargin
                },
                header = new
                {
                    left = headerFooter.OddHeader.LeftAlignedText,
                    center = headerFooter.OddHeader.CenteredText,
                    right = headerFooter.OddHeader.RightAlignedText
                },
                footer = new
                {
                    left = headerFooter.OddFooter.LeftAlignedText,
                    center = headerFooter.OddFooter.CenteredText,
                    right = headerFooter.OddFooter.RightAlignedText
                }
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            });
        });
    }

    private static ePaperSize ParsePaperSize(string paperSize)
    {
        if (PaperSizes.TryGetValue(paperSize, out var value))
            return value;
        throw new ArgumentException($"未知纸张大小: {paperSize}");
    }

    private static void SetHeaderFooterSection(object section, JsonObject config)
    {
        if (section == null)
            return;

        UpdateHeaderFooterProperty(section, "LeftAlignedText", config["left"]);
        UpdateHeaderFooterProperty(section, "CenteredText", config["center"]);
        UpdateHeaderFooterProperty(section, "RightAlignedText", config["right"]);
    }

    private static void UpdateHeaderFooterProperty(object section, string propertyName, JsonNode? node)
    {
        if (node == null)
            return;

        var property = section.GetType().GetProperty(propertyName);
        if (property == null || !property.CanWrite)
            return;

        property.SetValue(section, node.GetValue<string>());
    }

    private static decimal GetMarginOrDefault(JsonObject margins, string key, decimal currentValue)
    {
        var value = margins[key];
        if (value == null)
            return currentValue;

        return value.GetValueKind() switch
        {
            JsonValueKind.Number => value.GetValue<decimal>(),
            JsonValueKind.String when decimal.TryParse(value.GetValue<string>(), NumberStyles.Any,
                CultureInfo.InvariantCulture, out decimal parsed) => parsed,
            _ => throw new ArgumentException($"margins.{key} 必须为数字")
        };
    }
}
