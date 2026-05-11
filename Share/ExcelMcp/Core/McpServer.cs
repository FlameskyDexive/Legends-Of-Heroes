using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ET.Models;
using ET.Tools;

namespace ET;

/// <summary>
/// MCP (Model Context Protocol) server implementation for Excel operations using EPPlus
/// </summary>
public class McpServer
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, IExcelTool> _tools;

    public McpServer()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        _tools = ToolRegistry.DiscoverTools();

        Console.Error.WriteLine($"[INFO] Registered {_tools.Count} Excel tools using automatic discovery");
    }

    public async Task RunAsync()
    {
        await Console.Error.WriteLineAsync("[INFO] EPPlus MCP Server started");
        await Console.Error.FlushAsync();

        while (true)
        {
            try
            {
                var line = await Console.In.ReadLineAsync();

                if (string.IsNullOrEmpty(line)) break;

                var request = JsonSerializer.Deserialize<JsonObject>(line, _jsonOptions);
                if (request == null)
                {
                    await Console.Error.WriteLineAsync("[WARN] Failed to parse JSON request, skipping");
                    continue;
                }

                await HandleRequestAndSendResponseAsync(request);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"[ERROR] Error processing request: {ex.Message}");

                var errorResponse = new McpResponse
                {
                    Jsonrpc = "2.0",
                    Id = null,
                    Error = McpErrorHandler.ParseError(ex.Message)
                };
                var errorJson = JsonSerializer.Serialize(errorResponse, _jsonOptions);
                await Console.Out.WriteLineAsync(errorJson);
                await Console.Out.FlushAsync();
            }
        }
    }

    private async Task HandleRequestAndSendResponseAsync(JsonObject request)
    {
        try
        {
            var method = request["method"]?.GetValue<string>();
            var id = request["id"];

            // Handle notifications (no response needed)
            if (id == null || method == "initialized" || method?.StartsWith("notifications/") == true)
                return;

            var response = new McpResponse
            {
                Jsonrpc = "2.0",
                Id = id
            };

            if (method == "initialize")
            {
                var paramsObj = request["params"] as JsonObject;
                var clientProtocolVersion = paramsObj?["protocolVersion"]?.GetValue<string>();

                var protocolVersion = clientProtocolVersion == "2025-06-18" ? "2025-06-18" : "2025-11-25";

                response.Result = new
                {
                    protocolVersion,
                    serverInfo = new
                    {
                        name = "epplus-mcp-server",
                        version = VersionHelper.GetVersion()
                    },
                    capabilities = new
                    {
                        tools = new { }
                    }
                };

                var responseJson = JsonSerializer.Serialize(response, _jsonOptions);
                await Console.Out.WriteLineAsync(responseJson);
                await Console.Out.FlushAsync();
                return;
            }

            try
            {
                await ProcessRequestAsync(request, response, method);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"[ERROR] Error handling method '{method}': {ex.Message}");

                if (response.Error == null)
                {
                    response.Result = null;
                    response.Error = McpErrorHandler.HandleException(ex);
                }
            }

            var responseJson2 = JsonSerializer.Serialize(response, _jsonOptions);
            await Console.Out.WriteLineAsync(responseJson2);
            await Console.Out.FlushAsync();
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"[ERROR] Fatal error in HandleRequestAndSendResponseAsync: {ex.Message}");
        }
    }

    private async Task ProcessRequestAsync(JsonObject request, McpResponse response, string? method)
    {
        switch (method)
        {
            case "tools/list":
                response.Result = new
                {
                    tools = _tools.Select(kvp =>
                    {
                        var toolInstance = kvp.Value;
                        var toolName = kvp.Key;

                        var toolObj = new Dictionary<string, object?>
                        {
                            ["name"] = toolName,
                            ["description"] = toolInstance.Description,
                            ["inputSchema"] = toolInstance.InputSchema
                        };

                        var annotations = new Dictionary<string, object?>();

                        if (toolInstance is IAnnotatedTool annotatedTool)
                        {
                            if (annotatedTool.IsReadOnly.HasValue)
                                annotations["readonly"] = annotatedTool.IsReadOnly.Value;
                            if (annotatedTool.IsDestructive.HasValue)
                                annotations["destructive"] = annotatedTool.IsDestructive.Value;
                        }
                        else
                        {
                            var nameLower = toolName.ToLowerInvariant();

                            if (nameLower.StartsWith("get_") ||
                                nameLower.Contains("_get_") ||
                                nameLower.EndsWith("_info") ||
                                nameLower.EndsWith("_statistics"))
                                annotations["readonly"] = true;

                            if (nameLower.StartsWith("delete_") ||
                                nameLower.StartsWith("clear_") ||
                                nameLower.Contains("_delete_") ||
                                nameLower.Contains("_clear"))
                                annotations["destructive"] = true;
                        }

                        if (annotations.Count > 0)
                            toolObj["annotations"] = annotations;

                        return toolObj;
                    }).ToArray()
                };
                break;

            case "tools/call":
                var parameters = request["params"] as JsonObject;
                var toolName = parameters?["name"]?.GetValue<string>();
                var arguments = parameters?["arguments"] as JsonObject;

                if (string.IsNullOrEmpty(toolName))
                {
                    response.Result = null;
                    response.Error = new McpError
                    {
                        Code = -32602,
                        Message = "Tool name is required"
                    };
                    break;
                }

                if (_tools.TryGetValue(toolName, out var tool))
                {
                    try
                    {
                        var result = await tool.ExecuteAsync(arguments);
                        response.Result = new
                        {
                            content = new[]
                            {
                                new
                                {
                                    type = "text",
                                    text = result
                                }
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        response.Result = null;
                        response.Error = McpErrorHandler.HandleException(ex);
                        await Console.Error.WriteLineAsync($"[ERROR] Tool '{toolName}' execution failed: {ex.Message}");
                    }
                }
                else
                {
                    response.Result = null;
                    response.Error = McpErrorHandler.ToolNotFound(toolName);
                }

                break;

            default:
                response.Result = null;
                response.Error = McpErrorHandler.MethodNotFound(method ?? "null");
                break;
        }
    }
}
