using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ET;

/// <summary>
///     Helper class for argument parsing, type conversion, and path validation
/// </summary>
public static class ArgumentHelper
{
    #region GetInt - Integer Methods

    public static int GetInt(JsonObject? arguments, string key)
    {
        return GetInt(arguments, key, key);
    }

    public static int? GetInt(JsonObject? arguments, string key, bool required)
    {
        if (required)
            throw new ArgumentException("Use GetInt(arguments, key) for required parameters");
        return GetIntNullable(arguments, key);
    }

    public static int GetInt(JsonObject? arguments, string key, int defaultValue)
    {
        return GetInt(arguments, key, null, key, false, defaultValue);
    }

    public static int GetInt(JsonObject? arguments, string key, string paramName)
    {
        return GetInt(arguments, key, null, paramName);
    }

    public static int? GetInt(JsonObject? arguments, string key, string paramName, bool required)
    {
        return GetInt(arguments, key, null, paramName, required);
    }

    public static int GetInt(JsonObject? arguments, string primaryName, string? alternateName, string paramName,
        bool required = true, int? defaultValue = null)
    {
        var result = GetIntNullable(arguments, primaryName, alternateName, paramName);
        if (result.HasValue)
            return result.Value;

        if (required)
            throw new ArgumentException($"{paramName} is required");
        if (defaultValue.HasValue)
            return defaultValue.Value;
        throw new ArgumentException($"{paramName} is required");
    }

    public static int? GetIntNullable(JsonObject? arguments, string key)
    {
        return GetIntNullable(arguments, key, null, key);
    }

    public static int? GetIntNullable(JsonObject? arguments, string key, string paramName)
    {
        return GetIntNullable(arguments, key, null, paramName);
    }

    public static int? GetIntNullable(JsonObject? arguments, string primaryName, string? alternateName,
        string paramName)
    {
        JsonNode? node = null;
        if (arguments != null)
            node = arguments[primaryName] ?? (alternateName != null ? arguments[alternateName] : null);

        if (node == null)
            return null;

        if (node.GetValueKind() == JsonValueKind.String)
        {
            var str = node.GetValue<string>();
            if (string.IsNullOrEmpty(str) || !int.TryParse(str, out var result))
                throw new ArgumentException($"{paramName} must be a valid integer");
            return result;
        }

        if (node.GetValueKind() == JsonValueKind.Number) return node.GetValue<int>();

        throw new ArgumentException($"{paramName} must be a valid integer");
    }

    #endregion

    #region GetString - String Methods

    public static string GetString(JsonObject? arguments, string key)
    {
        return GetString(arguments, key, key);
    }

    public static string? GetString(JsonObject? arguments, string key, bool required)
    {
        if (required)
            throw new ArgumentException("Use GetString(arguments, key) for required parameters");
        return GetStringNullable(arguments, key);
    }

    public static string GetString(JsonObject? arguments, string key, string defaultValue)
    {
        return GetString(arguments, key, key, false, defaultValue);
    }

    public static string GetString(JsonObject? arguments, string key, string paramName, bool required,
        string? defaultValue = null)
    {
        return GetString(arguments, key, null, paramName, required, defaultValue);
    }

    public static string GetString(JsonObject? arguments, string primaryName, string? alternateName, string paramName,
        bool required = true, string? defaultValue = null)
    {
        var result = GetStringNullable(arguments, primaryName, alternateName, paramName);
        if (!string.IsNullOrEmpty(result))
            return result;

        if (required)
            throw new ArgumentException($"{paramName} is required");
        return defaultValue ?? string.Empty;
    }

    public static string? GetStringNullable(JsonObject? arguments, string key)
    {
        return GetStringNullable(arguments, key, null, key);
    }

    public static string? GetStringNullable(JsonObject? arguments, string key, string paramName)
    {
        return GetStringNullable(arguments, key, null, paramName);
    }

    public static string? GetStringNullable(JsonObject? arguments, string primaryName, string? alternateName,
        string _)
    {
        JsonNode? node = null;
        if (arguments != null)
            node = arguments[primaryName] ?? (alternateName != null ? arguments[alternateName] : null);

        return node?.GetValue<string>();
    }

    #endregion

    #region GetBool - Boolean Methods

    public static bool GetBool(JsonObject? arguments, string key)
    {
        return GetBool(arguments, key, key);
    }

    public static bool GetBool(JsonObject? arguments, string key, bool defaultValue)
    {
        return GetBool(arguments, key, key, defaultValue);
    }

    public static bool GetBool(JsonObject? arguments, string key, string paramName)
    {
        var result = GetBoolNullable(arguments, key, null, paramName);
        if (!result.HasValue)
            throw new ArgumentException($"{paramName} is required");
        return result.Value;
    }

    public static bool GetBool(JsonObject? arguments, string key, string paramName, bool defaultValue)
    {
        var result = GetBoolNullable(arguments, key, null, paramName);
        return result ?? defaultValue;
    }

    public static bool? GetBoolNullable(JsonObject? arguments, string key)
    {
        return GetBoolNullable(arguments, key, null, key);
    }

    public static bool? GetBoolNullable(JsonObject? arguments, string key, string paramName)
    {
        return GetBoolNullable(arguments, key, null, paramName);
    }

    public static bool? GetBoolNullable(JsonObject? arguments, string primaryName, string? alternateName,
        string paramName)
    {
        JsonNode? node = null;
        if (arguments != null)
            node = arguments[primaryName] ?? (alternateName != null ? arguments[alternateName] : null);

        if (node == null)
            return null;

        if (node.GetValueKind() == JsonValueKind.True || node.GetValueKind() == JsonValueKind.False)
            return node.GetValue<bool>();

        if (node.GetValueKind() == JsonValueKind.String)
        {
            var str = node.GetValue<string>();
            if (bool.TryParse(str, out var result))
                return result;
            throw new ArgumentException($"{paramName} must be a valid boolean");
        }

        if (node.GetValueKind() == JsonValueKind.Number)
        {
            var num = node.GetValue<int>();
            return num != 0;
        }

        throw new ArgumentException($"{paramName} must be a valid boolean");
    }

    #endregion

    #region GetArray - Array Methods

    public static JsonArray GetArray(JsonObject? arguments, string key)
    {
        return GetArray(arguments, key, key);
    }

    public static JsonArray? GetArray(JsonObject? arguments, string key, bool required)
    {
        if (required)
            throw new ArgumentException("Use GetArray(arguments, key) for required parameters");
        return GetArray(arguments, key, key, false);
    }

    public static JsonArray GetArray(JsonObject? arguments, string key, string paramName)
    {
        return GetArray(arguments, key, null, paramName) ?? throw new ArgumentException($"{paramName} is required");
    }

    public static JsonArray? GetArray(JsonObject? arguments, string key, string paramName, bool required)
    {
        return GetArray(arguments, key, null, paramName, required);
    }

    public static JsonArray? GetArray(JsonObject? arguments, string primaryName, string? alternateName,
        string paramName, bool required = true)
    {
        JsonNode? node = null;
        if (arguments != null)
            node = arguments[primaryName] ?? (alternateName != null ? arguments[alternateName] : null);

        if (node == null)
        {
            if (required)
                throw new ArgumentException($"{paramName} is required");
            return null;
        }

        if (node is JsonArray array) return array;

        throw new ArgumentException($"{paramName} must be an array");
    }

    #endregion

    #region Path Validation Methods

    public static string GetAndValidatePath(JsonObject? arguments, string paramName = "path")
    {
        var path = arguments?[paramName]?.GetValue<string>();
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException($"{paramName} is required");

        if (!File.Exists(path))
            throw new ArgumentException($"File not found: {path}");

        return path;
    }

    public static string GetAndValidateOutputPath(JsonObject? arguments, string inputPath,
        string paramName = "outputPath")
    {
        var outputPath = arguments?[paramName]?.GetValue<string>() ?? inputPath;

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return outputPath;
    }

    #endregion

    #region Type Conversion Methods

    public static object ParseValue(string value)
    {
        if (double.TryParse(value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var numValue))
            return numValue;
        if (bool.TryParse(value, out var boolValue))
            return boolValue;
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dateValue))
            return dateValue;
        return value;
    }

    #endregion
}
