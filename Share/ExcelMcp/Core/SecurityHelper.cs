using System.Text.RegularExpressions;

namespace ET;

/// <summary>
///     Security helper class for file path and name validation.
/// </summary>
public static class SecurityHelper
{
    private const int MaxPathLength = 260;
    private const int MaxFileNameLength = 255;
    private const int MaxArraySize = 1000;
    private const int MaxStringLength = 10000;

    /// <summary>
    ///     Sanitizes a file name to prevent path traversal attacks.
    /// </summary>
    public static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return "file";

        if (fileName.Length > MaxFileNameLength) fileName = fileName[..MaxFileNameLength];

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

        sanitized = sanitized.Replace("..", string.Empty)
            .Replace("\\", "_")
            .Replace("/", "_")
            .Replace(":", "_");
        sanitized = Regex.Replace(sanitized, @"^\s+|\s+$", string.Empty);
        sanitized = sanitized.Trim('.', ' ');

        if (string.IsNullOrWhiteSpace(sanitized)) sanitized = "file";

        if (sanitized.Length > MaxFileNameLength) sanitized = sanitized[..MaxFileNameLength];

        return sanitized;
    }

    /// <summary>
    ///     Validates that a file path is safe and doesn't contain path traversal attempts.
    /// </summary>
    public static bool IsSafeFilePath(string filePath, bool allowAbsolutePaths = false)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return false;

        if (filePath.Length > MaxPathLength) return false;

        if (filePath.Contains("..", StringComparison.Ordinal) || filePath.Contains('~')) return false;

        if (filePath.Contains("//", StringComparison.Ordinal) || filePath.Contains("\\\\", StringComparison.Ordinal))
            return false;

        if (Path.IsPathRooted(filePath))
        {
            if (!allowAbsolutePaths) return false;

            try
            {
                var fullPath = Path.GetFullPath(filePath);
                if (fullPath.Contains("..", StringComparison.Ordinal)) return false;
            }
            catch
            {
                return false;
            }
        }

        if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0) return false;

        return true;
    }

    /// <summary>
    ///     Validates and sanitizes a file path, throwing exception if invalid.
    /// </summary>
    public static string ValidateFilePath(string filePath, string paramName = "path", bool allowAbsolutePaths = false)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException($"{paramName} cannot be null or empty");

        if (!IsSafeFilePath(filePath, allowAbsolutePaths))
            throw new ArgumentException($"{paramName} contains invalid characters or path traversal attempts");

        return filePath;
    }

    /// <summary>
    ///     Validates and sanitizes a file name pattern (for use in split/export tools)
    /// </summary>
    public static string SanitizeFileNamePattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern)) return "file_{index}";

        if (pattern.Length > MaxFileNameLength) pattern = pattern[..MaxFileNameLength];

        var sanitized = pattern.Replace("\\", "_", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .Replace("..", string.Empty, StringComparison.Ordinal)
            .Replace(":", "_", StringComparison.Ordinal)
            .Trim('.', ' ');

        if (string.IsNullOrWhiteSpace(sanitized)) sanitized = "file_{index}";

        return sanitized;
    }

    /// <summary>
    ///     Validates array size to prevent resource exhaustion.
    /// </summary>
    public static void ValidateArraySize<T>(IEnumerable<T> array, string paramName = "array", int? maxSize = null)
    {
        var count = array.Count();
        var limit = maxSize ?? MaxArraySize;

        if (count > limit) throw new ArgumentException($"{paramName} exceeds maximum allowed size of {limit}");
    }

    /// <summary>
    ///     Validates string length to prevent resource exhaustion.
    /// </summary>
    public static void ValidateStringLength(string value, string paramName = "value", int? maxLength = null)
    {
        var limit = maxLength ?? MaxStringLength;

        if (value.Length > limit) throw new ArgumentException($"{paramName} exceeds maximum allowed length of {limit}");
    }
}
