using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace ET.Tools.Excel;

/// <summary>
///     图表操作工具 - 提供创建、编辑、删除、查询及属性设置能力
/// </summary>
public class ExcelChartTool : IExcelTool
{
    public string Description =>
        @"管理 Excel 图表。支持 6 种操作: add, edit, delete, get, update_data, set_properties。

使用示例:
- 添加图表: excel_chart(operation='add', path='book.xlsx', chartType='Column', dataRange='B1:B10', categoryAxisDataRange='A1:A10')
- 编辑图表: excel_chart(operation='edit', path='book.xlsx', chartIndex=0, chartType='Line')
- 删除图表: excel_chart(operation='delete', path='book.xlsx', chartIndex=0)
- 获取图表: excel_chart(operation='get', path='book.xlsx')
- 更新数据: excel_chart(operation='update_data', path='book.xlsx', chartIndex=0, dataRange='B1:C10')
- 设置属性: excel_chart(operation='set_properties', path='book.xlsx', chartIndex=0, title='Chart Title')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'add': 添加图表 (必需参数: path, chartType, dataRange)
- 'edit': 编辑图表 (必需参数: path, chartIndex)
- 'delete': 删除图表 (必需参数: path, chartIndex)
- 'get': 获取图表信息 (必需参数: path)
- 'update_data': 更新图表数据 (必需参数: path, chartIndex, dataRange)
- 'set_properties': 设置图表属性 (必需参数: path, chartIndex)",
                @enum = new[] { "add", "edit", "delete", "get", "update_data", "set_properties" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 默认为输入路径)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 可选, 默认: 0)"
            },
            chartIndex = new
            {
                type = "number",
                description = "图表索引 (从0开始, 用于 edit/delete/update_data/set_properties)"
            },
            chartType = new
            {
                type = "string",
                description = "图表类型 (add 操作必需, edit 操作可选)",
                @enum = new[]
                {
                    "Column", "Bar", "Line", "Pie", "Area", "Scatter", "Doughnut", "Radar", "Bubble", "Cylinder",
                    "Cone", "Pyramid"
                }
            },
            dataRange = new
            {
                type = "string",
                description = "数据范围 (例如 'B1:B10' 或多序列 'B1:B10,C1:C10')"
            },
            categoryAxisDataRange = new
            {
                type = "string",
                description = "横坐标数据范围 (可选, 例如 'A1:A10')"
            },
            title = new
            {
                type = "string",
                description = "图表标题 (可选)"
            },
            topRow = new
            {
                type = "number",
                description = "图表左上角所在的行 (从0开始, 可选, 默认: 自动计算)"
            },
            leftColumn = new
            {
                type = "number",
                description = "图表左上角所在的列 (从0开始, 可选, 默认: 0)"
            },
            width = new
            {
                type = "number",
                description = "图表宽度 (以列数为单位, 可选, 默认: 10)"
            },
            height = new
            {
                type = "number",
                description = "图表高度 (以行数为单位, 可选, 默认: 15)"
            },
            showLegend = new
            {
                type = "boolean",
                description = "是否显示图例 (edit/set_properties 可选)"
            },
            legendPosition = new
            {
                type = "string",
                description = "图例位置: Bottom, Top, Left, Right (可选)"
            },
            removeTitle = new
            {
                type = "boolean",
                description = "移除标题 (set_properties 可选)"
            },
            legendVisible = new
            {
                type = "boolean",
                description = "图例显示状态 (set_properties 可选)"
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
            "add" => await AddChartAsync(path, outputPath, sheetIndex, arguments),
            "edit" => await EditChartAsync(path, outputPath, sheetIndex, arguments),
            "delete" => await DeleteChartAsync(path, outputPath, sheetIndex, arguments),
            "get" => await GetChartsAsync(path, sheetIndex),
            "update_data" => await UpdateChartDataAsync(path, outputPath, sheetIndex, arguments),
            "set_properties" => await SetChartPropertiesAsync(path, outputPath, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> AddChartAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var chartTypeStr = ArgumentHelper.GetString(arguments, "chartType");
            var dataRange = ArgumentHelper.GetString(arguments, "dataRange");
            var categoryAxisDataRange = ArgumentHelper.GetStringNullable(arguments, "categoryAxisDataRange");
            var title = ArgumentHelper.GetStringNullable(arguments, "title");
            var topRow = ArgumentHelper.GetIntNullable(arguments, "topRow");
            var leftColumn = ArgumentHelper.GetInt(arguments, "leftColumn", 0);
            var width = ArgumentHelper.GetInt(arguments, "width", 10);
            var height = ArgumentHelper.GetInt(arguments, "height", 15);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var chartType = ParseChartType(chartTypeStr);

            var positionRow = topRow ?? CalculateChartTopRow(worksheet, dataRange);
            var chartName = $"chart_{Guid.NewGuid():N}";

            var chart = worksheet.Drawings.AddChart(chartName, chartType);
            chart.SetPosition(positionRow, 0, leftColumn, 0);
            chart.SetSize(ConvertColumnsToPixels(width), ConvertRowsToPixels(height));

            AddDataSeries(chart, dataRange, categoryAxisDataRange);

            if (!string.IsNullOrEmpty(title))
                chart.Title.Text = title;

            package.SaveAs(new FileInfo(outputPath));

            var message = $"已添加图表, 数据范围: {dataRange}";
            if (!string.IsNullOrEmpty(categoryAxisDataRange))
                message += $", 横坐标: {categoryAxisDataRange}";
            message += $". 输出: {outputPath}";
            return message;
        });
    }

    private Task<string> EditChartAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var chartIndex = ArgumentHelper.GetInt(arguments, "chartIndex");
            var title = ArgumentHelper.GetStringNullable(arguments, "title");
            var dataRange = ArgumentHelper.GetStringNullable(arguments, "dataRange");
            var categoryAxisDataRange = ArgumentHelper.GetStringNullable(arguments, "categoryAxisDataRange");
            var chartTypeStr = ArgumentHelper.GetStringNullable(arguments, "chartType");
            var showLegend = ArgumentHelper.GetBoolNullable(arguments, "showLegend");
            var legendPosition = ArgumentHelper.GetStringNullable(arguments, "legendPosition");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var chart = GetChartByIndex(worksheet, chartIndex);
            var changes = new List<string>();

            if (!string.IsNullOrEmpty(title))
            {
                chart.Title.Text = title;
                changes.Add($"Title: {title}");
            }

            if (!string.IsNullOrEmpty(dataRange))
            {
                AddDataSeries(chart, dataRange, categoryAxisDataRange);
                changes.Add($"Data: {dataRange}");
            }

            if (!string.IsNullOrEmpty(chartTypeStr))
            {
                var newType = ParseChartType(chartTypeStr, chart.ChartType);
                chart = ReplaceChartType(worksheet, chart, newType);
                changes.Add($"Type: {chartTypeStr}");
            }

            if (showLegend.HasValue)
            {
                ToggleLegend(chart, showLegend.Value);
                changes.Add($"Legend: {(showLegend.Value ? "show" : "hide")}");
            }

            if (!string.IsNullOrEmpty(legendPosition))
            {
                chart.Legend.Add();
                chart.Legend.Position = ParseLegendPosition(legendPosition, chart.Legend.Position);
                changes.Add($"Legend position: {legendPosition}");
            }

            package.SaveAs(new FileInfo(outputPath));

            return changes.Count > 0
                ? $"已编辑图表 #{chartIndex}: {string.Join(", ", changes)}. 输出: {outputPath}"
                : $"图表 #{chartIndex} 无改动. 输出: {outputPath}";
        });
    }

    private Task<string> DeleteChartAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var chartIndex = ArgumentHelper.GetInt(arguments, "chartIndex");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var charts = GetCharts(worksheet);

            if (chartIndex < 0 || chartIndex >= charts.Count)
                throw new ArgumentException(
                    $"图表索引 {chartIndex} 超出范围 (共有 {charts.Count} 个图表)");

            var chart = charts[chartIndex];
            var chartName = chart.Name ?? $"Chart{chartIndex}";
            worksheet.Drawings.Remove(chart);

            package.SaveAs(new FileInfo(outputPath));
            return $"已删除图表 #{chartIndex} ({chartName}), 剩余 {charts.Count - 1} 个. 输出: {outputPath}";
        });
    }

    private Task<string> GetChartsAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var charts = GetCharts(worksheet);

            if (charts.Count == 0)
            {
                var empty = new
                {
                    count = 0,
                    worksheetName = worksheet.Name,
                    items = Array.Empty<object>(),
                    message = "未找到图表"
                };
                return JsonSerializer.Serialize(empty, new JsonSerializerOptions { WriteIndented = true });
            }

            var chartInfo = charts.Select((chart, index) =>
            {
                var seriesList = new List<object>();
                for (var i = 0; i < chart.Series.Count && i < 5; i++)
                {
                    var serie = chart.Series[i];
                    seriesList.Add(new
                    {
                        index = i,
                        header = serie.Header,
                        valuesRange = serie.Series,
                        categoryRange = serie.XSeries
                    });
                }

                return new
                {
                    index,
                    name = chart.Name ?? "(no name)",
                    type = chart.ChartType.ToString(),
                    location = new
                    {
                        topRow = chart.From?.Row ?? 0,
                        bottomRow = chart.To?.Row ?? 0,
                        leftColumn = chart.From?.Column ?? 0,
                        rightColumn = chart.To?.Column ?? 0
                    },
                    width = chart.Size?.Width ?? 0,
                    height = chart.Size?.Height ?? 0,
                    title = chart.Title?.Text,
                    legendVisible = chart.Legend != null,
                    seriesCount = chart.Series.Count,
                    series = seriesList
                };
            }).ToList();

            var result = new
            {
                count = charts.Count,
                worksheetName = worksheet.Name,
                items = chartInfo
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> UpdateChartDataAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var chartIndex = ArgumentHelper.GetInt(arguments, "chartIndex");
            var dataRange = ArgumentHelper.GetString(arguments, "dataRange");
            var categoryAxisDataRange = ArgumentHelper.GetStringNullable(arguments, "categoryAxisDataRange");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var chart = GetChartByIndex(worksheet, chartIndex);

            AddDataSeries(chart, dataRange, categoryAxisDataRange);

            package.SaveAs(new FileInfo(outputPath));

            var msg = $"图表 #{chartIndex} 的数据已更新为: {dataRange}";
            if (!string.IsNullOrEmpty(categoryAxisDataRange))
                msg += $", 横坐标: {categoryAxisDataRange}";
            msg += $". 输出: {outputPath}";
            return msg;
        });
    }

    private Task<string> SetChartPropertiesAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var chartIndex = ArgumentHelper.GetInt(arguments, "chartIndex");
            var title = ArgumentHelper.GetStringNullable(arguments, "title");
            var removeTitle = ArgumentHelper.GetBool(arguments, "removeTitle", false);
            var legendVisible = ArgumentHelper.GetBoolNullable(arguments, "legendVisible");
            var legendPosition = ArgumentHelper.GetStringNullable(arguments, "legendPosition");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var chart = GetChartByIndex(worksheet, chartIndex);
            var changes = new List<string>();

            if (removeTitle)
            {
                chart.Title.Text = string.Empty;
                changes.Add("Title removed");
            }
            else if (!string.IsNullOrEmpty(title))
            {
                chart.Title.Text = title;
                changes.Add($"Title: {title}");
            }

            if (legendVisible.HasValue)
            {
                ToggleLegend(chart, legendVisible.Value);
                changes.Add($"Legend: {(legendVisible.Value ? "show" : "hide")}");
            }

            if (!string.IsNullOrEmpty(legendPosition))
            {
                chart.Legend.Add();
                chart.Legend.Position = ParseLegendPosition(legendPosition, chart.Legend.Position);
                changes.Add($"Legend position: {legendPosition}");
            }

            package.SaveAs(new FileInfo(outputPath));

            return changes.Count > 0
                ? $"图表 #{chartIndex} 属性已更新: {string.Join(", ", changes)}. 输出: {outputPath}"
                : $"图表 #{chartIndex} 无改动. 输出: {outputPath}";
        });
    }

    private static ExcelChart GetChartByIndex(ExcelWorksheet worksheet, int chartIndex)
    {
        var charts = GetCharts(worksheet);
        if (chartIndex < 0 || chartIndex >= charts.Count)
            throw new ArgumentException($"图表索引 {chartIndex} 超出范围 (共有 {charts.Count} 个图表)");
        return charts[chartIndex];
    }

    private static List<ExcelChart> GetCharts(ExcelWorksheet worksheet)
    {
        return worksheet.Drawings
            .Where(d => d is ExcelChart)
            .Cast<ExcelChart>()
            .OrderBy(c => c.From?.Row ?? int.MaxValue)
            .ThenBy(c => c.From?.Column ?? int.MaxValue)
            .ThenBy(c => c.Name)
            .ToList();
    }

    private static void AddDataSeries(ExcelChart chart, string dataRange, string? categoryAxisDataRange)
    {
        if (string.IsNullOrWhiteSpace(dataRange))
            throw new ArgumentException("dataRange 不能为空");

        while (chart.Series.Count > 0)
        {
            chart.Series.Delete(0);
        }

        var ranges = dataRange.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var worksheet = chart.WorkSheet;

        foreach (var rangeAddress in ranges)
        {
            var range = worksheet.Cells[rangeAddress];
            var startRow = range.Start.Row;
            var endRow = range.End.Row;
            var startColumn = range.Start.Column;
            var endColumn = range.End.Column;
            var hasMultipleColumns = endColumn > startColumn;
            var shouldSetHeader = ranges.Length > 1 || hasMultipleColumns;

            if (!hasMultipleColumns)
            {
                AddSeriesFromRange(chart, range, categoryAxisDataRange, shouldSetHeader);
                continue;
            }

            for (var col = startColumn; col <= endColumn; col++)
            {
                var columnRange = worksheet.Cells[startRow, col, endRow, col];
                AddSeriesFromRange(chart, columnRange, categoryAxisDataRange, shouldSetHeader);
            }
        }
    }

    private static void AddSeriesFromRange(ExcelChart chart, ExcelRangeBase valueRange, string? categoryAxisDataRange,
        bool setHeader)
    {
        ExcelRangeBase? categoryRange = null;
        if (!string.IsNullOrWhiteSpace(categoryAxisDataRange))
            categoryRange = chart.WorkSheet.Cells[categoryAxisDataRange];

        var serie = categoryRange == null
            ? chart.Series.Add(valueRange)
            : chart.Series.Add(valueRange, categoryRange);

        if (setHeader && serie.HeaderAddress == null)
            serie.HeaderAddress = valueRange.Worksheet.Cells[valueRange.Start.Row, valueRange.Start.Column];
    }

    private static eChartType ParseChartType(string? chartType, eChartType defaultType = eChartType.ColumnClustered)
    {
        if (string.IsNullOrEmpty(chartType))
            return defaultType;

        return chartType.ToLower() switch
        {
            "column" => eChartType.ColumnClustered,
            "bar" => eChartType.BarClustered,
            "line" => eChartType.Line,
            "pie" => eChartType.Pie,
            "area" => eChartType.Area,
            "scatter" => eChartType.XYScatterLines,
            "doughnut" => eChartType.Doughnut,
            "radar" => eChartType.Radar,
            "bubble" => eChartType.Bubble,
            "cylinder" => eChartType.CylinderColClustered,
            "cone" => eChartType.ConeColClustered,
            "pyramid" => eChartType.PyramidColClustered,
            _ => defaultType
        };
    }

    private static eLegendPosition ParseLegendPosition(string? legendPosition,
        eLegendPosition defaultPosition = eLegendPosition.Bottom)
    {
        if (string.IsNullOrEmpty(legendPosition))
            return defaultPosition;

        return legendPosition.ToLower() switch
        {
            "bottom" => eLegendPosition.Bottom,
            "top" => eLegendPosition.Top,
            "left" => eLegendPosition.Left,
            "right" => eLegendPosition.Right,
            "topright" => eLegendPosition.TopRight,
            _ => defaultPosition
        };
    }

    private static void ToggleLegend(ExcelChart chart, bool visible)
    {
        if (visible)
            chart.Legend.Add();
        else
            chart.Legend.Remove();
    }

    private static ExcelChart ReplaceChartType(ExcelWorksheet worksheet, ExcelChart chart, eChartType newType)
    {
        var name = chart.Name ?? $"chart_{Guid.NewGuid():N}";
        var fromRow = chart.From?.Row ?? 0;
        var fromRowOffset = chart.From?.RowOff ?? 0;
        var fromColumn = chart.From?.Column ?? 0;
        var fromColumnOffset = chart.From?.ColumnOff ?? 0;
        double widthValue = chart.Size?.Width ?? ConvertColumnsToPixels(10);
        double heightValue = chart.Size?.Height ?? ConvertRowsToPixels(15);
        var width = (int)Math.Round(widthValue, MidpointRounding.AwayFromZero);
        var height = (int)Math.Round(heightValue, MidpointRounding.AwayFromZero);
        var title = chart.Title?.Text;
        var legendPosition = chart.Legend?.Position ?? eLegendPosition.Right;
        var legendVisible = chart.Legend != null;

        var series = new List<(string values, string? categories, string? header)>();
        for (var i = 0; i < chart.Series.Count; i++)
        {
            var serie = chart.Series[i];
            series.Add((
                serie.Series,
                serie.XSeries,
                serie.Header));
        }

        worksheet.Drawings.Remove(chart);

        var newChart = worksheet.Drawings.AddChart(name, newType);
        newChart.SetPosition(fromRow, fromRowOffset, fromColumn, fromColumnOffset);
        newChart.SetSize(width, height);

        foreach (var serieInfo in series)
        {
            if (string.IsNullOrEmpty(serieInfo.values))
                continue;

            var serie = newChart.Series.Add(serieInfo.values);
            if (!string.IsNullOrEmpty(serieInfo.categories))
                serie.XSeries = serieInfo.categories;

            if (!string.IsNullOrEmpty(serieInfo.header))
                serie.Header = serieInfo.header;
        }

        if (!string.IsNullOrEmpty(title))
            newChart.Title.Text = title;

        if (legendVisible)
        {
            newChart.Legend.Add();
            newChart.Legend.Position = legendPosition;
        }
        else
        {
            newChart.Legend.Remove();
        }

        return newChart;
    }

    private static int ConvertColumnsToPixels(int columns)
    {
        columns = Math.Max(columns, 1);
        return columns * 64;
    }

    private static int ConvertRowsToPixels(int rows)
    {
        rows = Math.Max(rows, 1);
        return rows * 20;
    }

    private static int CalculateChartTopRow(ExcelWorksheet worksheet, string dataRange)
    {
        var firstRange = dataRange.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        if (string.IsNullOrEmpty(firstRange))
            return 0;

        var cells = worksheet.Cells[firstRange];
        var endRow = cells.End.Row;
        return Math.Max(0, endRow + 1);
    }
}
