using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using Serilog;

namespace to_lazy_to_curl.Services;

public static class ThemeService
{
    public static void Init()
	{
        AppState.IsDarkTheme = Properties.Settings.Default.IsDarkTheme;
        Log.Information($"Loaded Theme: {(AppState.IsDarkTheme ? "Dark" : "Light")}");

		SetColorTheme();
		SetSyntaxColorTheme();
	}

    public static bool GetIsDarkTheme() => AppState.IsDarkTheme;

    public static void ToggleTheme()
    {
        AppState.IsDarkTheme = !AppState.IsDarkTheme;
        SetColorTheme();
        SetSyntaxColorTheme();
        AppState.EditorInput.UpdateSingleViewPositionAndColor();
    }

	private static void SetColorTheme()
    {
        Log.Information($"Color: {(AppState.IsDarkTheme ? "Dark" : "Light")}");

        var dict = new ResourceDictionary
        {
            Source = new Uri(
                AppState.IsDarkTheme ? "Settings/Colors.Dark.xaml" : "Settings/Colors.Light.xaml",
                UriKind.Relative)
        };

        Application.Current.Resources.MergedDictionaries[0].Clear();
        Application.Current.Resources.MergedDictionaries.Add(dict);
    }

    public static void SetSyntaxColorTheme()
    {
        Log.Information($"Syntax: {(AppState.IsDarkTheme ? "Dark" : "Light")}");

        // Set syntax colors in both editors
        var editors = new[] {
            AppState.EditorInput.PayloadEditor1,
            AppState.EditorInput.PayloadEditor2,
            AppState.EditorInput.ResponseEditor1,
            AppState.EditorInput.ResponseEditor2,
        };

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
