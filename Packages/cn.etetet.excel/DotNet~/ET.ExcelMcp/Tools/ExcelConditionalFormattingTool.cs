using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using OfficeOpenXml.Style;

namespace ET.Tools.Excel;

/// <summary>
///     条件格式工具 – 支持添加、清除、删除和查询条件格式
/// </summary>
public class ExcelConditionalFormattingTool : IExcelTool
{
    private const string OperationAdd = "add";
    private const string OperationClear = "clear";
    private const string OperationDelete = "delete";
    private const string OperationGet = "get";

    public string Description =>
        @"管理 Excel 条件格式。支持 4 种操作: add, clear, delete, get。

使用示例:
- 添加规则: excel_conditional_formatting(operation='add', path='book.xlsx', range='A2:A20', ruleType='greater_than', value='100')
- 色阶: excel_conditional_formatting(operation='add', path='book.xlsx', range='B2:B20', ruleType='color_scale', scaleType='three_color', lowColor='#63BE7B', midColor='#FFEB84', highColor='#F8696B')
- 数据条: excel_conditional_formatting(operation='add', path='book.xlsx', range='C2:C20', ruleType='data_bar', color='#1F4E78')
- 获取信息: excel_conditional_formatting(operation='get', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'add': 添加条件格式
- 'clear': 按范围清除全部条件格式
- 'delete': 按索引删除指定条件格式
- 'get': 获取条件格式信息",
                @enum = new[] { OperationAdd, OperationClear, OperationDelete, OperationGet }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (add/clear/delete 使用, 默认为 path)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (默认 0)"
            },
            range = new
            {
                type = "string",
                description = "条件格式作用范围 (如 'A2:A20')"
            },
            ruleType = new
            {
                type = "string",
                description = @"add 操作必填, 规则类型:
- 'greater_than', 'less_than', 'between', 'equal', 'not_equal'
- 'contains_text'
- 'color_scale' (two_color/three_color)
- 'data_bar'
- 'icon_set'
- 'formula'",
                @enum = new[]
                    { "greater_than", "less_than", "between", "equal", "not_equal", "contains_text", "color_scale", "data_bar", "icon_set", "formula" }
            },
            value = new
            {
                type = "string",
                description = "比较值 (greater_than/less_than/equal/not_equal)"
            },
            minValue = new
            {
                type = "string",
                description = "区间最小值 (between)"
            },
            maxValue = new
            {
                type = "string",
                description = "区间最大值 (between)"
            },
            text = new
            {
                type = "string",
                description = "匹配文本 (contains_text)"
            },
            formula = new
            {
                type = "string",
                description = "自定义公式 (formula)"
            },
            scaleType = new
            {
                type = "string",
                description = "色阶类型 two_color / three_color (默认 three_color)"
            },
            lowColor = new
            {
                type = "string",
                description = "色阶低值颜色 (hex)"
            },
            midColor = new
            {
                type = "string",
                description = "色阶中间颜色 (hex, three_color)"
            },
            highColor = new
            {
                type = "string",
                description = "色阶高值颜色 (hex)"
            },
            color = new
            {
                type = "string",
                description = "数据条颜色 (hex)"
            },
            iconSet = new
            {
                type = "string",
                description = "图标集类型, 如 ThreeArrows, ThreeTrafficLights1 等"
            },
            showValue = new
            {
                type = "boolean",
                description = "数据条是否显示原值 (默认 true)"
            },
            ruleIndex = new
            {
                type = "number",
                description = "delete 操作使用的条件格式索引"
            },
            style = new
            {
                type = "object",
                description = "自定义样式设置, 支持 fontColor/backgroundColor/bold/italic",
                properties = new
                {
                    fontColor = new { type = "string" },
                    backgroundColor = new { type = "string" },
                    bold = new { type = "boolean" },
                    italic = new { type = "boolean" }
                }
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
            OperationAdd => await AddRuleAsync(path, ArgumentHelper.GetAndValidateOutputPath(arguments, path),
                sheetIndex, arguments),
            OperationClear => await ClearRulesAsync(path, ArgumentHelper.GetAndValidateOutputPath(arguments, path),
                sheetIndex, arguments),
            OperationDelete => await DeleteRuleAsync(path, ArgumentHelper.GetAndValidateOutputPath(arguments, path),
                sheetIndex, arguments),
            OperationGet => await GetRulesAsync(path, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> AddRuleAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var rangeAddress = ArgumentHelper.GetString(arguments, "range");
            var ruleType = ArgumentHelper.GetString(arguments, "ruleType").ToLowerInvariant();

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var normalizedRange = NormalizeRange(rangeAddress);
            var range = worksheet.Cells[normalizedRange];

            var rule = CreateRule(worksheet, range, ruleType, arguments);
            ApplyStyle(rule, arguments);

            package.SaveAs(new FileInfo(outputPath));
            return $"已在 {worksheet.Name}!{rangeAddress} 创建 {ruleType} 条件格式, 输出: {outputPath}";
        });
    }

    private Task<string> ClearRulesAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var rangeAddress = ArgumentHelper.GetString(arguments, "range");
            var normalizedRange = NormalizeRange(rangeAddress);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var removed = RemoveRulesByRange(worksheet, normalizedRange);

            package.SaveAs(new FileInfo(outputPath));
            return removed == 0
                ? $"范围 {rangeAddress} 没有条件格式"
                : $"已移除 {removed} 条条件格式, 输出: {outputPath}";
        });
    }

    private Task<string> DeleteRuleAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var ruleIndex = ArgumentHelper.GetInt(arguments, "ruleIndex");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            if (ruleIndex < 0 || ruleIndex >= worksheet.ConditionalFormatting.Count)
                throw new ArgumentException(
                    $"条件格式索引 {ruleIndex} 超出范围 (共 {worksheet.ConditionalFormatting.Count} 条)");

            worksheet.ConditionalFormatting.RemoveAt(ruleIndex);

            package.SaveAs(new FileInfo(outputPath));
            return $"已删除索引 {ruleIndex} 的条件格式, 输出: {outputPath}";
        });
    }

    private Task<string> GetRulesAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var filterRange = ArgumentHelper.GetStringNullable(arguments, "range");
            var normalizedFilter = string.IsNullOrEmpty(filterRange) ? null : NormalizeRange(filterRange);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var items = worksheet.ConditionalFormatting
                .Select((rule, index) => new
                {
                    index,
                    type = rule.Type.ToString(),
                    address = rule.Address.Address,
                    fontColor = GetColorHex(rule.Style?.Font?.Color?.Color),
                    backgroundColor = GetColorHex(rule.Style?.Fill?.BackgroundColor?.Color),
                    hasDataBar = rule.Type == eExcelConditionalFormattingRuleType.DataBar,
                    hasIconSet = rule.Type == eExcelConditionalFormattingRuleType.ThreeIconSet
                                 || rule.Type == eExcelConditionalFormattingRuleType.FourIconSet
                                 || rule.Type == eExcelConditionalFormattingRuleType.FiveIconSet
                })
                .Where(item => normalizedFilter == null ||
                               string.Equals(item.address, normalizedFilter,
                                   StringComparison.OrdinalIgnoreCase))
                .ToList();

            var payload = new
            {
                sheet = worksheet.Name,
                count = items.Count,
                message = items.Count == 0 ? "未找到条件格式" : "成功获取条件格式信息",
                items
            };

            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        });
    }

    private static IExcelConditionalFormattingRule CreateRule(ExcelWorksheet worksheet, ExcelRangeBase range,
        string ruleType, JsonObject? arguments)
    {
        var address = new ExcelAddress(range.Address);

        return ruleType switch
        {
            "greater_than" => CreateCompareRule(
                worksheet.ConditionalFormatting.AddGreaterThan(address),
                ArgumentHelper.GetString(arguments, "value")),
            "less_than" => CreateCompareRule(
                worksheet.ConditionalFormatting.AddLessThan(address),
                ArgumentHelper.GetString(arguments, "value")),
            "equal" => CreateCompareRule(
                worksheet.ConditionalFormatting.AddEqual(address),
                ArgumentHelper.GetString(arguments, "value")),
            "not_equal" => CreateCompareRule(
                worksheet.ConditionalFormatting.AddNotEqual(address),
                ArgumentHelper.GetString(arguments, "value")),
            "between" => CreateBetweenRule(
                worksheet.ConditionalFormatting.AddBetween(address),
                ArgumentHelper.GetString(arguments, "minValue"),
                ArgumentHelper.GetString(arguments, "maxValue")),
            "contains_text" => CreateContainsTextRule(worksheet, range, ArgumentHelper.GetString(arguments, "text")),
            "color_scale" => CreateColorScaleRule(worksheet, range, arguments),
            "data_bar" => CreateDataBarRule(worksheet, range, arguments),
            "icon_set" => CreateIconSetRule(worksheet, range, arguments),
            "formula" => CreateFormulaRule(
                worksheet.ConditionalFormatting.AddExpression(address),
                ArgumentHelper.GetString(arguments, "formula")),
            _ => throw new ArgumentException($"不支持的规则类型: {ruleType}")
        };
    }

    private static IExcelConditionalFormattingRule CreateCompareRule(
        IExcelConditionalFormattingRule rule, string value)
    {
        if (rule is ExcelConditionalFormattingRule typedRule)
            typedRule.Formula = value;
        return rule;
    }

    private static IExcelConditionalFormattingRule CreateBetweenRule(
        IExcelConditionalFormattingRule rule, string minValue, string maxValue)
    {
        if (rule is ExcelConditionalFormattingRule typedRule)
        {
            typedRule.Formula = minValue;
            typedRule.Formula2 = maxValue;
        }
        return rule;
    }

    private static IExcelConditionalFormattingRule CreateContainsTextRule(ExcelWorksheet worksheet,
        ExcelRangeBase range, string text)
    {
        var rule = worksheet.ConditionalFormatting.AddContainsText(new ExcelAddress(range.Address));
        SetPropertyIfExists(rule, "Text", text);
        return rule;
    }

    private static IExcelConditionalFormattingRule CreateColorScaleRule(ExcelWorksheet worksheet,
        ExcelRangeBase range, JsonObject? arguments)
    {
        var scaleType = ArgumentHelper.GetString(arguments, "scaleType", "three_color");
        var address = new ExcelAddress(range.Address);
        if (scaleType.Equals("two_color", StringComparison.OrdinalIgnoreCase))
        {
            var rule = worksheet.ConditionalFormatting.AddTwoColorScale(address);
            rule.LowValue.Color = ParseColor(ArgumentHelper.GetStringNullable(arguments, "lowColor"),
                Color.FromArgb(0x63, 0xBE, 0x7B));
            rule.HighValue.Color = ParseColor(ArgumentHelper.GetStringNullable(arguments, "highColor"),
                Color.FromArgb(0xF8, 0x69, 0x6B));
            return rule;
        }
        else
        {
            var rule = worksheet.ConditionalFormatting.AddThreeColorScale(address);
            rule.LowValue.Color = ParseColor(ArgumentHelper.GetStringNullable(arguments, "lowColor"),
                Color.FromArgb(0x63, 0xBE, 0x7B));
            rule.MiddleValue.Color = ParseColor(ArgumentHelper.GetStringNullable(arguments, "midColor"),
                Color.FromArgb(0xFF, 0xEB, 0x84));
            rule.HighValue.Color = ParseColor(ArgumentHelper.GetStringNullable(arguments, "highColor"),
                Color.FromArgb(0xF8, 0x69, 0x6B));
            return rule;
        }
    }

    private static IExcelConditionalFormattingRule CreateDataBarRule(ExcelWorksheet worksheet,
        ExcelRangeBase range, JsonObject? arguments)
    {
        var barColor = ParseColor(ArgumentHelper.GetStringNullable(arguments, "color"),
            Color.FromArgb(0x1F, 0x4E, 0x78));
        var rule = worksheet.ConditionalFormatting.AddDatabar(new ExcelAddress(range.Address), barColor);
        rule.ShowValue = ArgumentHelper.GetBool(arguments, "showValue", true);
        return rule;
    }

    private static IExcelConditionalFormattingRule CreateIconSetRule(ExcelWorksheet worksheet,
        ExcelRangeBase range, JsonObject? arguments)
    {
        var iconSetValue = ArgumentHelper.GetStringNullable(arguments, "iconSet");
        var address = new ExcelAddress(range.Address);
        if (Enum.TryParse(iconSetValue, true, out eExcelconditionalFormatting3IconsSetType iconSet3))
            return worksheet.ConditionalFormatting.AddThreeIconSet(address, iconSet3);

        if (Enum.TryParse(iconSetValue, true, out eExcelconditionalFormatting4IconsSetType iconSet4))
            return worksheet.ConditionalFormatting.AddFourIconSet(address, iconSet4);

        if (Enum.TryParse(iconSetValue, true, out eExcelconditionalFormatting5IconsSetType iconSet5))
            return worksheet.ConditionalFormatting.AddFiveIconSet(address, iconSet5);

        return worksheet.ConditionalFormatting.AddThreeIconSet(address,
            eExcelconditionalFormatting3IconsSetType.Arrows);
    }

    private static IExcelConditionalFormattingRule CreateFormulaRule(
        IExcelConditionalFormattingRule rule, string formula)
    {
        if (rule is ExcelConditionalFormattingRule typedRule)
            typedRule.Formula = formula;
        return rule;
    }

    private static int RemoveRulesByRange(ExcelWorksheet worksheet, string rangeAddress)
    {
        var removed = 0;
        for (var i = worksheet.ConditionalFormatting.Count - 1; i >= 0; i--)
        {
            var rule = worksheet.ConditionalFormatting[i];
            if (string.Equals(rule.Address.Address, rangeAddress, StringComparison.OrdinalIgnoreCase))
            {
                worksheet.ConditionalFormatting.RemoveAt(i);
                removed++;
            }
        }

        return removed;
    }

    private static void ApplyStyle(IExcelConditionalFormattingRule rule, JsonObject? arguments)
    {
        if (arguments?["style"] is not JsonObject styleNode)
            return;

        if (styleNode.TryGetPropertyValue("fontColor", out var fontColorNode) && fontColorNode != null)
        {
            var color = ParseColor(fontColorNode.GetValue<string>(), Color.Black);
            rule.Style.Font.Color.SetColor(color);
        }

        if (styleNode.TryGetPropertyValue("backgroundColor", out var backgroundNode) && backgroundNode != null)
        {
            var color = ParseColor(backgroundNode.GetValue<string>(), Color.LightYellow);
            rule.Style.Fill.PatternType = ExcelFillStyle.Solid;
            rule.Style.Fill.BackgroundColor.SetColor(color);
        }

        if (TryGetBool(styleNode, "bold", out var bold))
            rule.Style.Font.Bold = bold;

        if (TryGetBool(styleNode, "italic", out var italic))
            rule.Style.Font.Italic = italic;
    }

    private static bool TryGetBool(JsonObject styleNode, string propertyName, out bool value)
    {
        value = false;
        if (!styleNode.TryGetPropertyValue(propertyName, out var node) || node == null)
            return false;

        if (node.GetValueKind() == JsonValueKind.True || node.GetValueKind() == JsonValueKind.False)
        {
            value = node.GetValue<bool>();
            return true;
        }

        if (node.GetValueKind() == JsonValueKind.String &&
            bool.TryParse(node.GetValue<string>(), out var parsed))
        {
            value = parsed;
            return true;
        }

        return false;
    }

    private static Color ParseColor(string? input, Color fallback)
    {
        if (string.IsNullOrWhiteSpace(input))
            return fallback;

        var value = input.Trim().TrimStart('#');
        if (value.Length is 6 or 8 &&
            int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var colorValue))
        {
            if (value.Length == 6)
                return Color.FromArgb(255, (colorValue >> 16) & 0xFF, (colorValue >> 8) & 0xFF, colorValue & 0xFF);
            return Color.FromArgb((colorValue >> 24) & 0xFF, (colorValue >> 16) & 0xFF,
                (colorValue >> 8) & 0xFF, colorValue & 0xFF);
        }

        return fallback;
    }

    private static string NormalizeRange(string address)
    {
        var separatorIndex = address.IndexOf('!');
        return separatorIndex >= 0 ? address[(separatorIndex + 1)..] : address;
    }

    private static void SetPropertyIfExists(object target, string propertyName, object value)
    {
        var property = target.GetType().GetProperty(propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(target, value);
    }

    private static string? GetColorHex(Color? color)
    {
        if (color == null || color.Value.IsEmpty)
            return null;

        var value = color.Value;
        return $"#{value.R:X2}{value.G:X2}{value.B:X2}";
    }
}
