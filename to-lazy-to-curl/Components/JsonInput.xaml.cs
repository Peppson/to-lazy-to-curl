using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.Settings;
using Serilog;
using System.Windows.Media;

namespace to_lazy_to_curl.Components;

public partial class JsonInput : UserControl
{
    private bool _wasNarrow = false;
    private TabType _lastTab = TabType.Payload;

    public string PayloadEditorSyntax
    {
        get => (string)GetValue(PayloadEditorSyntaxProperty);
        set
        {
            SetValue(PayloadEditorSyntaxProperty, value);
            var definition =
                ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition(value);
            this.PayloadEditor1.SyntaxHighlighting = definition;
            this.PayloadEditor2.SyntaxHighlighting = definition;
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
            this.ResponseEditor1.SyntaxHighlighting = definition;
            this.ResponseEditor2.SyntaxHighlighting = definition;
        }
    }

    public string PayloadEditorText
    {
        get => PayloadEditor1.Text;
        set
        {
            if (PayloadEditor1.Document.Text != value)
            {
                PayloadEditor1.Document.UndoStack.StartUndoGroup();
                PayloadEditor1.Document.Text = value ?? string.Empty;
                PayloadEditor1.Document.UndoStack.EndUndoGroup();
            }

            if (PayloadEditor2.Document.Text != value)
            {
                PayloadEditor2.Document.UndoStack.StartUndoGroup();
                PayloadEditor2.Document.Text = value ?? string.Empty;
                PayloadEditor2.Document.UndoStack.EndUndoGroup();
            }
        }
    }

    public string ResponseEditorText
    {
        get => ResponseEditor1.Text;
        set
        {
            if (ResponseEditor1.Document.Text != value)
            {
                ResponseEditor1.Document.UndoStack.StartUndoGroup();
                ResponseEditor1.Document.Text = value ?? string.Empty;
                ResponseEditor1.Document.UndoStack.EndUndoGroup();
            }

            if (ResponseEditor2.Document.Text != value)
            {
                ResponseEditor2.Document.UndoStack.StartUndoGroup();
                ResponseEditor2.Document.Text = value ?? string.Empty;
                ResponseEditor2.Document.UndoStack.EndUndoGroup();
            }
        }
    }

    public string HeaderEditorText
    {
        get => HeaderEditor1.Text;
        set
        {
            if (HeaderEditor1.Document.Text != value)
            {
                HeaderEditor1.Document.UndoStack.StartUndoGroup();
                HeaderEditor1.Document.Text = value ?? string.Empty;
                HeaderEditor1.Document.UndoStack.EndUndoGroup();
            }

            if (HeaderEditor2.Document.Text != value)
            {
                HeaderEditor2.Document.UndoStack.StartUndoGroup();
                HeaderEditor2.Document.Text = value ?? string.Empty;
                HeaderEditor2.Document.UndoStack.EndUndoGroup();
            }
        }
    }

    public JsonInput()
    {
        InitializeComponent();
        AppState.JsonInput = this;
        SetupEditors();
        SetStartupText();

        // Track window width for split or single view
        Loaded += (_, __) =>
        {
            Window window = Window.GetWindow(this)!;
            window.SizeChanged += Window_SizeChanged;
            UpdateEditorLayouts(window.ActualWidth);
            _wasNarrow = window.ActualWidth < Config.SplitEditorThreshold;
        };

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

    private void PayloadButton_Click(object sender, RoutedEventArgs e)
    {
        _lastTab = TabType.Payload;
        UpdateEditorPosition();
        this.PayloadEditor1.Focus();
    }

    private void ResponseButton_Click(object sender, RoutedEventArgs e)
    {   
        _lastTab = TabType.Response;
        UpdateEditorPosition();
        this.ResponseEditor1.Focus();
    }
    
    private void HeaderButton_Click(object sender, RoutedEventArgs e)
    {
        _lastTab = TabType.Header;
        UpdateEditorPosition();
        this.HeaderEditor1.Focus();
    }

    private void SplitResponseButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.Splitter1.RaiseEvent(
            new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = UIElement.MouseLeftButtonDownEvent,
                Source = e.Source
            });

        e.Handled = true;
    }

    private void SplitHeaderButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.Splitter2.RaiseEvent(
            new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = UIElement.MouseLeftButtonDownEvent,
                Source = e.Source
            });

        e.Handled = true;
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

    private void UpdateEditorLayouts(double width)
    {
        bool isNarrow = width < Config.SplitEditorThreshold;

        if (isNarrow)
        {
            Log.Debug("SingleView");

            // Sync editors
            PayloadEditorText = PayloadEditor2.Text;
            ResponseEditorText = ResponseEditor2.Text;
            HeaderEditorText = HeaderEditor2.Text;

            // Change View
            this.SingleView.Visibility = Visibility.Visible;
            this.SplitView.Visibility = Visibility.Collapsed;

            UpdateEditorPosition();
        }
        else
        {
            Log.Debug("SplitView");

            // Change View
            this.SingleView.Visibility = Visibility.Collapsed;
            this.SplitView.Visibility = Visibility.Visible;

            // Sync editors
            PayloadEditorText = PayloadEditor1.Text;
            ResponseEditorText = ResponseEditor1.Text;
            HeaderEditorText = HeaderEditor1.Text;
        }
    }

    private void UpdateEditorPosition()
    {
        // Reset All tabs colors
        var selectedColor = (Brush)FindResource("EditorsBackground");
        var defaultColor = (Brush)FindResource("UrlInputBackground");

        ((Border)PayloadButton.Template.FindName("border", PayloadButton)).Background = defaultColor;
        ((Border)ResponseButton.Template.FindName("border", ResponseButton)).Background = defaultColor;
        ((Border)HeaderButton.Template.FindName("border", HeaderButton)).Background = defaultColor;

        if (_lastTab == TabType.Payload)
        {
            PayloadEditor1.Visibility = Visibility.Visible;
            ResponseEditor1.Visibility = Visibility.Collapsed;
            HeaderEditor1.Visibility = Visibility.Collapsed;

            ((Border)PayloadButton.Template.FindName("border", PayloadButton)).Background = selectedColor;
        }
        else if (_lastTab == TabType.Response)
        {
            PayloadEditor1.Visibility = Visibility.Collapsed;
            ResponseEditor1.Visibility = Visibility.Visible;
            HeaderEditor1.Visibility = Visibility.Collapsed;

            ((Border)ResponseButton.Template.FindName("border", ResponseButton)).Background = selectedColor;
        }
        else if (_lastTab == TabType.Header)
        {
            PayloadEditor1.Visibility = Visibility.Collapsed;
            ResponseEditor1.Visibility = Visibility.Collapsed;
            HeaderEditor1.Visibility = Visibility.Visible;

            ((Border)HeaderButton.Template.FindName("border", HeaderButton)).Background = selectedColor;
        }
    }

    private void SetupEditors()
    {
        var editors = new[] {
            this.PayloadEditor1,
            this.PayloadEditor2,
            this.ResponseEditor1,
            this.ResponseEditor2,
            this.HeaderEditor1,
            this.HeaderEditor2
        };

        foreach (var editor in editors)
        {
            editor.Options.EnableEmailHyperlinks = false;
            editor.Options.EnableHyperlinks = false;
        }
        
        PayloadEditorSyntax = AppState.ResponseEditorSyntax;
        ResponseEditorSyntax = AppState.ResponseEditorSyntax;
    }

    private void SetStartupText()
    {
        #if !RELEASE
            PayloadEditorText = Config.PayloadIsFirstBootData;
            ResponseEditorText = Config.ResponseStartupData;
            HeaderEditorText = Config.HeaderStartupData;
            return;
        #endif

        #pragma warning disable CS0162

       if (AppState.IsFirstBoot)
        {
            PayloadEditorText = Config.PayloadIsFirstBootData;
            ResponseEditorText = Config.ResponseStartupData;
            HeaderEditorText = Config.HeaderStartupData;
            return;
        }

        var payload = Properties.Settings.Default.PayloadText ?? string.Empty;
        var response = Properties.Settings.Default.ResponseText ?? string.Empty;
        var header = Properties.Settings.Default.HeaderText ?? string.Empty;

        PayloadEditorText = payload == string.Empty
            ? Config.PayloadStartupData
            : payload;

        ResponseEditorText = response == string.Empty
            ? Config.ResponseStartupData
            : response;

        HeaderEditorText = header == string.Empty
            ? Config.HeaderStartupData
            : header;

        #pragma warning restore CS0162
    }

    public string GetPayloadText() => this.PayloadEditorText;

    public string GetResponseText() => this.ResponseEditorText;

    public string GetHeaderText() => this.HeaderEditorText;

    public void Reset()
    {
        // Payload
        AppState.PayloadEditorSyntax = SyntaxHighlighting.Json;
        PayloadEditorSyntax = SyntaxHighlighting.Json;
        PayloadEditorText = string.Empty;

        // Response
        AppState.ResponseEditorSyntax = SyntaxHighlighting.Json;
        ResponseEditorSyntax = SyntaxHighlighting.Json;
        ResponseEditorText = string.Empty;

        // Header
        HeaderEditorText = Config.HeaderStartupData;

        // Reset Grid Splitters in Split View
        ResetGridSplitterPositions();

        Window parentWindow = Window.GetWindow(this);
        UpdateEditorLayouts(parentWindow.ActualWidth);        
    }

    private void ResetGridSplitterPositions()
    {
        SplitView.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);    // Payload
        SplitView.ColumnDefinitions[1].Width = new GridLength(0);                       // Splitter1
        SplitView.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);    // Response
        SplitView.ColumnDefinitions[3].Width = new GridLength(0);                       // Splitter2
        SplitView.ColumnDefinitions[4].Width = new GridLength(1, GridUnitType.Star);    // Header
    }
    
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
}
