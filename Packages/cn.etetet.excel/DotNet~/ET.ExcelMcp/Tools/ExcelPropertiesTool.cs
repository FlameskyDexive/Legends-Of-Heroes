using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using ET;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 文档/工作表属性管理工具。
/// </summary>
public class ExcelPropertiesTool : IExcelTool
{
    private const string OperationGetWorkbookProperties = "get_workbook_properties";
    private const string OperationSetWorkbookProperties = "set_workbook_properties";
    private const string OperationGetSheetProperties = "get_sheet_properties";
    private const string OperationEditSheetProperties = "edit_sheet_properties";
    private const string OperationGetSheetInfo = "get_sheet_info";

    public string Description =>
        $@"管理 Excel 文档与工作表属性。支持 5 种操作: {OperationGetWorkbookProperties}, {OperationSetWorkbookProperties}, {OperationGetSheetProperties}, {OperationEditSheetProperties}, {OperationGetSheetInfo}。

使用示例:
- 获取工作簿属性: excel_properties(operation='{OperationGetWorkbookProperties}', path='book.xlsx')
- 设置工作簿属性: excel_properties(operation='{OperationSetWorkbookProperties}', path='book.xlsx', title='月报', author='Alice')
- 获取工作表属性: excel_properties(operation='{OperationGetSheetProperties}', path='book.xlsx', sheetIndex=0)
- 编辑工作表属性: excel_properties(operation='{OperationEditSheetProperties}', path='book.xlsx', sheetIndex=0, name='Summary', tabColor='#FF0000')
- 获取工作表概览: excel_properties(operation='{OperationGetSheetInfo}', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = $@"要执行的操作。
- '{OperationGetWorkbookProperties}': 获取工作簿文档属性
- '{OperationSetWorkbookProperties}': 设置工作簿文档属性
- '{OperationGetSheetProperties}': 获取指定工作表属性
- '{OperationEditSheetProperties}': 编辑指定工作表属性
- '{OperationGetSheetInfo}': 获取所有或单个工作表概览",
                @enum = new[]
                {
                    OperationGetWorkbookProperties, OperationSetWorkbookProperties,
                    OperationGetSheetProperties, OperationEditSheetProperties, OperationGetSheetInfo
                }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (写操作可选, 默认为输入文件)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 针对工作表操作必需)"
            },
            title = new
            {
                type = "string",
                description = "标题 (set_workbook_properties 可选)"
            },
            subject = new
            {
                type = "string",
                description = "主题 (set_workbook_properties 可选)"
            },
            author = new
            {
                type = "string",
                description = "作者 (set_workbook_properties 可选)"
            },
            keywords = new
            {
                type = "string",
                description = "关键字 (set_workbook_properties 可选)"
            },
            comments = new
            {
                type = "string",
                description = "备注 (set_workbook_properties 可选)"
            },
            category = new
            {
                type = "string",
                description = "类别 (set_workbook_properties 可选)"
            },
            company = new
            {
                type = "string",
                description = "公司 (set_workbook_properties 可选)"
            },
            manager = new
            {
                type = "string",
                description = "经理 (set_workbook_properties 可选)"
            },
            customProperties = new
            {
                type = "object",
                description = "自定义属性键值对 (set_workbook_properties 可选)"
            },
            name = new
            {
                type = "string",
                description = "工作表名称 (edit_sheet_properties 可选)"
            },
            isVisible = new
            {
                type = "boolean",
                description = "是否可见 (edit_sheet_properties 可选)"
            },
            tabColor = new
            {
                type = "string",
                description = "工作表标签颜色 (edit_sheet_properties 可选, 例如 #FF0000)"
            },
            isSelected = new
            {
                type = "boolean",
                description = "是否设为选中工作表 (edit_sheet_properties 可选)"
            }
        },
        required = new[] { "operation", "path" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation").ToLowerInvariant();
        var path = ArgumentHelper.GetAndValidatePath(arguments);
        var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

        return operation switch
        {
            OperationGetWorkbookProperties => await GetWorkbookPropertiesAsync(path),
            OperationSetWorkbookProperties => await SetWorkbookPropertiesAsync(path, outputPath, arguments),
            OperationGetSheetProperties => await GetSheetPropertiesAsync(path,
                ArgumentHelper.GetInt(arguments, "sheetIndex", "sheetIndex")),
            OperationEditSheetProperties => await EditSheetPropertiesAsync(path, outputPath,
                ArgumentHelper.GetInt(arguments, "sheetIndex", "sheetIndex"), arguments),
            OperationGetSheetInfo => await GetSheetInfoAsync(path, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> GetWorkbookPropertiesAsync(string path)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var props = package.Workbook.Properties;
            var result = new
            {
                title = props.Title,
                subject = props.Subject,
                author = props.Author,
                keywords = props.Keywords,
                comments = props.Comments,
                category = props.Category,
                company = props.Company,
                manager = props.Manager,
                status = props.Status,
                hyperlinkBase = GetHyperlinkBaseSafe(props),
                created = props.Created.ToString("o"),
                modified = props.Modified.ToString("o"),
                lastModifiedBy = props.LastModifiedBy,
                totalSheets = package.Workbook.Worksheets.Count,
                customProperties = ExtractCustomProperties(props)
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> SetWorkbookPropertiesAsync(string path, string outputPath, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var props = package.Workbook.Properties;

            props.Title = ArgumentHelper.GetStringNullable(arguments, "title") ?? props.Title;
            props.Subject = ArgumentHelper.GetStringNullable(arguments, "subject") ?? props.Subject;
            props.Author = ArgumentHelper.GetStringNullable(arguments, "author") ?? props.Author;
            props.Keywords = ArgumentHelper.GetStringNullable(arguments, "keywords") ?? props.Keywords;
            props.Comments = ArgumentHelper.GetStringNullable(arguments, "comments") ?? props.Comments;
            props.Category = ArgumentHelper.GetStringNullable(arguments, "category") ?? props.Category;
            props.Company = ArgumentHelper.GetStringNullable(arguments, "company") ?? props.Company;
            props.Manager = ArgumentHelper.GetStringNullable(arguments, "manager") ?? props.Manager;

            if (arguments?["customProperties"] is JsonObject customPropsNode)
                foreach (var kvp in customPropsNode)
                {
                    var value = ConvertCustomPropertyValue(kvp.Value) ?? string.Empty;
                    props.SetCustomPropertyValue(kvp.Key, value);
                }

            package.SaveAs(new FileInfo(outputPath));
            return $"工作簿属性已更新. 输出: {outputPath}";
        });
    }

    private static object? ConvertCustomPropertyValue(JsonNode? valueNode)
    {
        if (valueNode is not JsonValue jsonValue)
            return valueNode?.ToJsonString();

        if (jsonValue.TryGetValue(out string? stringValue)) return stringValue;
        if (jsonValue.TryGetValue(out bool boolValue)) return boolValue;
        if (jsonValue.TryGetValue(out int intValue)) return intValue;
        if (jsonValue.TryGetValue(out long longValue)) return longValue;
        if (jsonValue.TryGetValue(out double doubleValue)) return doubleValue;
        if (jsonValue.TryGetValue(out DateTime dateValue)) return dateValue;

        return jsonValue.ToString();
    }

    private Task<string> GetSheetPropertiesAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var dimension = worksheet.Dimension;
            var printer = worksheet.PrinterSettings;
            var drawings = worksheet.Drawings;

            var result = new
            {
                name = worksheet.Name,
                index = sheetIndex,
                isVisible = worksheet.Hidden == eWorkSheetHidden.Visible,
                hiddenState = worksheet.Hidden.ToString(),
                tabColor = ColorToHex(worksheet.TabColor),
                isSelected = worksheet.View.TabSelected,
                dataRowCount = dimension?.End.Row ?? 0,
                dataColumnCount = dimension?.End.Column ?? 0,
                usedRange = dimension?.Address,
                isProtected = worksheet.Protection.IsProtected,
                commentsCount = worksheet.Comments.Count,
                chartsCount = drawings.Count(d => d is ExcelChart),
                picturesCount = drawings.Count(d => d is ExcelPicture),
                hyperlinksCount = CountHyperlinks(worksheet),
                printSettings = new
                {
                    printArea = printer.PrintArea?.Address,
                    printTitleRows = printer.RepeatRows?.Address,
                    printTitleColumns = printer.RepeatColumns?.Address,
                    orientation = SafePrinterOrientation(printer),
                    paperSize = SafePrinterPaperSize(printer),
                    fitToPagesWide = printer.FitToWidth,
                    fitToPagesTall = printer.FitToHeight
                }
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> EditSheetPropertiesAsync(string path, string outputPath, int sheetIndex,
        JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var newName = ArgumentHelper.GetStringNullable(arguments, "name");
            var isVisible = ArgumentHelper.GetBoolNullable(arguments, "isVisible");
            var tabColor = ArgumentHelper.GetStringNullable(arguments, "tabColor");
            var isSelected = ArgumentHelper.GetBoolNullable(arguments, "isSelected");

            if (!string.IsNullOrWhiteSpace(newName))
                worksheet.Name = newName;

            if (isVisible.HasValue)
            {
                if (!isVisible.Value)
                {
                    if (worksheet.Hidden == eWorkSheetHidden.Visible && IsLastVisibleSheet(package, worksheet))
                        throw new InvalidOperationException("至少需要一个可见的工作表，不能将最后一个工作表隐藏。");
                    worksheet.Hidden = eWorkSheetHidden.Hidden;
                }
                else
                {
                    worksheet.Hidden = eWorkSheetHidden.Visible;
                }
            }

            if (!string.IsNullOrWhiteSpace(tabColor))
                worksheet.TabColor = ParseColor(tabColor!, worksheet.TabColor);

            if (isSelected.HasValue && isSelected.Value)
            {
                foreach (var sheet in package.Workbook.Worksheets)
                    sheet.View.TabSelected = false;

                worksheet.View.TabSelected = true;
                package.Workbook.View.ActiveTab = sheetIndex;
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"工作表 {sheetIndex} 属性已更新. 输出: {outputPath}";
        });
    }

    private Task<string> GetSheetInfoAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheets = package.Workbook.Worksheets;
            var sheetIndex = ArgumentHelper.GetIntNullable(arguments, "sheetIndex");

            var items = new List<object>();
            if (sheetIndex.HasValue)
            {
                var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex.Value);
                items.Add(CreateSheetInfo(worksheet, sheetIndex.Value));
            }
            else
            {
                for (var i = 0; i < worksheets.Count; i++)
                    items.Add(CreateSheetInfo(worksheets[i], i));
            }

            var result = new
            {
                count = items.Count,
                totalWorksheets = worksheets.Count,
                items
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private static object CreateSheetInfo(ExcelWorksheet worksheet, int index)
    {
        var dimension = worksheet.Dimension;
        var printer = worksheet.PrinterSettings;
        var pane = worksheet.View.PaneSettings;
        var isFrozen = pane?.State is ePaneState.Frozen or ePaneState.FrozenSplit;

        int? frozenRows = null;
        int? frozenColumns = null;
        if (isFrozen)
        {
            var rows = (int)Math.Round(pane!.YSplit);
            var columns = (int)Math.Round(pane.XSplit);
            if (rows > 0) frozenRows = rows;
            if (columns > 0) frozenColumns = columns;
        }

        return new
        {
            index,
            name = worksheet.Name,
            visibility = worksheet.Hidden.ToString(),
            isVisible = worksheet.Hidden == eWorkSheetHidden.Visible,
            tabColor = ColorToHex(worksheet.TabColor),
            dataRowCount = dimension?.End.Row ?? 0,
            dataColumnCount = dimension?.End.Column ?? 0,
            usedRange = new
            {
                address = dimension?.Address,
                rowCount = dimension?.Rows ?? 0,
                columnCount = dimension?.Columns ?? 0
            },
            pageOrientation = SafePrinterOrientation(printer),
            paperSize = SafePrinterPaperSize(printer),
            freezePanes = new
            {
                rows = frozenRows,
                columns = frozenColumns
            }
        };
    }

    private static IEnumerable<object> ExtractCustomProperties(OfficeProperties props)
    {
        var xml = props.CustomPropertiesXml;
        if (xml?.DocumentElement == null)
            return Array.Empty<object>();

        var items = new List<object>();
        var nsmgr = new XmlNamespaceManager(xml.NameTable);
        nsmgr.AddNamespace("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");

        foreach (XmlNode node in xml.DocumentElement.ChildNodes)
        {
            if (node.NodeType != XmlNodeType.Element)
                continue;

            var name = node.Attributes?["name"]?.Value;
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var valueNode = node.FirstChild;
            items.Add(new
            {
                name,
                value = valueNode?.InnerText,
                type = valueNode?.LocalName ?? "string"
            });
        }

        return items;
    }

    private static string? GetHyperlinkBaseSafe(OfficeProperties props)
    {
        try
        {
            return props.HyperlinkBase?.OriginalString;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsLastVisibleSheet(ExcelPackage package, ExcelWorksheet worksheet)
    {
        return !package.Workbook.Worksheets.Any(ws =>
            !ReferenceEquals(ws, worksheet) && ws.Hidden == eWorkSheetHidden.Visible);
    }

    private static int CountHyperlinks(ExcelWorksheet worksheet)
    {
        var xml = worksheet.WorksheetXml;
        var nsUri = xml.DocumentElement?.NamespaceURI ?? "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var nsmgr = new XmlNamespaceManager(xml.NameTable);
        nsmgr.AddNamespace("d", nsUri);
        var nodes = xml.SelectNodes("//d:hyperlinks/d:hyperlink", nsmgr);
        return nodes?.Count ?? 0;
    }

    private static string SafePrinterOrientation(ExcelPrinterSettings printer)
    {
        try
        {
            return printer.Orientation.ToString();
        }
        catch
        {
            return "Unknown";
        }
    }

    private static string SafePrinterPaperSize(ExcelPrinterSettings printer)
    {
        try
        {
            return printer.PaperSize.ToString();
        }
        catch
        {
            return "Unknown";
        }
    }

    private static string? ColorToHex(Color color)
    {
        return color.IsEmpty ? null : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private static Color ParseColor(string colorString, Color fallback)
    {
        if (string.IsNullOrWhiteSpace(colorString))
            return fallback;

        colorString = colorString.Trim();
        if (colorString.StartsWith("#"))
            colorString = colorString[1..];

        if (colorString.Length is 6 or 8)
        {
            try
            {
                var start = colorString.Length == 8 ? 2 : 0;
                var r = Convert.ToInt32(colorString.Substring(start, 2), 16);
                var g = Convert.ToInt32(colorString.Substring(start + 2, 2), 16);
                var b = Convert.ToInt32(colorString.Substring(start + 4, 2), 16);
                var a = colorString.Length == 8 ? Convert.ToInt32(colorString.Substring(0, 2), 16) : 255;
                return Color.FromArgb(a, r, g, b);
            }
            catch
            {
                throw new ArgumentException($"无效的颜色格式: {colorString}");
            }
        }

        throw new ArgumentException($"无效的颜色格式: {colorString}");
    }
}
