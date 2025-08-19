using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.Settings;

namespace to_lazy_to_curl.Components;

public partial class JsonInput : UserControl
{



    private bool _wasNarrow = false;
    private GridLength _lastLeftWidth = new(1, GridUnitType.Star);
    private GridLength _lastRightWidth = new(1, GridUnitType.Star);




    public static readonly DependencyProperty IsResponseEditorProperty =
        DependencyProperty.Register(
            nameof(IsResponseEditor),
            typeof(bool),
            typeof(JsonInput),
            new PropertyMetadata(false));

    public static readonly DependencyProperty PayloadEditorSyntaxProperty =
        DependencyProperty.Register(
            nameof(PayloadEditorSyntax),
            typeof(string),
            typeof(JsonInput),
            new PropertyMetadata(SyntaxHighlighting.Json));

    public static readonly DependencyProperty ResponseEditorSyntaxProperty =
        DependencyProperty.Register(
            nameof(ResponseEditorSyntax),
            typeof(string),
            typeof(JsonInput),
            new PropertyMetadata(SyntaxHighlighting.Json));

    public string PayloadEditorSyntax
    {
        get => (string)GetValue(PayloadEditorSyntaxProperty);
        set
        {
            SetValue(PayloadEditorSyntaxProperty, value);
            var definition =
                ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition(value);
            JsonTextBox.SyntaxHighlighting = definition;
        }
    }

    public string ResponseEditorSyntax
    {
        get => (string)GetValue(ResponseEditorSyntaxProperty);
        set
        {
            SetValue(ResponseEditorSyntaxProperty, value);
            var definition =
                ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition(value);
            ResponseEditor.SyntaxHighlighting = definition;
        }
    }

    public bool IsResponseEditor
    {
        get => (bool)GetValue(IsResponseEditorProperty);
        private set => SetValue(IsResponseEditorProperty, value);
    }




    public string JsonRequestBody
    {
        get => JsonTextBox.Text;
        set
        {
            if (JsonTextBox.Document.Text != value)
            {
                JsonTextBox.Document.UndoStack.StartUndoGroup();
                JsonTextBox.Document.Text = value ?? string.Empty;
                JsonTextBox.Document.UndoStack.EndUndoGroup();
            }
        }
    }

    public string JsonResponseBody
    {
        get => ResponseEditor.Text;
        set
        {
            if (ResponseEditor.Document.Text != value)
            {
                ResponseEditor.Document.UndoStack.StartUndoGroup();
                ResponseEditor.Document.Text = value ?? string.Empty;
                ResponseEditor.Document.UndoStack.EndUndoGroup();
            }
        }
    }



    public JsonInput()
    {
        InitializeComponent();
        AppState.JsonInput = this;
        SetupEditors();

        // Track window width for split or single view
        Loaded += (_, __) =>
        {
            Window window = Window.GetWindow(this)!;
            window.SizeChanged += Window_SizeChanged;
            UpdateEditorLayouts(window.ActualWidth);
            _wasNarrow = window.ActualWidth < Config.SplitEditorThreshold;
        };

        Splitter.LayoutUpdated += (s, e) => SetResponseButtonPositionSplitView();

        EventHandler payloadHandler = (_, __) => PayloadEditorSyntax = AppState.PayloadEditorSyntax;
        EventHandler responseHandler = (_, __) => ResponseEditorSyntax = AppState.ResponseEditorSyntax;
        AppState.PayloadEditorSyntaxEvent += payloadHandler;
        AppState.ResponseEditorSyntaxEvent += responseHandler;

        Unloaded += (_, __) =>
        {
            AppState.PayloadEditorSyntaxEvent -= payloadHandler;
            AppState.ResponseEditorSyntaxEvent -= responseHandler;
        };
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
        IsResponseEditor = false;
        UpdateEditorPositions();
        JsonTextBox.Focus();
    }

    private void ResponseButton_Click(object sender, RoutedEventArgs e)
    {
        IsResponseEditor = true;
        UpdateEditorPositions();
        ResponseEditor.Focus();
    }

    private void UpdateEditorPositions()
    {
        UpdateEditorVisibility();

        if (IsResponseEditor)
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
        ResponseEditor.Visibility = IsResponseEditor ? Visibility.Visible : Visibility.Collapsed;
        JsonTextBox.Visibility = IsResponseEditor ? Visibility.Collapsed : Visibility.Visible;
    }

    private void UpdateEditorLayouts(double width)
    {
        bool isNarrow = width < Config.SplitEditorThreshold;

        // Single view
        if (isNarrow)
        {
            SingleViewButtonPanel.Visibility = Visibility.Visible;
            SplitViewButtonPanel.Visibility = Visibility.Collapsed;

            UpdateEditorPositions();
            UpdateEditorVisibility();

            // Save widths before swaping layout
            _lastLeftWidth = JsonEditorGrid.ColumnDefinitions[0].Width;
            _lastRightWidth = JsonEditorGrid.ColumnDefinitions[2].Width;

            JsonEditorGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            JsonEditorGrid.ColumnDefinitions[1].Width = new GridLength(0);
            JsonEditorGrid.ColumnDefinitions[2].Width = new GridLength(0);
            JsonEditorGrid.ColumnDefinitions[0].MinWidth = 0;
            JsonEditorGrid.ColumnDefinitions[2].MinWidth = 0;
        }
        else // Split view
        {
            SingleViewButtonPanel.Visibility = Visibility.Collapsed;
            SplitViewButtonPanel.Visibility = Visibility.Visible;

            Grid.SetColumn(JsonTextBox, 0);
            Grid.SetColumn(ResponseEditor, 2);
            UpdateEditorVisibility(true);

            // Restore last split widths
            JsonEditorGrid.ColumnDefinitions[0].Width = _lastLeftWidth;
            JsonEditorGrid.ColumnDefinitions[1].Width = new GridLength(10);
            JsonEditorGrid.ColumnDefinitions[2].Width = _lastRightWidth;
            JsonEditorGrid.ColumnDefinitions[0].MinWidth = 157;
            JsonEditorGrid.ColumnDefinitions[2].MinWidth = 170;

            SetResponseButtonPositionSplitView();
        }
    }

    private void SetResponseButtonPositionSplitView()
    {
        double leftWidth = JsonEditorGrid.ColumnDefinitions[0].ActualWidth;
        ResponseButton2.Margin = new Thickness(leftWidth + 6f, 0, 0, -1f);
    }

    private void SetupEditors()
    {
        IsResponseEditor = false;

        PayloadEditorSyntax = AppState.ResponseEditorSyntax;
        JsonTextBox.Options.EnableEmailHyperlinks = false;
        JsonTextBox.Options.EnableHyperlinks = false;
        JsonTextBox.Text = Config.JsonSampleData;

        ResponseEditorSyntax = AppState.ResponseEditorSyntax;
        ResponseEditor.Options.EnableEmailHyperlinks = false;
        ResponseEditor.Options.EnableHyperlinks = false;
        ResponseEditor.Text = Config.JsonSampleResponse;

        AppState.JsonInput.JsonTextBox.Text = JsonTextBox.Text;
    }

    public void Reset()
    {
        IsResponseEditor = false;

        // Payload
        AppState.PayloadEditorSyntax = SyntaxHighlighting.Json;
        PayloadEditorSyntax = SyntaxHighlighting.Json;
        JsonRequestBody = string.Empty;

        // Response
        AppState.ResponseEditorSyntax = SyntaxHighlighting.Json;
        ResponseEditorSyntax = SyntaxHighlighting.Json;
        JsonResponseBody = string.Empty;

        // Layout and grid
        _lastLeftWidth = new GridLength(1, GridUnitType.Star);
        _lastRightWidth = new GridLength(1, GridUnitType.Star);
        JsonEditorGrid.ColumnDefinitions[0].Width = _lastLeftWidth;
        JsonEditorGrid.ColumnDefinitions[2].Width = _lastRightWidth;

        Window parentWindow = Window.GetWindow(this);
        UpdateEditorLayouts(parentWindow.ActualWidth);
        SetResponseButtonPositionSplitView();
    }
    









    // todo

    private bool _isDragging = false;
    private Point _startPoint;

    private void ResponseButton2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _startPoint = e.GetPosition(JsonEditorGrid); // relative to the editor grid
        ResponseButton2.CaptureMouse();
    }

    private void ResponseButton2_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging) return;

        Point current = e.GetPosition(JsonEditorGrid);
        double deltaX = current.X - _startPoint.X;

        // Move the splitter
        double newLeftWidth = JsonEditorGrid.ColumnDefinitions[0].ActualWidth + deltaX;

        // respect min width
        newLeftWidth = Math.Max(newLeftWidth, 170);
        newLeftWidth = Math.Min(newLeftWidth, JsonEditorGrid.ActualWidth - 170 - Splitter.Width);

        JsonEditorGrid.ColumnDefinitions[0].Width = new GridLength(newLeftWidth, GridUnitType.Pixel);
        JsonEditorGrid.ColumnDefinitions[2].Width = new GridLength(JsonEditorGrid.ActualWidth - newLeftWidth - Splitter.Width, GridUnitType.Pixel);

        _startPoint = current; // reset start for smooth dragging
    }

    private void ResponseButton2_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            ResponseButton2.ReleaseMouseCapture();
        }
    }



}
