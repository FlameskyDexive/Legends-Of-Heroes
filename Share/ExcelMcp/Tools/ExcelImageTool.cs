using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using ET;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace ET.Tools.Excel;

/// <summary>
///     Excel 图片工具 - 支持添加、删除、获取、导出图片。
/// </summary>
public class ExcelImageTool : IExcelTool
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tif", ".tiff"
    };

    public string Description => @"管理 Excel 图片。支持 4 种操作: add, delete, get, extract。

使用示例:
- 添加图片: excel_image(operation='add', path='book.xlsx', imagePath='logo.png', cell='A1', width=200, height=150)
- 删除图片: excel_image(operation='delete', path='book.xlsx', imageIndex=0)
- 获取图片: excel_image(operation='get', path='book.xlsx')
- 导出图片: excel_image(operation='extract', path='book.xlsx', imageIndex=0, exportPath='export.png')";

    public object InputSchema => new
    {
        type = "object",
        properties = new
        {
            operation = new
            {
                type = "string",
                description = @"要执行的操作。
- 'add': 添加图片 (必需: path, imagePath, cell)
- 'delete': 删除图片 (必需: path, imageIndex)
- 'get': 获取图片列表 (必需: path)
- 'extract': 导出图片 (必需: path, imageIndex, exportPath)",
                @enum = new[] { "add", "delete", "get", "extract" }
            },
            path = new
            {
                type = "string",
                description = "Excel 文件路径"
            },
            outputPath = new
            {
                type = "string",
                description = "输出文件路径 (add/delete 可选, 默认为输入文件)"
            },
            sheetIndex = new
            {
                type = "number",
                description = "工作表索引 (从0开始, 默认0)"
            },
            imagePath = new
            {
                type = "string",
                description = "图片文件路径 (add 必需)"
            },
            cell = new
            {
                type = "string",
                description = "图片左上角单元格地址 (add 必需)"
            },
            width = new
            {
                type = "number",
                description = "图片宽度 (像素, 可选)"
            },
            height = new
            {
                type = "number",
                description = "图片高度 (像素, 可选)"
            },
            keepAspectRatio = new
            {
                type = "boolean",
                description = "保持宽高比 (默认 true)"
            },
            imageIndex = new
            {
                type = "number",
                description = "图片索引 (从0开始, delete/extract 必需)"
            },
            exportPath = new
            {
                type = "string",
                description = "导出图片路径 (extract 必需)"
            }
        },
        required = new[] { "operation", "path" }
    };

    public async Task<string> ExecuteAsync(JsonObject? arguments)
    {
        var operation = ArgumentHelper.GetString(arguments, "operation").ToLowerInvariant();
        var path = ArgumentHelper.GetAndValidatePath(arguments);
        var outputPath = ArgumentHelper.GetAndValidateOutputPath(arguments, path);
        var sheetIndex = ArgumentHelper.GetInt(arguments, "sheetIndex", 0);

        return operation switch
        {
            "add" => await AddImageAsync(path, outputPath, sheetIndex, arguments),
            "delete" => await DeleteImageAsync(path, outputPath, sheetIndex, arguments),
            "get" => await GetImagesAsync(path, sheetIndex),
            "extract" => await ExtractImageAsync(path, sheetIndex, arguments),
            _ => throw new ArgumentException($"未知操作: {operation}")
        };
    }

    private Task<string> AddImageAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var imagePath = ArgumentHelper.GetString(arguments, "imagePath");
            SecurityHelper.ValidateFilePath(imagePath, "imagePath", true);
            var cell = ArgumentHelper.GetString(arguments, "cell");
            var width = ArgumentHelper.GetIntNullable(arguments, "width");
            var height = ArgumentHelper.GetIntNullable(arguments, "height");
            var keepAspectRatio = ArgumentHelper.GetBool(arguments, "keepAspectRatio", true);

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"图片文件不存在: {imagePath}");

            ValidateImageFormat(imagePath);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var pictureName = $"Image_{worksheet.Drawings.Count + 1}";
            var picture = worksheet.Drawings.AddPicture(pictureName, new FileInfo(imagePath));

            var address = new ExcelAddress(cell);
            picture.SetPosition(address.Start.Row - 1, 0, address.Start.Column - 1, 0);
            ApplySize(picture, width, height, keepAspectRatio);

            package.SaveAs(new FileInfo(outputPath));
            return $"已在单元格 {cell} 插入图片 (当前共 {GetPictures(worksheet).Count} 张). 输出: {outputPath}";
        });
    }

    private Task<string> DeleteImageAsync(string path, string outputPath, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var imageIndex = ArgumentHelper.GetInt(arguments, "imageIndex");

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var pictures = GetPictures(worksheet);

            if (imageIndex < 0 || imageIndex >= pictures.Count)
                throw new ArgumentException($"图片索引 {imageIndex} 超出范围 (0-{pictures.Count - 1}).");

            var picture = pictures[imageIndex];
            worksheet.Drawings.Remove(picture);

            package.SaveAs(new FileInfo(outputPath));
            var note = pictures.Count > 1 ? "剩余图片索引会重新编号。" : string.Empty;
            return $"已删除第 {imageIndex} 个图片 {note} 输出: {outputPath}";
        });
    }

    private Task<string> GetImagesAsync(string path, int sheetIndex)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var pictures = GetPictures(worksheet);

            if (pictures.Count == 0)
            {
                var empty = new
                {
                    count = 0,
                    worksheetName = worksheet.Name,
                    items = Array.Empty<object>(),
                    message = "未找到图片"
                };
                return JsonSerializer.Serialize(empty, new JsonSerializerOptions { WriteIndented = true });
            }

            var items = pictures.Select((picture, index) => new
            {
                index,
                name = picture.Name,
                location = GetAnchorAddress(picture),
                width = picture.Size.Width,
                height = picture.Size.Height,
                lockAspectRatio = picture.LockAspectRatio,
                rotation = GetRotation(picture)
            }).ToList();

            var result = new
            {
                count = items.Count,
                worksheetName = worksheet.Name,
                items
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        });
    }

    private Task<string> ExtractImageAsync(string path, int sheetIndex, JsonObject? arguments)
    {
        return Task.Run(() =>
        {
            var imageIndex = ArgumentHelper.GetInt(arguments, "imageIndex");
            var exportPath = ArgumentHelper.GetString(arguments, "exportPath");
            SecurityHelper.ValidateFilePath(exportPath, "exportPath", true);

            var directory = Path.GetDirectoryName(exportPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = ExcelHelper.GetWorksheet(package, sheetIndex);
            var pictures = GetPictures(worksheet);

            if (imageIndex < 0 || imageIndex >= pictures.Count)
                throw new ArgumentException($"图片索引 {imageIndex} 超出范围 (0-{pictures.Count - 1}).");

            var picture = pictures[imageIndex];
            if (picture.Image == null)
                throw new InvalidOperationException("无法读取图片数据。");

            picture.Image.Save(exportPath, picture.Image.RawFormat);
            return $"已将图片 #{imageIndex} 导出到 {exportPath}";
        });
    }

    private static void ApplySize(ExcelPicture picture, int? width, int? height, bool keepAspectRatio)
    {
        if (!width.HasValue && !height.HasValue)
        {
            picture.LockAspectRatio = keepAspectRatio;
            return;
        }

        var currentWidth = (int)picture.Size.Width;
        var currentHeight = (int)picture.Size.Height;

        if (width.HasValue && !height.HasValue && keepAspectRatio && currentWidth > 0)
        {
            height = (int)Math.Round((double)width.Value / currentWidth * currentHeight);
        }
        else if (height.HasValue && !width.HasValue && keepAspectRatio && currentHeight > 0)
        {
            width = (int)Math.Round((double)height.Value / currentHeight * currentWidth);
        }
        else
        {
            width ??= currentWidth;
            height ??= currentHeight;
        }

        picture.SetSize(width.Value, height.Value);
        picture.LockAspectRatio = keepAspectRatio;
    }

    private static void ValidateImageFormat(string imagePath)
    {
        var extension = Path.GetExtension(imagePath);
        if (string.IsNullOrEmpty(extension) || !SupportedExtensions.Contains(extension))
            throw new ArgumentException(
                $"不支持的图片格式: {extension}. 仅支持: {string.Join(", ", SupportedExtensions)}");
    }

    private static List<ExcelPicture> GetPictures(ExcelWorksheet worksheet)
    {
        return worksheet.Drawings
            .Where(d => d is ExcelPicture)
            .Cast<ExcelPicture>()
            .OrderBy(p => p.From?.Row ?? 0)
            .ThenBy(p => p.From?.Column ?? 0)
            .ToList();
    }

    private static string? GetAnchorAddress(ExcelPicture picture)
    {
        if (picture.From == null) return null;
        return ExcelCellBase.GetAddress(picture.From.Row + 1, picture.From.Column + 1);
    }

    private static object? GetRotation(ExcelPicture picture)
    {
        var property = picture.GetType().GetProperty("Rotation");
        return property?.GetValue(picture);
    }
}
