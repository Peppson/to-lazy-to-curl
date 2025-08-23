using System.IO;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace to_lazy_to_curl.Services;

public static class LogService
{   
    private static string _logFilePath = string.Empty;

    public static void Init()
    {
        #if !RELEASE
            SetupDebugLogger();
            LogStartupData();
        #endif
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

    public static void LogStartupData()
    {
        Log.Information("---- Starting App ----");

        Log.Information("Width: {Width}px", AppState.MainWindow.Width);
        Log.Information("Height: {Height}px", AppState.MainWindow.Height);
        Log.Information("Top: {Top}px", AppState.MainWindow.Top);
        Log.Information("Left: {Left}px", AppState.MainWindow.Left);

        Log.Information("IsFirstBoot: {IsFirstBoot}", AppState.IsFirstBoot);
        Log.Information("IsDarkTheme: {IsDarkTheme}", Properties.Settings.Default.IsDarkTheme);
        Log.Information("Url: {UrlInputText}", Properties.Settings.Default.UrlInputText);
        Log.Information("Payload: {PayloadText}", Properties.Settings.Default.PayloadText);
        Log.Information("Response: {ResponseText}", Properties.Settings.Default.ResponseText);
        Log.Information("Header: {HeaderText}", Properties.Settings.Default.HeaderText);
    }

    public static void Shutdown()
    {
        Log.CloseAndFlush();
    }
}
