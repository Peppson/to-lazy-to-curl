using System.Windows;
using Serilog;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
    private static bool _isDarkTheme;

    public MainWindow()
    {
        InitializeComponent();
        InitWindowSizeAndPosition();
        InitColorTheme();
        LogService.Init();
    }

    private void ToggleTheme_Click(object sender, RoutedEventArgs e)
    {   
        _isDarkTheme = !_isDarkTheme;
        SetColorTheme(_isDarkTheme);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);
        LogService.Shutdown();
        
        Properties.Settings.Default.IsDarkTheme = _isDarkTheme;
        Properties.Settings.Default.WindowWidth = Width;
        Properties.Settings.Default.WindowHeight = Height;
        Properties.Settings.Default.WindowTop = Top;
        Properties.Settings.Default.WindowLeft = Left;
        Properties.Settings.Default.Save();
    }
    
    private void InitWindowSizeAndPosition()
    {
        if (Properties.Settings.Default.WindowWidth <= 0) return;

        Width = Properties.Settings.Default.WindowWidth;
        Height = Properties.Settings.Default.WindowHeight;
        Top = Properties.Settings.Default.WindowTop;
        Left = Properties.Settings.Default.WindowLeft;
    }

    private static void InitColorTheme()
    {
        _isDarkTheme = Properties.Settings.Default.IsDarkTheme;
        SetColorTheme(_isDarkTheme);
    }

    private static void SetColorTheme(bool darkTheme)
    {
        Log.Debug($"Theme: {(darkTheme ? "Dark" : "Light")}");

        var dict = new ResourceDictionary
        {
            Source = new Uri(
                darkTheme ? "Settings/Colors.Dark.xaml" : "Settings/Colors.Light.xaml",
                UriKind.Relative)
        };

        Application.Current.Resources.MergedDictionaries[0] = dict;
    }
}
