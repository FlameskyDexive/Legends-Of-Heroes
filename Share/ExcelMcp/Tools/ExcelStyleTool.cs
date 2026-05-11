using System.Drawing;
using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 样式操作工具 (format, get_format, copy_sheet_format)
/// </summary>
public class ExcelStyleTool : IExcelTool
{
    public string Description => @"Excel 样式操作。支持 3 种操作: format, get_format, copy_sheet_format。

使用示例:
- 格式化单元格: excel_style(operation='format', path='book.xlsx', range='A1:B10', fontName='Arial', fontSize=12, bold=true)
- 获取格式: excel_style(operation='get_format', path='book.xlsx', range='A1')
- 复制工作表格式: excel_style(operation='copy_sheet_format', path='book.xlsx', sourceSheetIndex=0, targetSheetIndex=1)";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'format': 格式化单元格 (必需参数: path, range)
- 'get_format': 获取单元格格式 (必需参数: path, range)
- 'copy_sheet_format': 复制工作表格式 (必需参数: path, sourceSheetIndex, targetSheetIndex)",
                @enum = new[] { "format", "get_format", "copy_sheet_format" }
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
            range = new
            {
                type = "string",
                description = "单元格范围 (如 'A1:C5', format操作使用)"
            },
            ranges = new
            {
                type = "array",
                description = "多个单元格范围 (format操作可选, 如 ['A1', 'B2'])"
            },
            cell = new
            {
                type = "string",
                description = "单元格地址 (如 'A1', get_format操作可使用, 与range二选一)"
            },
            fields = new
            {
                type = "string",
                description = "要获取的字段 (get_format操作可选, 逗号分隔: 'font,color,alignment,border,value')"
            },
            sourceSheetIndex = new
            {
                type = "number",
                description = "源工作表索引 (copy_sheet_format操作必需)"
            },
            targetSheetIndex = new
            {
                type = "number",
                description = "目标工作表索引 (copy_sheet_format操作必需)"
            },
            fontName = new
            {
                type = "string",
                description = "字体名称 (可选)"
            },
            fontSize = new
            {
                type = "number",
                description = "字体大小 (可选)"
            },
            bold = new
            {
                type = "boolean",
                description = "粗体 (可选)"
            },
            italic = new
            {
                type = "boolean",
                description = "斜体 (可选)"
            },
            underline = new
            {
                type = "boolean",
                description = "下划线 (可选)"
            },
            strikethrough = new
            {
                type = "boolean",
                description = "删除线 (可选)"
            },
            fontColor = new
            {
                type = "string",
                description = "字体颜色 (十六进制格式如 '#FF0000', 可选)"
            },
            backgroundColor = new
            {
                type = "string",
                description = "背景颜色 (十六进制格式如 '#FFFF00', 可选)"
            },
            numberFormat = new
            {
                type = "string",
                description = "数字格式字符串 (如 'yyyy-mm-dd', '#,##0.00', 可选)"
            },
            borderStyle = new
            {
                type = "string",
                description = "边框样式 (None, Thin, Medium, Thick, 可选)",
                @enum = new[] { "None", "Thin", "Medium", "Thick", "Dotted", "Dashed", "Double" }
            },
            borderColor = new
            {
                type = "string",
                description = "边框颜色 (十六进制格式, 可选)"
            },
            patternType = new
            {
                type = "string",
                description = "填充图案类型 (可选, 如 'Solid', 'DiagonalStripe', 'Gray50')"
            },
            patternColor = new
            {
                type = "string",
                description = "图案颜色 (十六进制格式, 可选)"
            },
            horizontalAlignment = new
            {
                type = "string",
                description = "水平对齐 (Left, Center, Right, 可选)",
                @enum = new[] { "Left", "Center", "Right" }
            },
            verticalAlignment = new
            {
                type = "string",
                description = "垂直对齐 (Top, Center, Bottom, 可选)",
                @enum = new[] { "Top", "Center", "Bottom" }
            },
            copyColumnWidths = new
            {
                type = "boolean",
                description = "复制列宽 (copy_sheet_format操作可选, 默认true)"
            },
            copyRowHeights = new
            {
                type = "boolean",
                description = "复制行高 (copy_sheet_format操作可选, 默认true)"
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
            "format" => await FormatCellsAsync(path, arguments),
            "get_format" => await GetCellFormatAsync(path, arguments),
            "copy_sheet_format" => await CopySheetFormatAsync(path, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> FormatCellsAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            // 获取样式参数
            var fontName = ArgumentHelper.GetStringNullable(arguments, "fontName");
            var fontSize = ArgumentHelper.GetIntNullable(arguments, "fontSize");
            var bold = ArgumentHelper.GetBoolNullable(arguments, "bold");
            var italic = ArgumentHelper.GetBoolNullable(arguments, "italic");
            var underline = ArgumentHelper.GetBoolNullable(arguments, "underline");
            var strikethrough = ArgumentHelper.GetBoolNullable(arguments, "strikethrough");
            var fontColor = ArgumentHelper.GetStringNullable(arguments, "fontColor");
            var backgroundColor = ArgumentHelper.GetStringNullable(arguments, "backgroundColor");
            var numberFormat = ArgumentHelper.GetStringNullable(arguments, "numberFormat");
            var borderStyle = ArgumentHelper.GetStringNullable(arguments, "borderStyle");
            var borderColor = ArgumentHelper.GetStringNullable(arguments, "borderColor");
            var patternType = ArgumentHelper.GetStringNullable(arguments, "patternType");
            var patternColor = ArgumentHelper.GetStringNullable(arguments, "patternColor");
            var horizontalAlignment = ArgumentHelper.GetStringNullable(arguments, "horizontalAlignment");
            var verticalAlignment = ArgumentHelper.GetStringNullable(arguments, "verticalAlignment");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            // 支持单个range或多个ranges
            var rangeList = new List<string>();
            if (arguments?.ContainsKey("range") == true)
            {
                rangeList.Add(ArgumentHelper.GetString(arguments, "range"));
            }
            else if (arguments?.ContainsKey("ranges") == true && arguments["ranges"] is JsonArray rangesArray)
            {
                foreach (var rangeNode in rangesArray)
                {
                    if (rangeNode != null)
                    {
                        rangeList.Add(rangeNode.ToString());
                    }
                }
            }
            else
            {
                throw new ArgumentException("必须提供 'range' 或 'ranges' 参数");
            }

            // 对每个范围应用格式
            foreach (var range in rangeList)
            {
                var cells = worksheet.Cells[range];

                // 应用字体设置
                if (!string.IsNullOrEmpty(fontName))
                    cells.Style.Font.Name = fontName;

                if (fontSize.HasValue)
                    cells.Style.Font.Size = fontSize.Value;

                if (bold.HasValue)
                    cells.Style.Font.Bold = bold.Value;

                if (italic.HasValue)
                    cells.Style.Font.Italic = italic.Value;

                if (underline.HasValue)
                    cells.Style.Font.UnderLine = underline.Value;

                if (strikethrough.HasValue)
                    cells.Style.Font.Strike = strikethrough.Value;

                // 应用字体颜色
                if (!string.IsNullOrEmpty(fontColor))
                {
                    var color = ParseColor(fontColor);
                    cells.Style.Font.Color.SetColor(color);
                }

                // 应用背景颜色和图案
                if (!string.IsNullOrEmpty(patternType))
                {
                    // 解析图案类型
                    var pattern = Enum.TryParse<ExcelFillStyle>(patternType, true, out var parsedPattern)
                        ? parsedPattern
                        : ExcelFillStyle.Solid;

                    cells.Style.Fill.PatternType = pattern;

                    // 应用背景颜色
                    if (!string.IsNullOrEmpty(backgroundColor))
                    {
                        var bgColor = ParseColor(backgroundColor);
                        cells.Style.Fill.BackgroundColor.SetColor(bgColor);
                    }

                    // 应用图案颜色
                    if (!string.IsNullOrEmpty(patternColor))
                    {
                        var ptColor = ParseColor(patternColor);
                        cells.Style.Fill.PatternColor.SetColor(ptColor);
                    }
                }
                else if (!string.IsNullOrEmpty(backgroundColor))
                {
                    var color = ParseColor(backgroundColor);
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(color);
                }

                // 应用数字格式
                if (!string.IsNullOrEmpty(numberFormat))
                {
                    cells.Style.Numberformat.Format = numberFormat;
                }

                // 应用对齐方式
                if (!string.IsNullOrEmpty(horizontalAlignment))
                {
                    cells.Style.HorizontalAlignment = horizontalAlignment.ToLowerInvariant() switch
                    {
                        "left" => ExcelHorizontalAlignment.Left,
                        "center" => ExcelHorizontalAlignment.Center,
                        "right" => ExcelHorizontalAlignment.Right,
                        _ => ExcelHorizontalAlignment.Left
                    };
                }

                if (!string.IsNullOrEmpty(verticalAlignment))
                {
                    cells.Style.VerticalAlignment = verticalAlignment.ToLowerInvariant() switch
                    {
                        "top" => ExcelVerticalAlignment.Top,
                        "center" => ExcelVerticalAlignment.Center,
                        "bottom" => ExcelVerticalAlignment.Bottom,
                        _ => ExcelVerticalAlignment.Center
                    };
                }

                // 应用边框
                if (!string.IsNullOrEmpty(borderStyle))
                {
                    var style = borderStyle.ToLowerInvariant() switch
                    {
                        "none" => ExcelBorderStyle.None,
                        "thin" => ExcelBorderStyle.Thin,
                        "medium" => ExcelBorderStyle.Medium,
                        "thick" => ExcelBorderStyle.Thick,
                        "dotted" => ExcelBorderStyle.Dotted,
                        "dashed" => ExcelBorderStyle.Dashed,
                        "double" => ExcelBorderStyle.Double,
                        _ => ExcelBorderStyle.Thin
                    };

                    var color = string.IsNullOrEmpty(borderColor) ? Color.Black : ParseColor(borderColor);

                    cells.Style.Border.Top.Style = style;
                    cells.Style.Border.Bottom.Style = style;
                    cells.Style.Border.Left.Style = style;
                    cells.Style.Border.Right.Style = style;

                    if (!string.IsNullOrEmpty(borderColor))
                    {
                        cells.Style.Border.Top.Color.SetColor(color);
                        cells.Style.Border.Bottom.Color.SetColor(color);
                        cells.Style.Border.Left.Color.SetColor(color);
                        cells.Style.Border.Right.Color.SetColor(color);
                    }
                }
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"工作表 {sheetIndex} 的 {rangeList.Count} 个范围已格式化. 输出: {outputPath}";
        });
    }

    private Task<string> GetCellFormatAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            // 支持 cell 或 range 参数
            string range;
            if (arguments?.ContainsKey("cell") == true)
            {
                range = ArgumentHelper.GetString(arguments, "cell");
            }
            else if (arguments?.ContainsKey("range") == true)
            {
                range = ArgumentHelper.GetString(arguments, "range");
            }
            else
            {
                throw new ArgumentException("必须提供 'cell' 或 'range' 参数");
            }

            var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);
            var fieldsStr = ArgumentHelper.GetStringNullable(arguments, "fields");

            // 解析要获取的字段
            var fieldsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(fieldsStr))
            {
                foreach (var field in fieldsStr.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    fieldsSet.Add(field.Trim());
                }
            }
            bool allFields = fieldsSet.Count == 0;

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            // 验证范围
            OfficeOpenXml.ExcelRange cells;
            try
            {
                cells = worksheet.Cells[range];
            }
            catch
            {
                throw new ArgumentException($"Invalid range or cell address: {range}");
            }

            var cellList = new List<object>();

            for (int row = cells.Start.Row; row <= cells.End.Row; row++)
            {
                for (int col = cells.Start.Column; col <= cells.End.Column; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    var style = cell.Style;

                    var cellData = new Dictionary<string, object?>
                    {
                        ["cell"] = cell.Address
                    };

                    // 值和数据类型
                    if (allFields || fieldsSet.Contains("value"))
                    {
                        cellData["value"] = cell.Text;
                        cellData["dataType"] = cell.Value?.GetType().Name ?? "Empty";
                    }

                    // 格式化对象
                    var formatData = new Dictionary<string, object?>();

                    // 字体信息
                    if (allFields || fieldsSet.Contains("font"))
                    {
                        formatData["fontName"] = style.Font.Name;
                        formatData["fontSize"] = style.Font.Size;
                        formatData["bold"] = style.Font.Bold;
                        formatData["italic"] = style.Font.Italic;
                        formatData["underline"] = style.Font.UnderLine;
                        formatData["strikethrough"] = style.Font.Strike;
                    }

                    // 颜色信息
                    if (allFields || fieldsSet.Contains("color"))
                    {
                        formatData["fontColor"] = style.Font.Color.Rgb;
                        formatData["foregroundColor"] = style.Fill.BackgroundColor.Rgb;
                        formatData["backgroundColor"] = style.Fill.BackgroundColor.Rgb;
                        formatData["patternType"] = style.Fill.PatternType.ToString();
                    }

                    // 对齐方式
                    if (allFields || fieldsSet.Contains("alignment"))
                    {
                        formatData["horizontalAlignment"] = style.HorizontalAlignment.ToString();
                        formatData["verticalAlignment"] = style.VerticalAlignment.ToString();
                    }

                    // 数字格式
                    if (allFields || fieldsSet.Contains("number") || allFields)
                    {
                        formatData["numberFormat"] = style.Numberformat.Format;
                    }

                    // 边框
                    if (allFields || fieldsSet.Contains("border"))
                    {
                        formatData["borders"] = new
                        {
                            top = new { style = style.Border.Top.Style.ToString(), color = style.Border.Top.Color.Rgb },
                            bottom = new { style = style.Border.Bottom.Style.ToString(), color = style.Border.Bottom.Color.Rgb },
                            left = new { style = style.Border.Left.Style.ToString(), color = style.Border.Left.Color.Rgb },
                            right = new { style = style.Border.Right.Style.ToString(), color = style.Border.Right.Color.Rgb }
                        };
                    }

                    if (formatData.Count > 0)
                    {
                        cellData["format"] = formatData;
                    }

                    if (!string.IsNullOrEmpty(cell.Formula))
                    {
                        cellData["formula"] = cell.Formula;
                    }

                    cellList.Add(cellData);
                }
            }

            var result = new
            {
                count = cellList.Count,
                worksheetName = worksheet.Name,
                range = cells.Address,
                items = cellList
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> CopySheetFormatAsync(string path, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sourceSheetIndex = ArgumentHelper.GetInt(arguments, "sourceSheetIndex");
            var targetSheetIndex = ArgumentHelper.GetInt(arguments, "targetSheetIndex");
            var copyColumnWidths = ArgumentHelper.GetBool(arguments, "copyColumnWidths", true);
            var copyRowHeights = ArgumentHelper.GetBool(arguments, "copyRowHeights", true);
            var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);

            using var package = new ExcelPackage(new FileInfo(path));
            var sourceSheet = ExcelHelper.GetWorksheet(package, sourceSheetIndex);
            var targetSheet = ExcelHelper.GetWorksheet(package, targetSheetIndex);

            // 复制列宽
            if (copyColumnWidths)
            {
                // 获取源工作表中所有设置过列宽的列
                // EPPlus中，即使没有数据，列宽也可以被设置
                // 我们遍历到足够大的列数（比如前1000列）或者直到Dimension的列数
                int maxCol = sourceSheet.Dimension?.Columns ?? 100;
                // 扩展到至少检查前100列，以确保捕获所有自定义列宽
                maxCol = Math.Max(maxCol, 100);

                for (int col = 1; col <= maxCol; col++)
                {
                    var sourceColWidth = sourceSheet.Column(col).Width;
                    // EPPlus默认列宽约为9.14，只复制非默认宽度的列
                    if (Math.Abs(sourceColWidth - 9.140625) > 0.01)
                    {
                        targetSheet.Column(col).Width = sourceColWidth;
                    }
                }
            }

            // 复制行高
            if (copyRowHeights && sourceSheet.Dimension != null)
            {
                for (int row = 1; row <= sourceSheet.Dimension.Rows; row++)
                {
                    targetSheet.Row(row).Height = sourceSheet.Row(row).Height;
                }
            }

            package.SaveAs(new FileInfo(outputPath));
            return $"工作表格式已从工作表 {sourceSheetIndex} 复制到工作表 {targetSheetIndex}. 输出: {outputPath}";
        });
    }

    /// <summary>
    ///     解析颜色字符串为 System.Drawing.Color
    ///     支持格式: #RRGGBB, RRGGBB
    /// </summary>
    private static Color ParseColor(string colorString)
    {
        if (string.IsNullOrWhiteSpace(colorString))
            return Color.Black;

        colorString = colorString.Trim();

        // 移除 # 前缀
        if (colorString.StartsWith("#"))
            colorString = colorString.Substring(1);

        // 解析十六进制颜色
        if (colorString.Length == 6)
        {
            try
            {
                int r = Convert.ToInt32(colorString.Substring(0, 2), 16);
                int g = Convert.ToInt32(colorString.Substring(2, 2), 16);
                int b = Convert.ToInt32(colorString.Substring(4, 2), 16);
                return Color.FromArgb(r, g, b);
            }
            catch
            {
                throw new ArgumentException($"无效的颜色格式: {colorString}。请使用格式 #RRGGBB 或 RRGGBB");
            }
        }
        else if (colorString.Length == 8)
        {
            // 支持 AARRGGBB 格式
            try
            {
                int a = Convert.ToInt32(colorString.Substring(0, 2), 16);
                int r = Convert.ToInt32(colorString.Substring(2, 2), 16);
                int g = Convert.ToInt32(colorString.Substring(4, 2), 16);
                int b = Convert.ToInt32(colorString.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            catch
            {
                throw new ArgumentException($"无效的颜色格式: {colorString}。请使用格式 #AARRGGBB 或 AARRGGBB");
            }
        }

        throw new ArgumentException($"无效的颜色格式: {colorString}。请使用格式 #RRGGBB 或 RRGGBB");
    }
}
