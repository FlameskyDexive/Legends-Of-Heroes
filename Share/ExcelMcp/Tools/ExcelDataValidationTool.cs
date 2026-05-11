using System.Text.Json;
using System.Text.Json.Nodes;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.DataValidation.Contracts;
using ET;

namespace ET.Tools.Excel;

/// <summary>
///     数据验证工具 - 支持添加、编辑、删除、获取验证规则等操作
/// </summary>
public class ExcelDataValidationTool : IExcelTool
{
    public string Description => @"管理 Excel 数据验证。支持 5 种操作: add, edit, delete, get, set_messages。

使用示例:
- 添加列表验证: excel_data_validation(operation='add', path='book.xlsx', range='A1:A10', validationType='List', formula1='1,2,3')
- 添加数值范围: excel_data_validation(operation='add', path='book.xlsx', range='B1:B10', validationType='WholeNumber', operatorType='Between', formula1='0', formula2='100')
- 添加大于条件: excel_data_validation(operation='add', path='book.xlsx', range='C1:C10', validationType='WholeNumber', operatorType='GreaterThan', formula1='0')
- 编辑验证: excel_data_validation(operation='edit', path='book.xlsx', validationIndex=0, inputMessage='请输入值')
- 删除验证: excel_data_validation(operation='delete', path='book.xlsx', validationIndex=0)
- 获取验证: excel_data_validation(operation='get', path='book.xlsx')
- 设置消息: excel_data_validation(operation='set_messages', path='book.xlsx', validationIndex=0, inputMessage='请输入值', errorMessage='无效的值')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'add': 添加数据验证 (必需参数: path, range, validationType, formula1)
- 'edit': 编辑数据验证 (必需参数: path, validationIndex)
- 'delete': 删除数据验证 (必需参数: path, validationIndex)
- 'get': 获取数据验证信息 (必需参数: path)
- 'set_messages': 设置输入/错误消息 (必需参数: path, validationIndex)",
                @enum = new[] { "add", "edit", "delete", "get", "set_messages" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径 (所有操作必需)"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (可选, 用于 add/edit/delete/set_messages 操作, 默认为输入路径)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 可选, 默认: 0)"
            },
            range = new
            {
                type = "string",
                description = "应用验证的单元格范围 (例如 'A1:A10', 用于 add 操作)"
            },
            validationIndex = new
            {
                type = "number",
                description = "数据验证索引 (从0开始, 用于 edit, delete, set_messages 操作)"
            },
            validationType = new
            {
                type = "string",
                description = "验证类型: 'WholeNumber', 'Decimal', 'List', 'Date', 'Time', 'TextLength', 'Custom'",
                @enum = new[] { "WholeNumber", "Decimal", "List", "Date", "Time", "TextLength", "Custom" }
            },
            operatorType = new
            {
                type = "string",
                description = "运算符类型。默认: 如果提供了 formula2 则为 'Between', 否则为 'Equal'. 对于 List 类型忽略此参数",
                @enum = new[]
                    { "Between", "Equal", "NotEqual", "GreaterThan", "LessThan", "GreaterOrEqual", "LessOrEqual" }
            },
            formula1 = new
            {
                type = "string",
                description = "第一个公式/值 (例如 '1,2,3' 用于 List, '0' 用于最小值, 用于 add 必需)"
            },
            formula2 = new
            {
                type = "string",
                description = "第二个公式/值 ('Between' 运算符必需, 其他运算符可选)"
            },
            inCellDropDown = new
            {
                type = "boolean",
                description = "在单元格中显示下拉列表 (仅用于 List 类型, 可选, 默认: true)"
            },
            errorMessage = new
            {
                type = "string",
                description = "验证失败时显示的错误消息 (可选)"
            },
            inputMessage = new
            {
                type = "string",
                description = "选中单元格时显示的输入消息 (可选)"
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
            "add" => await AddDataValidationAsync(path, outputPath, sheetIndex, arguments),
            "edit" => await EditDataValidationAsync(path, outputPath, sheetIndex, arguments),
            "delete" => await DeleteDataValidationAsync(path, outputPath, sheetIndex, arguments),
            "get" => await GetDataValidationAsync(path, sheetIndex),
            "set_messages" => await SetMessagesAsync(path, outputPath, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> AddDataValidationAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var range = ArgumentHelper.GetString(arguments, "range");
            var validationType = ArgumentHelper.GetString(arguments, "validationType");
            var formula1 = ArgumentHelper.GetString(arguments, "formula1");
            var formula2 = ArgumentHelper.GetStringNullable(arguments, "formula2");
            var operatorTypeStr = ArgumentHelper.GetStringNullable(arguments, "operatorType");
            var inCellDropDown = ArgumentHelper.GetBool(arguments, "inCellDropDown", true);
            var errorMessage = ArgumentHelper.GetStringNullable(arguments, "errorMessage");
            var inputMessage = ArgumentHelper.GetStringNullable(arguments, "inputMessage");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);

            IExcelDataValidation validation;

            switch (validationType.ToLower())
            {
                case "list":
                    var listValidation = worksheet.DataValidations.AddListValidation(range);
                    // 解析逗号分隔的列表项
                    var items = formula1.Split(',');
                    foreach (var item in items)
                    {
                        listValidation.Formula.Values.Add(item.Trim());
                    }
                    // EPPlus 使用 AllowBlank 和 ShowInputMessage 来控制行为
                    // ShowDropdown 不可单独设置，列表验证默认显示下拉
                    validation = listValidation;
                    break;

                case "wholenumber":
                    var intValidation = worksheet.DataValidations.AddIntegerValidation(range);
                    SetIntegerValidation(intValidation, formula1, formula2, operatorTypeStr);
                    validation = intValidation;
                    break;

                case "decimal":
                    var decValidation = worksheet.DataValidations.AddDecimalValidation(range);
                    SetDecimalValidation(decValidation, formula1, formula2, operatorTypeStr);
                    validation = decValidation;
                    break;

                case "date":
                    var dateValidation = worksheet.DataValidations.AddDateTimeValidation(range);
                    SetDateTimeValidation(dateValidation, formula1, formula2, operatorTypeStr);
                    validation = dateValidation;
                    break;

                case "time":
                    var timeValidation = worksheet.DataValidations.AddTimeValidation(range);
                    SetTimeValidation(timeValidation, formula1, formula2, operatorTypeStr);
                    validation = timeValidation;
                    break;

                case "textlength":
                    var textValidation = worksheet.DataValidations.AddTextLengthValidation(range);
                    SetTextLengthValidation(textValidation, formula1, formula2, operatorTypeStr);
                    validation = textValidation;
                    break;

                case "custom":
                    var customValidation = worksheet.DataValidations.AddCustomValidation(range);
                    customValidation.Formula.ExcelFormula = formula1;
                    validation = customValidation;
                    break;

                default:
                    throw new ArgumentException($"不支持的验证类型: {validationType}");
            }

            // 设置消息
            if (!string.IsNullOrEmpty(errorMessage))
            {
                validation.ShowErrorMessage = true;
                validation.ErrorTitle = "验证错误";
                validation.Error = errorMessage;
            }

            if (!string.IsNullOrEmpty(inputMessage))
            {
                validation.ShowInputMessage = true;
                validation.PromptTitle = "输入提示";
                validation.Prompt = inputMessage;
            }

            package.SaveAs(new FileInfo(outputPath));

            var validationIndex = worksheet.DataValidations.Count - 1;
            return $"已添加数据验证到范围 {range} (类型: {validationType}, 索引: {validationIndex}). 输出: {outputPath}";
        });
    }

    private void SetIntegerValidation(IExcelDataValidationInt validation,
        string formula1, string? formula2, string? operatorTypeStr)
    {
        var op = ParseOperator(operatorTypeStr, formula2);
        validation.Operator = op;
        validation.Formula.Value = int.TryParse(formula1, out var val1) ? val1 : 0;
        if (!string.IsNullOrEmpty(formula2) && int.TryParse(formula2, out var val2))
        {
            validation.Formula2.Value = val2;
        }
    }

    private void SetDecimalValidation(IExcelDataValidationDecimal validation,
        string formula1, string? formula2, string? operatorTypeStr)
    {
        var op = ParseOperator(operatorTypeStr, formula2);
        validation.Operator = op;
        validation.Formula.Value = double.TryParse(formula1, out var val1) ? val1 : 0;
        if (!string.IsNullOrEmpty(formula2) && double.TryParse(formula2, out var val2))
        {
            validation.Formula2.Value = val2;
        }
    }

    private void SetTextLengthValidation(IExcelDataValidationInt validation,
        string formula1, string? formula2, string? operatorTypeStr)
    {
        var op = ParseOperator(operatorTypeStr, formula2);
        validation.Operator = op;
        validation.Formula.Value = int.TryParse(formula1, out var val1) ? val1 : 0;
        if (!string.IsNullOrEmpty(formula2) && int.TryParse(formula2, out var val2))
        {
            validation.Formula2.Value = val2;
        }
    }

    private void SetDateTimeValidation(IExcelDataValidationDateTime validation,
        string formula1, string? formula2, string? operatorTypeStr)
    {
        var op = ParseOperator(operatorTypeStr, formula2);
        validation.Operator = op;
        if (DateTime.TryParse(formula1, out var date1))
            validation.Formula.Value = date1;
        if (!string.IsNullOrEmpty(formula2) && DateTime.TryParse(formula2, out var date2))
            validation.Formula2.Value = date2;
    }

    private void SetTimeValidation(IExcelDataValidationTime validation,
        string formula1, string? formula2, string? operatorTypeStr)
    {
        var op = ParseOperator(operatorTypeStr, formula2);
        validation.Operator = op;
        if (TimeSpan.TryParse(formula1, out var time1))
            validation.Formula.Value = new ExcelTime() { Hour = time1.Hours, Minute = time1.Minutes, Second = time1.Seconds };
        if (!string.IsNullOrEmpty(formula2) && TimeSpan.TryParse(formula2, out var time2))
            validation.Formula2.Value = new ExcelTime() { Hour = time2.Hours, Minute = time2.Minutes, Second = time2.Seconds };
    }

    private ExcelDataValidationOperator ParseOperator(string? operatorTypeStr, string? formula2)
    {
        if (!string.IsNullOrEmpty(operatorTypeStr))
        {
            return operatorTypeStr.ToLower() switch
            {
                "between" => ExcelDataValidationOperator.between,
                "notbetween" => ExcelDataValidationOperator.notBetween,
                "equal" => ExcelDataValidationOperator.equal,
                "notequal" => ExcelDataValidationOperator.notEqual,
                "greaterthan" => ExcelDataValidationOperator.greaterThan,
                "lessthan" => ExcelDataValidationOperator.lessThan,
                "greaterorequal" => ExcelDataValidationOperator.greaterThanOrEqual,
                "lessorequal" => ExcelDataValidationOperator.lessThanOrEqual,
                _ => throw new ArgumentException($"不支持的运算符类型: {operatorTypeStr}")
            };
        }

        return !string.IsNullOrEmpty(formula2)
            ? ExcelDataValidationOperator.between
            : ExcelDataValidationOperator.equal;
    }

    private Task<string> EditDataValidationAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var validationIndex = ArgumentHelper.GetInt(arguments, "validationIndex");
            var errorMessage = ArgumentHelper.GetStringNullable(arguments, "errorMessage");
            var inputMessage = ArgumentHelper.GetStringNullable(arguments, "inputMessage");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var validations = worksheet.DataValidations;

            if (validationIndex < 0 || validationIndex >= validations.Count)
                throw new ArgumentException($"数据验证索引 {validationIndex} 超出范围 (共 {validations.Count} 个验证)");

            var validation = validations[validationIndex];
            var changes = new List<string>();

            if (errorMessage != null)
            {
                validation.Error = errorMessage;
                validation.ShowErrorMessage = !string.IsNullOrEmpty(errorMessage);
                changes.Add($"ErrorMessage={errorMessage}");
            }

            if (inputMessage != null)
            {
                validation.Prompt = inputMessage;
                validation.ShowInputMessage = !string.IsNullOrEmpty(inputMessage);
                changes.Add($"InputMessage={inputMessage}");
            }

            package.SaveAs(new FileInfo(outputPath));

            var changesStr = changes.Count > 0 ? string.Join(", ", changes) : "无更改";
            return $"已编辑数据验证 #{validationIndex} ({changesStr}). 输出: {outputPath}";
        });
    }

    private Task<string> DeleteDataValidationAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var validationIndex = ArgumentHelper.GetInt(arguments, "validationIndex");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var validations = worksheet.DataValidations;

            if (validationIndex < 0 || validationIndex >= validations.Count)
                throw new ArgumentException($"数据验证索引 {validationIndex} 超出范围 (共 {validations.Count} 个验证)");

            // EPPlus 使用 Remove 方法，传入验证对象
            var validationToRemove = validations[validationIndex];
            validations.Remove(validationToRemove);

            package.SaveAs(new FileInfo(outputPath));

            return $"已删除数据验证 #{validationIndex} (剩余: {validations.Count}). 输出: {outputPath}";
        });
    }

    private Task<string> GetDataValidationAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var validations = worksheet.DataValidations;

            var validationList = new List<object>();
            for (var i = 0; i < validations.Count; i++)
            {
                var validation = validations[i];
                validationList.Add(new
                {
                    index = i,
                    type = validation.ValidationType.Type.ToString(),
                    address = validation.Address.Address,
                    errorMessage = validation.Error,
                    inputMessage = validation.Prompt,
                    showError = validation.ShowErrorMessage,
                    showInput = validation.ShowInputMessage
                });
            }

            var result = new
            {
                count = validations.Count,
                worksheetName = worksheet.Name,
                items = validationList
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> SetMessagesAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var validationIndex = ArgumentHelper.GetInt(arguments, "validationIndex");
            var errorMessage = ArgumentHelper.GetStringNullable(arguments, "errorMessage");
            var inputMessage = ArgumentHelper.GetStringNullable(arguments, "inputMessage");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var validations = worksheet.DataValidations;

            if (validationIndex < 0 || validationIndex >= validations.Count)
                throw new ArgumentException($"数据验证索引 {validationIndex} 超出范围 (共 {validations.Count} 个验证)");

            var validation = validations[validationIndex];
            var changes = new List<string>();

            if (errorMessage != null)
            {
                validation.Error = errorMessage;
                validation.ShowErrorMessage = !string.IsNullOrEmpty(errorMessage);
                changes.Add($"ErrorMessage={errorMessage}");
            }

            if (inputMessage != null)
            {
                validation.Prompt = inputMessage;
                validation.ShowInputMessage = !string.IsNullOrEmpty(inputMessage);
                changes.Add($"InputMessage={inputMessage}");
            }

            package.SaveAs(new FileInfo(outputPath));

            var changesStr = changes.Count > 0 ? string.Join(", ", changes) : "无更改";
            return $"已更新数据验证 #{validationIndex} 消息 ({changesStr}). 输出: {outputPath}";
        });
    }
}
