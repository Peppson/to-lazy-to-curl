using System.IO;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace to_lazy_to_curl.Services;

public static class LogService
{   
    private static string _logFilePath = string.Empty;

    public static void Init()
    {
        SetupDebugLogger();
        Log.Debug("-- Started --");
    }

    private static void SetupDebugLogger()
    {
        string logDirectory = "Logs";
        _logFilePath = Path.Combine(logDirectory, "Log.txt");
        
        if (!Directory.Exists(logDirectory))
            Directory.CreateDirectory(logDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.File(_logFilePath, 
                rollingInterval: RollingInterval.Infinite, 
                retainedFileCountLimit: 1,
                fileSizeLimitBytes: 1_000_000,
                rollOnFileSizeLimit: true)
            .CreateLogger();
    }

    public static void Shutdown()
    {
        Log.CloseAndFlush();
    }
}
