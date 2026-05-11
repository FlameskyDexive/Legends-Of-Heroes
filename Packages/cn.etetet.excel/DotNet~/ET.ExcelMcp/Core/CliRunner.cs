using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using ET.Tools;

namespace ET;

/// <summary>
/// CLI runner for executing Excel tools from command line
/// </summary>
public static class CliRunner
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Run a tool from command line arguments
    /// Usage: dotnet ET.ExcelMcp.dll cli &lt;tool_name&gt; '&lt;json_args&gt;'
    /// Example: dotnet ET.ExcelMcp.dll cli excel_cell '{"operation":"write","path":"test.xlsx","cell":"A1","value":"Hello"}'
    /// </summary>
    public static async Task<int> RunAsync(string[] args)
    {
        if (args.Length < 2)
        {
            PrintUsage();
            return 1;
        }

        var toolName = args[1];

        // Special command: list tools
        if (toolName == "list" || toolName == "--list" || toolName == "-l")
        {
            ListTools();
            return 0;
        }

        // Special command: help for a specific tool
        if (toolName == "help" && args.Length >= 3)
        {
            ShowToolHelp(args[2]);
            return 0;
        }

        // Get JSON arguments (optional, defaults to empty object)
        var jsonArgs = args.Length >= 3 ? args[2] : "{}";

        try
        {
            var tools = ToolRegistry.DiscoverTools();

            if (!tools.TryGetValue(toolName, out var tool))
            {
                Console.Error.WriteLine($"[ERROR] Tool not found: {toolName}");
                Console.Error.WriteLine();
                Console.Error.WriteLine("Available tools:");
                foreach (var name in tools.Keys.OrderBy(k => k))
                {
                    Console.Error.WriteLine($"  - {name}");
                }
                return 1;
            }

            var arguments = JsonSerializer.Deserialize<JsonObject>(jsonArgs, JsonOptions);
            var result = await tool.ExecuteAsync(arguments);

            Console.WriteLine(result);
            return 0;
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"[ERROR] Invalid JSON arguments: {ex.Message}");
            Console.Error.WriteLine($"[ERROR] Input: {jsonArgs}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ERROR] {ex.Message}");
#if DEBUG
            Console.Error.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
#endif
            return 1;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine(@"ET.ExcelMcp - Excel CLI Tool

Usage:
  dotnet ET.ExcelMcp.dll cli <tool_name> '<json_args>'
  dotnet ET.ExcelMcp.dll cli list
  dotnet ET.ExcelMcp.dll cli help <tool_name>

Examples:
  # Create a new Excel file
  dotnet ET.ExcelMcp.dll cli excel_file_operations '{""operation"":""create"",""path"":""test.xlsx""}'

  # Write to a cell
  dotnet ET.ExcelMcp.dll cli excel_cell '{""operation"":""write"",""path"":""test.xlsx"",""cell"":""A1"",""value"":""Hello""}'

  # Read from a cell
  dotnet ET.ExcelMcp.dll cli excel_cell '{""operation"":""get"",""path"":""test.xlsx"",""cell"":""A1""}'

  # List all tools
  dotnet ET.ExcelMcp.dll cli list

  # Get help for a specific tool
  dotnet ET.ExcelMcp.dll cli help excel_cell

Notes:
  - JSON arguments should be properly escaped for your shell
  - Use single quotes around JSON on Unix/macOS
  - Use escaped double quotes on Windows PowerShell
");
    }

    private static void ListTools()
    {
        var tools = ToolRegistry.DiscoverTools();

        Console.WriteLine("Available Excel Tools:");
        Console.WriteLine("======================");
        Console.WriteLine();

        foreach (var (name, tool) in tools.OrderBy(kvp => kvp.Key))
        {
            // Get first line of description
            var desc = tool.Description.Split('\n')[0].Trim();
            Console.WriteLine($"  {name}");
            Console.WriteLine($"    {desc}");
            Console.WriteLine();
        }

        Console.WriteLine($"Total: {tools.Count} tools");
    }

    private static void ShowToolHelp(string toolName)
    {
        var tools = ToolRegistry.DiscoverTools();

        if (!tools.TryGetValue(toolName, out var tool))
        {
            Console.Error.WriteLine($"[ERROR] Tool not found: {toolName}");
            return;
        }

        Console.WriteLine($"Tool: {toolName}");
        Console.WriteLine(new string('=', toolName.Length + 6));
        Console.WriteLine();
        Console.WriteLine("Description:");
        Console.WriteLine(tool.Description);
        Console.WriteLine();
        Console.WriteLine("Input Schema:");
        Console.WriteLine(JsonSerializer.Serialize(tool.InputSchema, JsonOptions));
    }
}
