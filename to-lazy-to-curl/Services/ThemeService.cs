using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using Serilog;

namespace to_lazy_to_curl.Services;

public static class ThemeService
{
    private static bool _isDarkTheme = false;

    public static void Init()
	{
		_isDarkTheme = Properties.Settings.Default.IsDarkTheme;
        Log.Debug($"Loaded theme: {(_isDarkTheme ? "Dark" : "Light")}");

		SetColorTheme();
		SetSyntaxColorTheme();
	}

    public static bool GetIsDarkTheme() => _isDarkTheme;

    public static void ToggleTheme()
    {
        _isDarkTheme = !_isDarkTheme;
        SetColorTheme();
        SetSyntaxColorTheme();
    }

	private static void SetColorTheme()
    {
        Log.Debug($"ColorTheme: {(_isDarkTheme ? "Dark" : "Light")}");
        AppState.IsDarkTheme = _isDarkTheme;

        var dict = new ResourceDictionary
        {
            Source = new Uri(
                _isDarkTheme ? "Settings/Colors.Dark.xaml" : "Settings/Colors.Light.xaml",
                UriKind.Relative)
        };

        Application.Current.Resources.MergedDictionaries[0].Clear();
        Application.Current.Resources.MergedDictionaries.Add(dict);
    }

    public static void SetSyntaxColorTheme()
    {
        Log.Debug($"SyntaxTheme: {(_isDarkTheme ? "Dark" : "Light")}");

        // Set syntax colors in both editors
        var editors = new[] { AppState.JsonInput.JsonTextBox, AppState.JsonInput.ResponseEditor };

        foreach (var editor in editors)
        {
            var syntaxColor = editor.SyntaxHighlighting;
            foreach (var color in syntaxColor.NamedHighlightingColors)
            {
                if (Application.Current.TryFindResource(color.Name) is SolidColorBrush brush)
                    color.Foreground = new SimpleHighlightingBrush(brush.Color);
            }       
            editor.TextArea.TextView.Redraw();
        }
    }
}
