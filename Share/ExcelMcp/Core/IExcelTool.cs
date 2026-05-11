using System.Text.Json.Nodes;

namespace ET.Tools;

/// <summary>
/// Interface for Excel MCP tools
/// </summary>
public interface IExcelTool
{
    /// <summary>
    /// Gets the description of the tool and its usage examples
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the JSON schema defining the input parameters for the tool
    /// </summary>
    object InputSchema { get; }

    /// <summary>
    /// Executes the tool operation with the provided JSON arguments
    /// </summary>
    Task<string> ExecuteAsync(JsonObject? arguments);
}

/// <summary>
/// Optional interface for tools that support MCP annotations
/// </summary>
public interface IAnnotatedTool : IExcelTool
{
    /// <summary>
    /// Indicates if the tool is read-only (does not modify data)
    /// </summary>
    bool? IsReadOnly { get; }

    /// <summary>
    /// Indicates if the tool is destructive (may delete or permanently modify data)
    /// </summary>
    bool? IsDestructive { get; }
}
