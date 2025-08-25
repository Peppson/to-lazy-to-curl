using System.IO;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace to_lazy_to_curl.Services;

public static class LogService
{   
    private static readonly string _appName = System.Windows.Application.Current.MainWindow?.Title!;
    private const string _logFileName = "Log.txt";
    private static bool _logDirExisted = true;

    public static string FilePath { get; private set; } = string.Empty;

    public static void Init()
    {
        #if !RELEASE
            InitDebug();
        #else
            InitRelease();
        #endif
    }
    
    private static void EnsureDirectoryExists(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            _logDirExisted = false;
        }
    }

    private static void RemoveOldLogFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public static void InitRelease()
    {
        var baseDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            _appName
        );

        // Dir
        var dir = Path.Combine(baseDir, "Logs");
        EnsureDirectoryExists(dir);

        // File
        FilePath = Path.Combine(dir, _logFileName);
        RemoveOldLogFile(FilePath);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.File(FilePath,
                rollingInterval: RollingInterval.Infinite,
                retainedFileCountLimit: 1,
                fileSizeLimitBytes: 1_000_000,
                rollOnFileSizeLimit: true)
            .CreateLogger();

        if (!_logDirExisted)
        {
            Log.Warning($"Log directory did not exist. Created new directory at: \n{dir}");
        }

        LogStartupData(dir);
    }

    private static void InitDebug()
    {
        // Get project root directory
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var projectDir = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName; // ../../../../../../../../
        if (projectDir == null)
            throw new InvalidOperationException("Project root directory could not be found");

        // Dir
        var dir = Path.Combine(projectDir, "Logs");
        EnsureDirectoryExists(dir);

        // File
        FilePath = Path.Combine(dir, _logFileName);
        RemoveOldLogFile(FilePath);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.File(FilePath,
                rollingInterval: RollingInterval.Infinite,
                retainedFileCountLimit: 1,
                fileSizeLimitBytes: 1_000_000,
                rollOnFileSizeLimit: true)
            .CreateLogger();

        LogStartupData(dir);
    }

    private static void LogStartupData(string createdLogDirectory)
    {
        Log.Warning("---- Starting App ----"); // Makes sure logfile is created on "Release" too

        Log.Information("Width: {Width}px", AppState.MainWindow.Width);
        Log.Information("Height: {Height}px", AppState.MainWindow.Height);
        Log.Information("Top: {Top}px", AppState.MainWindow.Top);
        Log.Information("Left: {Left}px", AppState.MainWindow.Left);

        Log.Information("IsFirstBoot: {IsFirstBoot}", AppState.IsFirstBoot);
        Log.Information("IsDarkTheme: {IsDarkTheme}", Properties.Settings.Default.IsDarkTheme);
        Log.Information("Url: {UrlInputText}", Properties.Settings.Default.UrlInputText);
        Log.Information("Payload: \n{PayloadText}", Properties.Settings.Default.PayloadText);
        Log.Information("Response: \n{ResponseText}", Properties.Settings.Default.ResponseText);
        Log.Information("Header: \n{HeaderText}", Properties.Settings.Default.HeaderText);

        if (!_logDirExisted)
        {
            Log.Warning("Log directory did not exist. Created new directory at: \n{createdLogDirectory}", createdLogDirectory);
        }
    }

    public static void Shutdown()
    {
        Log.CloseAndFlush();
    }
}
