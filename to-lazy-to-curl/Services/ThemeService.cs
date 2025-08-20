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

        var dict = new ResourceDictionary
        {
            Source = new Uri(
                _isDarkTheme ? "Settings/Colors.Dark.xaml" : "Settings/Colors.Light.xaml",
                UriKind.Relative)
        };

        Application.Current.Resources.MergedDictionaries[0].Clear();
        Application.Current.Resources.MergedDictionaries.Add(dict);
    }
	
	private static void SetSyntaxColorTheme()
    {	
		Log.Debug($"SyntaxTheme: {(_isDarkTheme ? "Dark" : "Light")}");

		// Set syntax colors
		var syntaxColor = AppState.JsonInput.JsonTextBox.SyntaxHighlighting;
		foreach (var color in syntaxColor.NamedHighlightingColors)
		{
			if (Application.Current.FindResource(color.Name) is SolidColorBrush brush)
				color.Foreground = new SimpleHighlightingBrush(brush.Color);
		}

		AppState.JsonInput.JsonTextBox.TextArea.TextView.Redraw();
		AppState.JsonInput.ResponseEditor.TextArea.TextView.Redraw();
    }
}
