using to_lazy_to_curl.Components;
using to_lazy_to_curl.Models;

namespace to_lazy_to_curl.Services;

static public class AppState
{
    public static event EventHandler? SelectedHttpButtonEvent;
    public static event EventHandler? PayloadEditorSyntaxEvent;
    public static event EventHandler? ResponseEditorSyntaxEvent;

    private static HttpAction _selectedAction = HttpAction.NONE;
    private static string _payloadEditorSyntax = SyntaxHighlighting.Json;
    private static string _responseEditorSyntax = SyntaxHighlighting.Json;

    public static MainWindow MainWindow { get; set; } = null!;
    public static UrlInput UrlInput { get; set; } = null!;
    public static EditorInput EditorInput { get; set; } = null!;
    public static HttpMessageBox MessageBox { get; set; } = null!;
    public static PopupMenu PopupMenu { get; set; } = null!;
    public static bool IsFirstBoot { get; set; } = false;
    public static bool IsDarkTheme { get; set; } = true;
    public static bool IsNarrow { get; set; } = false;

    public static HttpAction SelectedHttpAction
    {
        get => _selectedAction;
        set
        {
            if (_selectedAction == value) return;
            _selectedAction = value;
            SelectedHttpButtonEvent?.Invoke(null, EventArgs.Empty);
        }
    }

    public static string PayloadEditorSyntax
    {
        get => _payloadEditorSyntax;
        set
        {
            if (_payloadEditorSyntax == value) return;
            _payloadEditorSyntax = value;
            PayloadEditorSyntaxEvent?.Invoke(null, EventArgs.Empty);
        }
    }

    public static string ResponseEditorSyntax
    {
        get => _responseEditorSyntax;
        set
        {
            if (_responseEditorSyntax == value) return;
            _responseEditorSyntax = value;
            ResponseEditorSyntaxEvent?.Invoke(null, EventArgs.Empty);
        }
    }
}
