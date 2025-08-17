using System.Windows;
using System.Windows.Controls;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Components;

public partial class JsonInput : UserControl
{
    private bool _isResponseEditor = false;
    private bool _wasNarrow;
    private GridLength _lastLeftWidth = new GridLength(1, GridUnitType.Star);
    private GridLength _lastRightWidth = new GridLength(1, GridUnitType.Star);

    public string JsonRequestBody
    {
        get => JsonTextBox.Text;
        set => JsonTextBox.Text = value;
    }

    public string JsonResponseBody
    {
        get => ResponseEditor.Text;
        set => ResponseEditor.Text = value;
    }

    public JsonInput()
    {
        InitializeComponent();
        SetupEditors();

        // Track window width for Json editor split or single view
        Loaded += (_, __) =>
        {
            Window window = Window.GetWindow(this)!;
            window.SizeChanged += Window_SizeChanged;
            UpdateEditorLayouts(window.ActualWidth);
        };

        HttpService.JsonResponseBody = ResponseEditor;
        UiService.JsonEditorBorder = JsonEditorsBorder;
    }

    private void Window_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        double width = e.NewSize.Width;
        bool isNarrow = width < Config.SplitEditorThreshold;

        if (isNarrow != _wasNarrow)
        {
            _wasNarrow = isNarrow;
            UpdateEditorLayouts(width);
        }
    }

    private void RequestButton_Click(object sender, RoutedEventArgs e)
    {
        _isResponseEditor = false;
        UpdateEditorPositions();
    }

    private void ResponseButton_Click(object sender, RoutedEventArgs e)
    {
        _isResponseEditor = true;
        UpdateEditorPositions();
    }

    private void SetupEditors()
    {
        JsonTextBox.Text = Config.JsonSampleData;
        JsonTextBox.Options.EnableHyperlinks = false;
        JsonTextBox.Options.EnableEmailHyperlinks = false;

        ResponseEditor.Text = Config.UrlStartupData; // todo
        ResponseEditor.Options.EnableHyperlinks = false;
        ResponseEditor.Options.EnableEmailHyperlinks = false;
    }

    private void UpdateEditorPositions()
    {
        UpdateEditorVisibility();

        if (_isResponseEditor)
        {
            Grid.SetColumn(JsonTextBox, 2);
            Grid.SetColumn(ResponseEditor, 0);
        }
        else
        {
            Grid.SetColumn(JsonTextBox, 0);
            Grid.SetColumn(ResponseEditor, 2);
        }
    }

    private void UpdateEditorVisibility(bool showAll = false)
    {
        if (showAll)
        {
            JsonTextBox.Visibility = Visibility.Visible;
            ResponseEditor.Visibility = Visibility.Visible;
            return;
        }
        ResponseEditor.Visibility = _isResponseEditor ? Visibility.Visible : Visibility.Collapsed;
        JsonTextBox.Visibility = _isResponseEditor ? Visibility.Collapsed : Visibility.Visible;
    }

    private void UpdateEditorLayouts(double width)
    {
        bool isNarrow = width < Config.SplitEditorThreshold;

        // Single view
        if (isNarrow)
        {
            ModeButtonPanel.Visibility = Visibility.Visible;

            UpdateEditorPositions();
            UpdateEditorVisibility();

            // Save widths before swaping layout
            _lastLeftWidth = MainGrid.ColumnDefinitions[0].Width;
            _lastRightWidth = MainGrid.ColumnDefinitions[2].Width;

            MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            MainGrid.ColumnDefinitions[1].Width = new GridLength(0);
            MainGrid.ColumnDefinitions[2].Width = new GridLength(0);
        }
        else // Split view
        {
            ModeButtonPanel.Visibility = Visibility.Collapsed;

            Grid.SetColumn(JsonTextBox, 0);
            Grid.SetColumn(ResponseEditor, 2);
            UpdateEditorVisibility(true);

            // Restore last split widths
            MainGrid.ColumnDefinitions[0].Width = _lastLeftWidth;
            MainGrid.ColumnDefinitions[1].Width = new GridLength(10);
            MainGrid.ColumnDefinitions[2].Width = _lastRightWidth;

        }
    }

    public void Reset()
    {
        _isResponseEditor = false;
        JsonRequestBody = string.Empty;
        JsonResponseBody = string.Empty;

        // Layout and grid
        _lastLeftWidth = new GridLength(1, GridUnitType.Star);
        _lastRightWidth = new GridLength(1, GridUnitType.Star);
        MainGrid.ColumnDefinitions[0].Width = _lastLeftWidth;
        MainGrid.ColumnDefinitions[2].Width = _lastRightWidth;
        
        Window parentWindow = Window.GetWindow(this);
        UpdateEditorLayouts(parentWindow.ActualWidth);
    }
}
