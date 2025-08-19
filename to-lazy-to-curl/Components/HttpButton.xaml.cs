using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class HttpButton : UserControl
{
    public HttpAction ActionType { get; set; } = HttpAction.NONE;

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public object ButtonContent
    {
        get => GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }

    public Brush ButtonColor
    {
        get => (Brush)GetValue(ButtonColorProperty);
        set => SetValue(ButtonColorProperty, value);
    }

    public HttpButton()
    {
        InitializeComponent();
        MainButton.Click += MainButton_Click;

        EventHandler handler = (_, __) =>
        {
            IsSelected = AppState.SelectedHttpAction == this.ActionType;
        };
        AppState.SelectedHttpButtonEvent += handler;

        Unloaded += (_, __) => AppState.SelectedHttpButtonEvent -= handler;
    }

    private void MainButton_Click(object sender, RoutedEventArgs e)
    {
        AppState.SelectedHttpAction = (AppState.SelectedHttpAction == this.ActionType)
            ? HttpAction.NONE
            : this.ActionType;
    }

    public static readonly DependencyProperty IsSelectedProperty =
    DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(HttpButton),
        new PropertyMetadata(false));

    public static readonly DependencyProperty ButtonContentProperty =
        DependencyProperty.Register(
            nameof(ButtonContent),
            typeof(object),
            typeof(HttpButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty ButtonColorProperty =
    DependencyProperty.Register(
        nameof(ButtonColor),
        typeof(Brush),
        typeof(HttpButton),
        new PropertyMetadata(Brushes.Gray));
}
