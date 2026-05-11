using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;

namespace ET.Tools.Excel;

/// <summary>
///     Converts between Excel A1 cell addresses and zero-based row/column indexes.
/// </summary>
public class ExcelGetCellAddressTool : IExcelTool
{
    private const int MaxExcelRows = 1_048_576;
    private const int MaxExcelColumns = 16_384;

    public string Description => @"在 A1 单元格地址与行/列索引 (0-based) 之间转换。

使用示例:
- excel_get_cell_address(cellAddress='B2') 返回行列索引
- excel_get_cell_address(row=1, column=1) 返回单元格地址
- excel_get_cell_address(cellAddress='AA100') 可用于校验地址";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            cellAddress = new
            {
                type = "string",
                description = "A1 格式的单元格地址 (如 'A1', 'B2', 'AA100')。与 row/column 参数互斥"
            },
            row = new
            {
                type = "number",
                description = "行索引，0-based，范围 0 - 1048575。与 column 参数搭配使用"
            },
            column = new
            {
                type = "number",
                description = "列索引，0-based，范围 0 - 16383。与 row 参数搭配使用"
            }
        },
        required = Array.Empty<string>()
    };

    public Task<string> ExecuteAsync(JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var cellAddress = ArgumentHelper.GetStringNullable(arguments, "cellAddress");
            var row = ArgumentHelper.GetIntNullable(arguments, "row");
            var column = ArgumentHelper.GetIntNullable(arguments, "column");

            var hasCellAddress = !string.IsNullOrWhiteSpace(cellAddress);
            var hasRowColumn = row.HasValue && column.HasValue;
            var hasPartialRowColumn = row.HasValue ^ column.HasValue;

            if (hasPartialRowColumn)
                throw new ArgumentException("row 和 column 参数必须成对提供");

            if (hasCellAddress && hasRowColumn)
                throw new ArgumentException("cellAddress 与 row/column 参数不能同时出现");

            if (!hasCellAddress && !hasRowColumn)
                throw new ArgumentException("必须提供 cellAddress 或 row + column 参数");

            int finalRow;
            int finalColumn;

            if (hasRowColumn)
            {
                finalRow = row!.Value;
                finalColumn = column!.Value;
            }
            else
            {
                (finalRow, finalColumn) = ParseCellAddress(cellAddress!);
            }

            ValidateIndexBounds(finalRow, finalColumn);

            var address = ExcelCellBase.GetAddress(finalRow + 1, finalColumn + 1);
            return $"{address} = Row {finalRow}, Column {finalColumn}";
        });
    }

    private static (int Row, int Column) ParseCellAddress(string cellAddress)
    {
        var normalized = cellAddress.Replace("$", string.Empty).Trim();
        var delimiterIndex = normalized.LastIndexOf('!');
        if (delimiterIndex >= 0 && delimiterIndex < normalized.Length - 1)
            normalized = normalized[(delimiterIndex + 1)..];

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("cellAddress 格式无效");

        normalized = normalized.ToUpperInvariant();

        var splitIndex = 0;
        while (splitIndex < normalized.Length && char.IsLetter(normalized[splitIndex])) splitIndex++;

        if (splitIndex == 0 || splitIndex == normalized.Length)
            throw new ArgumentException("cellAddress 必须包含字母列和数字行");

        var columnPart = normalized[..splitIndex];
        var rowPart = normalized[splitIndex..];

        if (!int.TryParse(rowPart, out var row1Based) || row1Based <= 0)
            throw new ArgumentException("cellAddress 中的行号必须为正整数");

        var column1Based = 0;
        foreach (var letter in columnPart)
        {
            if (letter is < 'A' or > 'Z')
                throw new ArgumentException("cellAddress 仅支持英文字母列");
            column1Based = column1Based * 26 + (letter - 'A' + 1);
        }

        return (row1Based - 1, column1Based - 1);
    }

    private static void ValidateIndexBounds(int row, int column)
    {
        if (row < 0 || row >= MaxExcelRows)
            throw new ArgumentException($"row 超出范围 (0-{MaxExcelRows - 1})，当前值: {row}");

        if (column < 0 || column >= MaxExcelColumns)
            throw new ArgumentException($"column 超出范围 (0-{MaxExcelColumns - 1})，当前值: {column}");
    }
}
