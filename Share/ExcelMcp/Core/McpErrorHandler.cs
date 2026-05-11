using System.Text.RegularExpressions;
using ET.Models;

namespace ET;

/// <summary>
/// Centralized error handler for MCP server errors
/// </summary>
public static class McpErrorHandler
{
    public static McpError HandleException(Exception ex, bool includeStackTrace = false)
    {
        return ex switch
        {
            ArgumentNullException => new McpError
            {
                Code = -32602,
                Message = "Required parameter is missing or null"
            },
            ArgumentException argEx => new McpError
            {
                Code = -32602,
                Message = SanitizeErrorMessage(argEx.Message, true)
            },
            FileNotFoundException => new McpError
            {
                Code = -32602,
                Message = "File not found"
            },
            DirectoryNotFoundException => new McpError
            {
                Code = -32602,
                Message = "Directory not found"
            },
            UnauthorizedAccessException => new McpError
            {
                Code = -32603,
                Message = "Access denied to file or directory"
            },
            IOException => new McpError
            {
                Code = -32603,
                Message = "I/O error occurred while processing the request"
            },
            InvalidOperationException opEx => new McpError
            {
                Code = -32603,
                Message = SanitizeErrorMessage(opEx.Message)
            },
            NotSupportedException => new McpError
            {
                Code = -32603,
                Message = "Operation not supported"
            },
            _ => new McpError
            {
                Code = -32603,
                Message = includeStackTrace ? SanitizeErrorMessage(ex.Message) : "Internal server error occurred"
            }
        };
    }

    public static McpError MethodNotFound(string method)
    {
        return new McpError
        {
            Code = -32601,
            Message = $"Unknown method: {method}"
        };
    }

    public static McpError ToolNotFound(string toolName)
    {
        return new McpError
        {
            Code = -32601,
            Message = $"Unknown tool: {toolName}"
        };
    }

    public static McpError ParseError(string message)
    {
        return new McpError
        {
            Code = -32700,
            Message = $"Parse error: {message}"
        };
    }

    public static McpError InvalidParams(string message)
    {
        return new McpError
        {
            Code = -32602,
            Message = SanitizeErrorMessage(message, true)
        };
    }

    private static string SanitizeErrorMessage(string message, bool preserveDetails = false)
    {
        if (string.IsNullOrWhiteSpace(message)) return "An error occurred";

        var sanitized = message;

        if (!preserveDetails)
        {
            sanitized = Regex.Replace(sanitized, @"[A-Za-z]:\\[^\s]+", "[path removed]");
            sanitized = Regex.Replace(sanitized, @"/[^\s]+", "[path removed]");
            sanitized = Regex.Replace(sanitized, @"at\s+[^\r\n]+", "");
            sanitized = Regex.Replace(sanitized, @"in\s+[^\r\n]+", "");
            sanitized = Regex.Replace(sanitized, @"line\s+\d+", "");
            sanitized = Regex.Replace(sanitized, @"\w+\.\w+Exception", "Error");
        }
        else
        {
            sanitized = Regex.Replace(sanitized, @"[A-Za-z]:\\[^\s]+", "[path removed]");
            sanitized = Regex.Replace(sanitized, @"\/[^\s]+", "[path removed]");
        }

        var maxLength = preserveDetails ? 2000 : 500;
        if (sanitized.Length > maxLength) sanitized = sanitized.Substring(0, maxLength - 3) + "...";

        return sanitized.Trim();
    }
}
