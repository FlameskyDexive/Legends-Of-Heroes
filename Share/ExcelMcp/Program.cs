using System.Text;
using ET;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

try
{
    // Set console encoding to UTF-8 for proper Chinese character support
    Console.OutputEncoding = Encoding.UTF8;
    Console.InputEncoding = Encoding.UTF8;

    // Check if running in CLI mode
    if (args.Length > 0 && args[0].ToLower() == "cli")
    {
        // CLI mode: direct command line execution
        var exitCode = await CliRunner.RunAsync(args);
        Environment.Exit(exitCode);
    }
    else
    {
        // MCP mode: JSON-RPC server via stdin/stdout
        Console.Error.WriteLine("[INFO] EPPlus MCP Server - Excel专用服务器");
        await Console.Error.FlushAsync();

        var server = new McpServer();
        await server.RunAsync();
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[ERROR] Fatal error: {ex.GetType().Name}");
#if DEBUG
    Console.Error.WriteLine($"[ERROR] Details: {ex.Message}");
    Console.Error.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
#else
    Console.Error.WriteLine($"[ERROR] An internal error occurred. Check logs for details.");
#endif
    Environment.Exit(1);
}
