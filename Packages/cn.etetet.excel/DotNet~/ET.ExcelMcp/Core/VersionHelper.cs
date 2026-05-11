using System.Reflection;

namespace ET;

public static class VersionHelper
{
    private static string? _version;

    public static string GetVersion()
    {
        if (_version != null) return _version;

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            if (version != null)
            {
                _version = $"{version.Major}.{version.Minor}.{version.Build}";
                return _version;
            }
        }
        catch
        {
            // Fallback to default version
        }

        _version = "1.0.0";
        return _version;
    }
}
