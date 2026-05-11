using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     公式操作工具 - 支持添加、获取、计算、数组公式等操作
/// </summary>
public class ExcelFormulaTool : IExcelTool
{
    public string Description => @"管理 Excel 公式。支持 6 种操作: add, get, get_result, calculate, set_array, get_array。

使用示例:
- 添加公式: excel_formula(operation='add', path='book.xlsx', cell='A1', formula='=SUM(B1:B10)')
- 获取公式: excel_formula(operation='get', path='book.xlsx')
- 获取公式结果: excel_formula(operation='get_result', path='book.xlsx', cell='A1')
- 计算所有公式: excel_formula(operation='calculate', path='book.xlsx')
- 设置数组公式: excel_formula(operation='set_array', path='book.xlsx', range='A1:A10', formula='=B1:B10*2')
- 获取数组公式: excel_formula(operation='get_array', path='book.xlsx', cell='A1')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'add': 添加公式到单元格 (必需参数: path, cell, formula)
- 'get': 获取工作表中的公式 (必需参数: path)
- 'get_result': 获取公式计算结果 (必需参数: path, cell)
- 'calculate': 计算所有公式 (必需参数: path)
- 'set_array': 设置数组公式 (必需参数: path, range, formula)
- 'get_array': 获取数组公式信息 (必需参数: path, cell)",
                @enum = new[] { "add", "get", "get_result", "calculate", "set_array", "get_array" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 用于 add/calculate/set_array 操作, 默认为输入路径)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 可选, 默认: 0)"
            },
            cell = new
            {
                type = "string",
                description = "单元格引用 (例如 'A1', 用于 add/get_result/get_array)"
            },
            range = new
            {
                type = "string",
                description = "单元格范围 (例如 'A1:C10', 可选用于 get, 必需用于 set_array)"
            },
            formula = new
            {
                type = "string",
                description = "公式 (例如 '=SUM(A1:A10)', 用于 add/set_array)"
            },
            calculateBeforeRead = new
            {
                type = "boolean",
                description = "读取前是否计算公式 (可选, 用于 get_result, 默认: true)"
            },
            autoCalculate = new
            {
                type = "boolean",
                description = "添加后是否自动计算公式 (可选, 用于 add/set_array, 默认: true). 批量操作时设为 false 可提高性能"
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
            "add" => await AddFormulaAsync(path, outputPath, sheetIndex, arguments),
            "get" => await GetFormulasAsync(path, sheetIndex, arguments),
            "get_result" => await GetFormulaResultAsync(path, sheetIndex, arguments),
            "calculate" => await CalculateFormulasAsync(path, outputPath),
            "set_array" => await SetArrayFormulaAsync(path, outputPath, sheetIndex, arguments),
            "get_array" => await GetArrayFormulaAsync(path, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> AddFormulaAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetString(arguments, "cell");
            var formula = ArgumentHelper.GetString(arguments, "formula");
            var autoCalculate = ArgumentHelper.GetBool(arguments, "autoCalculate", true);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var cellObj = worksheet.Cells[cell];
            cellObj.Formula = formula;

            string? warningMessage = null;
            if (autoCalculate)
            {
                package.Workbook.Calculate();
                var value = cellObj.Value;

                // 检查是否有错误
                if (value is ExcelErrorValue errorValue)
                {
                    warningMessage = $" 警告: {errorValue.Type}";
                    warningMessage += errorValue.Type switch
                    {
                        eErrorType.Name => " (无效的函数名)",
                        eErrorType.Value => " (参数类型不正确)",
                        eErrorType.Ref => " (无效的单元格引用)",
                        eErrorType.Div0 => " (除以零)",
                        eErrorType.Null => " (空引用)",
                        eErrorType.Num => " (数值错误)",
                        eErrorType.NA => " (值不可用)",
                        _ => ""
                    };
                }
            }

            package.SaveAs(new FileInfo(outputPath));

            var result = $"公式已添加到 {cell}: {formula}";
            if (!string.IsNullOrEmpty(warningMessage)) result += $".{warningMessage}";
            result += $". 输出: {outputPath}";
            return result;
        });
    }

    private Task<string> GetFormulasAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetStringNullable(arguments, "range");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            int startRow, endRow, startCol, endCol;

            if (!string.IsNullOrEmpty(range))
            {
                var rangeObj = worksheet.Cells[range];
                startRow = rangeObj.Start.Row;
                endRow = rangeObj.End.Row;
                startCol = rangeObj.Start.Column;
                endCol = rangeObj.End.Column;
            }
            else
            {
                startRow = 1;
                endRow = worksheet.Dimension?.End.Row ?? 1;
                startCol = 1;
                endCol = worksheet.Dimension?.End.Column ?? 1;
            }

            var formulaList = new List<object>();
            for (var row = startRow; row <= endRow && row <= 10000; row++)
            for (var col = startCol; col <= endCol && col <= 1000; col++)
            {
                var cell = worksheet.Cells[row, col];
                if (!string.IsNullOrEmpty(cell.Formula))
                    formulaList.Add(new
                    {
                        cell = cell.Address,
                        formula = cell.Formula,
                        value = cell.Value?.ToString() ?? "(计算中)"
                    });
            }

            if (formulaList.Count == 0)
            {
                var emptyResult = new
                {
                    count = 0,
                    worksheetName = worksheet.Name,
                    items = Array.Empty<object>(),
                    message = "未找到公式"
                };
                return JsonSerializer.Serialize(emptyResult, new JsonSerializerOptions { WriteIndented = true });
            }

            var result = new
            {
                count = formulaList.Count,
                worksheetName = worksheet.Name,
                items = formulaList
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> GetFormulaResultAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetString(arguments, "cell");
            var calculateBeforeRead = ArgumentHelper.GetBool(arguments, "calculateBeforeRead", true);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var cellObj = worksheet.Cells[cell];

            if (calculateBeforeRead) package.Workbook.Calculate();

            var calculatedValue = cellObj.Value;
            var valueType = "Unknown";

            if (calculatedValue != null)
            {
                valueType = calculatedValue switch
                {
                    ExcelErrorValue => "Error",
                    double => "Number",
                    int => "Number",
                    bool => "Boolean",
                    DateTime => "DateTime",
                    string => "String",
                    _ => calculatedValue.GetType().Name
                };
            }

            var result = new
            {
                cell,
                formula = cellObj.Formula,
                calculatedValue = calculatedValue?.ToString() ?? "(空)",
                valueType
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> CalculateFormulasAsync(string path, string outputPath)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            package.Workbook.Calculate();
            package.SaveAs(new FileInfo(outputPath));

            return $"公式已计算. 输出: {outputPath}";
        });
    }

    private Task<string> SetArrayFormulaAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var formula = ArgumentHelper.GetString(arguments, "formula");
            var autoCalculate = ArgumentHelper.GetBool(arguments, "autoCalculate", true);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            var rangeObj = worksheet.Cells[range];

            // EPPlus 使用 CreateArrayFormula 方法
            var cleanFormula = formula.TrimStart('{').TrimEnd('}');
            if (!cleanFormula.StartsWith("="))
                cleanFormula = "=" + cleanFormula;

            rangeObj.CreateArrayFormula(cleanFormula);

            if (autoCalculate) package.Workbook.Calculate();

            package.SaveAs(new FileInfo(outputPath));
            return $"数组公式已设置到范围 {range}. 输出: {outputPath}";
        });
    }

    private Task<string> GetArrayFormulaAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cell = ArgumentHelper.GetString(arguments, "cell");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var cellObj = worksheet.Cells[cell];

            var isArrayFormula = cellObj.IsArrayFormula;

            if (!isArrayFormula)
            {
                var notFoundResult = new
                {
                    cell,
                    isArrayFormula = false,
                    message = "该单元格没有数组公式"
                };
                return JsonSerializer.Serialize(notFoundResult, new JsonSerializerOptions { WriteIndented = true });
            }

            var formula = cellObj.Formula;

            var result = new
            {
                cell,
                isArrayFormula = true,
                formula = formula ?? "(空)"
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }
}
