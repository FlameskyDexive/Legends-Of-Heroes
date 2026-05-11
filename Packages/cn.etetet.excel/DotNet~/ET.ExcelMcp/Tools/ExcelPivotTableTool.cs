using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;

namespace ET.Tools.Excel;

/// <summary>
///     数据透视表操作工具 – 创建、配置、刷新、获取信息
/// </summary>
public class ExcelPivotTableTool : IExcelTool
{
    private const string OperationCreate = "create";
    private const string OperationConfigure = "configure";
    private const string OperationRefresh = "refresh";
    private const string OperationGet = "get";

    private static readonly Dictionary<string, DataFieldFunctions> DataFunctionMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["sum"] = DataFieldFunctions.Sum,
            ["count"] = DataFieldFunctions.Count,
            ["average"] = DataFieldFunctions.Average,
            ["max"] = DataFieldFunctions.Max,
            ["min"] = DataFieldFunctions.Min,
            ["product"] = DataFieldFunctions.Product,
            ["stddev"] = DataFieldFunctions.StdDev,
            ["stddevp"] = DataFieldFunctions.StdDevP,
            ["var"] = DataFieldFunctions.Var,
            ["varp"] = DataFieldFunctions.VarP
        };

    public string Description =>
        @"管理 Excel 数据透视表。支持 4 种操作: create, configure, refresh, get。

使用示例:
- 创建: excel_pivot_table(operation='create', path='book.xlsx', dataRange='Sheet1!A1:D20', rowFields=['地区'], dataFields=[{""field"":""销售额"",""function"":""sum""}])
- 配置: excel_pivot_table(operation='configure', path='book.xlsx', pivotIndex=0, columnFields=['产品'], dataFields=[{""field"":""数量"",""function"":""count""}])
- 刷新: excel_pivot_table(operation='refresh', path='book.xlsx', pivotIndex=0)
- 获取信息: excel_pivot_table(operation='get', path='book.xlsx')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'create': 创建数据透视表
- 'configure': 配置字段
- 'refresh': 刷新数据/计算
- 'get': 获取当前工作表的数据透视表信息",
                @enum = new[] { OperationCreate, OperationConfigure, OperationRefresh, OperationGet }
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
                description = "目标工作表索引 (默认 0)"
            },
            sourceSheetIndex = new
            {
                type = "number",
                description = "数据源所在工作表索引 (create 专用, 默认与 sheetIndex 相同)"
            },
            destinationSheetIndex = new
            {
                type = "number",
                description = "数据透视表放置的工作表索引 (create 专用, 默认与 sheetIndex 相同)"
            },
            dataRange = new
            {
                type = "string",
                description = "数据源范围 (如 'Sheet1!A1:D20', create 必填)"
            },
            destinationCell = new
            {
                type = "string",
                description = "数据透视表放置位置 (如 'G3', create 可选)"
            },
            pivotTableName = new
            {
                type = "string",
                description = "数据透视表名称 (create 可选)"
            },
            pivotIndex = new
            {
                type = "number",
                description = "数据透视表索引 (configure/refresh/get 使用, 默认 0)"
            },
            rowFields = new
            {
                type = "array",
                description = "行字段名称数组"
            },
            columnFields = new
            {
                type = "array",
                description = "列字段名称数组"
            },
            pageFields = new
            {
                type = "array",
                description = "筛选字段名称数组"
            },
            dataFields = new
            {
                type = "array",
                description = @"值字段集合, 每个对象支持:
- field: 字段名称
- function: 聚合函数 (sum/count/average/max/min/product/stddev/stddevp/var/varp)
- name: 显示名称
- format: 数字格式"
            },
            clearExistingFields = new
            {
                type = "boolean",
                description = "配置前是否清空已存在字段 (configure 默认 true)"
            },
            refreshData = new
            {
                type = "boolean",
                description = "refresh 操作是否刷新缓存 (默认 true)"
            },
            calculateData = new
            {
                type = "boolean",
                description = "refresh 操作是否重新计算数据 (默认 true)"
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
            OperationCreate => await CreatePivotTableAsync(path,
                ArgumentHelper.GetAndValidateOutputPath(arguments, path), sheetIndex, arguments),
            OperationConfigure => await ConfigurePivotTableAsync(path,
                ArgumentHelper.GetAndValidateOutputPath(arguments, path), sheetIndex, arguments),
            OperationRefresh => await RefreshPivotTableAsync(path,
                ArgumentHelper.GetAndValidateOutputPath(arguments, path), sheetIndex, arguments),
            OperationGet => await GetPivotTablesAsync(path, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> CreatePivotTableAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var sourceSheetIndex = ArgumentHelper.GetInt(arguments, "sourceSheetIndex", sheetIndex);
            var destinationSheetIndex = ArgumentHelper.GetInt(arguments, "destinationSheetIndex", sheetIndex);
            var dataRange = ArgumentHelper.GetString(arguments, "dataRange");
            var destinationCellOverride = ArgumentHelper.GetStringNullable(arguments, "destinationCell");
            var pivotTableName = ArgumentHelper.GetStringNullable(arguments, "pivotTableName") ??
                                 $"pivot_{Guid.NewGuid():N}";

            using var package = new ExcelPackage(new FileInfo(path));
            var sourceSheet = ExcelHelper.GetWorksheet(package, sourceSheetIndex);
            var destinationSheet = ExcelHelper.GetWorksheet(package, destinationSheetIndex);
            var sourceRangeAddress = NormalizeRangeAddress(dataRange);
            var sourceAddress = sourceSheet.Cells[sourceRangeAddress];
            var destinationCell = NormalizeRangeAddress(
                destinationCellOverride ?? GetDefaultDestinationAddress(destinationSheet));
            var destinationAddress = destinationSheet.Cells[destinationCell];

            var pivotTable = destinationSheet.PivotTables.Add(destinationAddress, sourceAddress, pivotTableName);

            ApplyFieldConfiguration(pivotTable, arguments, true);

            package.SaveAs(new FileInfo(outputPath));
            return $"已创建数据透视表 {pivotTable.Name}, 位置: {destinationSheet.Name}!{destinationCell}, 输出: {outputPath}";
        });
    }

    private Task<string> ConfigurePivotTableAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var pivotIndex = ArgumentHelper.GetInt(arguments, "pivotIndex", 0);
            var clearExisting = ArgumentHelper.GetBool(arguments, "clearExistingFields", true);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var pivotTable = GetPivotTable(worksheet, pivotIndex);

            ApplyFieldConfiguration(pivotTable, arguments, clearExisting);

            package.SaveAs(new FileInfo(outputPath));
            return $"已更新数据透视表 {pivotTable.Name}, 输出: {outputPath}";
        });
    }

    private Task<string> RefreshPivotTableAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var pivotIndex = ArgumentHelper.GetInt(arguments, "pivotIndex", 0);
            var refreshData = ArgumentHelper.GetBool(arguments, "refreshData", true);
            var calculateData = ArgumentHelper.GetBool(arguments, "calculateData", true);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var pivotTable = GetPivotTable(worksheet, pivotIndex);

            if (refreshData)
            {
                RefreshPivotCache(pivotTable);
                pivotTable.CacheDefinition.SaveData = true;
            }

            if (calculateData)
                package.Workbook.Calculate();

            package.SaveAs(new FileInfo(outputPath));
            return $"已刷新数据透视表 {pivotTable.Name}, 输出: {outputPath}";
        });
    }

    private Task<string> GetPivotTablesAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var pivotIndexFilter = ArgumentHelper.GetIntNullable(arguments, "pivotIndex");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var items = worksheet.PivotTables
                .Select((pt, index) => new
                {
                    index,
                    name = pt.Name,
                    location = pt.Address.Address,
                    source = pt.CacheDefinition?.SourceRange?.Address ?? string.Empty,
                    rowFields = pt.RowFields.Select(f => f.Name).ToArray(),
                    columnFields = pt.ColumnFields.Select(f => f.Name).ToArray(),
                    filterFields = pt.PageFields.Select(f => f.Name).ToArray(),
                    dataFields = pt.DataFields.Select(df => new
                    {
                        df.Name,
                        field = df.Field?.Name,
                        function = df.Function.ToString(),
                        df.Format
                    }).ToArray()
                })
                .Where(item => !pivotIndexFilter.HasValue || item.index == pivotIndexFilter.Value)
                .ToList();

            var payload = new
            {
                sheet = worksheet.Name,
                count = items.Count,
                message = items.Count == 0 ? "未找到数据透视表" : "成功获取数据透视表信息",
                items
            };

            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        });
    }

    private static void RefreshPivotCache(ExcelPivotTable pivotTable)
    {
        var cacheDefinition = pivotTable.CacheDefinition;
        var refreshMethod = cacheDefinition.GetType().GetMethod("Refresh", Type.EmptyTypes);
        if (refreshMethod != null)
        {
            refreshMethod.Invoke(cacheDefinition, null);
            return;
        }

        cacheDefinition.CacheDefinitionXml?.DocumentElement?.SetAttribute("refreshOnLoad", "1");
    }

    private static void ApplyFieldConfiguration(ExcelPivotTable pivotTable, JsonObject? arguments, bool clearExisting)
    {
        var rowFields = GetFieldList(arguments, "rowFields");
        var columnFields = GetFieldList(arguments, "columnFields");
        var pageFields = GetFieldList(arguments, "pageFields");
        var dataFieldConfigs = GetDataFieldConfigs(arguments);

        if (!rowFields.Any() && !columnFields.Any() && !pageFields.Any() && !dataFieldConfigs.Any())
            return;

        if (clearExisting)
        {
            ClearFieldCollection(pivotTable.RowFields);
            ClearFieldCollection(pivotTable.ColumnFields);
            ClearFieldCollection(pivotTable.PageFields);
            ClearDataFields(pivotTable.DataFields);
        }

        foreach (var field in rowFields)
            AddField(pivotTable, pivotTable.RowFields, field);

        foreach (var field in columnFields)
            AddField(pivotTable, pivotTable.ColumnFields, field);

        foreach (var field in pageFields)
            AddField(pivotTable, pivotTable.PageFields, field);

        foreach (var config in dataFieldConfigs)
            AddDataField(pivotTable, config);
    }

    private static void AddField(ExcelPivotTable pivotTable, ExcelPivotTableRowColumnFieldCollection targetCollection,
        string fieldName)
    {
        var field = pivotTable.Fields[fieldName];
        if (field == null)
            throw new ArgumentException($"数据透视表字段 '{fieldName}' 不存在");

        targetCollection.Add(field);
    }

    private static void ClearFieldCollection(ExcelPivotTableRowColumnFieldCollection collection)
    {
        while (collection.Count > 0)
        {
            var field = collection[collection.Count - 1];
            collection.Remove(field);
        }
    }

    private static void ClearDataFields(ExcelPivotTableDataFieldCollection collection)
    {
        for (var i = collection.Count - 1; i >= 0; i--)
        {
            var field = collection[i];
            collection.Remove(field);
        }
    }

    private static void AddDataField(ExcelPivotTable pivotTable, DataFieldConfig config)
    {
        var baseField = pivotTable.Fields[config.Field];
        if (baseField == null)
            throw new ArgumentException($"值字段 '{config.Field}' 不存在");

        var dataField = pivotTable.DataFields.Add(baseField);
        if (!string.IsNullOrEmpty(config.Name))
            dataField.Name = config.Name;

        if (!string.IsNullOrEmpty(config.Function) &&
            DataFunctionMap.TryGetValue(config.Function, out var function))
            dataField.Function = function;

        if (!string.IsNullOrEmpty(config.Format))
            dataField.Format = config.Format;
    }

    private static List<string> GetFieldList(JsonObject? arguments, string key)
    {
        var array = ArgumentHelper.GetArray(arguments, key, key, false);
        if (array == null)
            return new List<string>();

        return array
            .Select(node => node?.GetValue<string>())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToList();
    }

    private static List<DataFieldConfig> GetDataFieldConfigs(JsonObject? arguments)
    {
        var array = ArgumentHelper.GetArray(arguments, "dataFields", "dataFields", false);
        if (array == null)
            return new List<DataFieldConfig>();

        var result = new List<DataFieldConfig>();
        foreach (var node in array)
        {
            if (node is not JsonObject obj)
                throw new ArgumentException("dataFields 中的每一项都必须是对象");

            var field = ArgumentHelper.GetString(obj, "field");
            var function = ArgumentHelper.GetStringNullable(obj, "function")?.ToLowerInvariant();
            var name = ArgumentHelper.GetStringNullable(obj, "name");
            var format = ArgumentHelper.GetStringNullable(obj, "format");
            result.Add(new DataFieldConfig(field, function, name, format));
        }

        return result;
    }

    private static ExcelPivotTable GetPivotTable(ExcelWorksheet worksheet, int pivotIndex)
    {
        if (pivotIndex < 0 || pivotIndex >= worksheet.PivotTables.Count)
            throw new ArgumentException(
                $"数据透视表索引 {pivotIndex} 超出范围 (共 {worksheet.PivotTables.Count} 个)");

        return worksheet.PivotTables[pivotIndex];
    }

    private static string GetDefaultDestinationAddress(ExcelWorksheet worksheet)
    {
        if (worksheet.Dimension == null)
            return "E3";

        var startRow = Math.Max(1, worksheet.Dimension.End.Row + 2);
        var startColumn = Math.Max(1, worksheet.Dimension.End.Column + 2);
        return worksheet.Cells[startRow, startColumn].Address;
    }

    private static string NormalizeRangeAddress(string address)
    {
        var separatorIndex = address.IndexOf('!');
        return separatorIndex >= 0 ? address[(separatorIndex + 1)..] : address;
    }

    private sealed record DataFieldConfig(string Field, string? Function, string? Name, string? Format);
}
