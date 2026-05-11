using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ET.Tools;

namespace ET;

/// <summary>
/// Tool registry that automatically discovers and registers Excel tools
/// </summary>
public static class ToolRegistry
{
    public static Dictionary<string, IExcelTool> DiscoverTools()
    {
        var tools = new Dictionary<string, IExcelTool>();
        var assembly = Assembly.GetExecutingAssembly();

        var toolTypes = assembly.GetTypes()
            .Where(t => typeof(IExcelTool).IsAssignableFrom(t)
                        && t is { IsInterface: false, IsAbstract: false, Namespace: { } ns } &&
                        ns.StartsWith("ET"))
            .ToList();

        foreach (var toolType in toolTypes)
        {
            try
            {
                var tool = (IExcelTool)Activator.CreateInstance(toolType)!;
                var toolName = GetToolName(toolType);

                if (!tools.TryAdd(toolName, tool))
                {
                    Console.Error.WriteLine($"[WARN] Duplicate tool name detected: {toolName}. Skipping {toolType.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR] Failed to instantiate tool {toolType.Name}: {ex.Message}");
            }
        }

        return tools;
    }

    /// <summary>
    /// Extracts tool name from type name using naming convention
    /// Example: ExcelFileOperationsTool -> excel_file_operations
    /// </summary>
    private static string GetToolName(Type toolType)
    {
        var name = toolType.Name;

        if (name.EndsWith("Tool"))
            name = name.Substring(0, name.Length - 4);

        var snakeCase = string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + c.ToString().ToLowerInvariant() : c.ToString().ToLowerInvariant()));

        return snakeCase;
    }
}
