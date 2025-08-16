using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Components;

public partial class HttpButton : UserControl
{
    public HttpAction ActionType { get; set; } = HttpAction.None;

    public HttpButton()
    {
        InitializeComponent();
        MainButton.Click += (_, __) =>
        {
            States.SelectedAction = (States.SelectedAction == this.ActionType)
                ? HttpAction.None 
                : this.ActionType;
        };

        EventHandler handler = (_, __) =>
        {
            IsSelected = States.SelectedAction == this.ActionType;
        };

        States.SelectedActionChanged += handler;
        this.Unloaded += (_, __) => States.SelectedActionChanged -= handler;
    }

    public static readonly DependencyProperty IsSelectedProperty =
    DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(HttpButton),
        new PropertyMetadata(false));

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly DependencyProperty ButtonContentProperty =
        DependencyProperty.Register(
            nameof(ButtonContent),
            typeof(object),
            typeof(HttpButton),
            new PropertyMetadata(null));

    public object ButtonContent
    {
        get => GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }

    public static readonly DependencyProperty ButtonColorProperty =
    DependencyProperty.Register(
        nameof(ButtonColor),
        typeof(Brush),
        typeof(HttpButton),
        new PropertyMetadata(Brushes.Gray));

    public Brush ButtonColor
    {
        get => (Brush)GetValue(ButtonColorProperty);
        set => SetValue(ButtonColorProperty, value);
    }
}
