using System.Windows;
using System.Windows.Controls;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Components;

public partial class JsonInput : UserControl
{
    private bool _isResponseEditor = false;
    private bool _isSplitMode = false;

    public string JsonText
    {
        get => JsonTextBox.Text;
        set => JsonTextBox.Text = value;
    }

    public JsonInput()
    {
        InitializeComponent();

        // Track window width for Json editor split or single view
        Loaded += (_, __) =>
        {
            Window window = Window.GetWindow(this)!;
            window.SizeChanged += Window_SizeChanged;

            UpdateEditorLayouts(window.ActualWidth);
        };
        JsonTextBox.Options.EnableHyperlinks = false;
        JsonTextBox.Options.EnableEmailHyperlinks = false;


        // todo
        JsonTextBox.Text = Config.JsonSampleData;
        ResponseEditor.Text = Config.UrlStartupData;


        HttpService.JsonInputEditor = JsonTextBox;
        //UiService.JsonInputEditor = JsonTextBox; // todo border instead
    }

    private void Window_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        double width = e.NewSize.Width;
        UpdateEditorLayouts(width);
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

    private void UpdateEditorPositions()
    {
        ResponseEditor.Visibility = _isResponseEditor ? Visibility.Visible : Visibility.Collapsed;
        JsonTextBox.Visibility = _isResponseEditor ? Visibility.Collapsed : Visibility.Visible;

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

    private void UpdateEditorLayouts(double width)
    {
        bool isNarrow = width < Config.SplitEditorThreshold;

        // Single view
        if (isNarrow)
        {
            _isSplitMode = false;
            UpdateEditorPositions();
            ModeButtonPanel.Visibility = Visibility.Visible;

            ResponseEditor.Visibility = _isResponseEditor ? Visibility.Visible : Visibility.Collapsed;
            JsonTextBox.Visibility = _isResponseEditor ? Visibility.Collapsed : Visibility.Visible;

            MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            MainGrid.ColumnDefinitions[1].Width = new GridLength(0);
            MainGrid.ColumnDefinitions[2].Width = new GridLength(0);
        }
        else // Split view
        {
            _isSplitMode = true;
            Grid.SetColumn(JsonTextBox, 0);
            Grid.SetColumn(ResponseEditor, 2);
            ModeButtonPanel.Visibility = Visibility.Hidden;

            JsonTextBox.Visibility = Visibility.Visible;
            ResponseEditor.Visibility = Visibility.Visible;

            MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            MainGrid.ColumnDefinitions[1].Width = new GridLength(10);
            MainGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);

        }
    }
}
