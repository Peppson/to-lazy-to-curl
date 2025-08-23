using System.Windows;
using System.Windows.Controls;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class PopupMenu : UserControl
{
    private static bool _loaded = false; // Guard against a strange bug...

    public bool IsOpen
    {
        get => this.PopupMenuWindow.IsOpen;
        set => this.PopupMenuWindow.IsOpen = value;
    }

    public PopupMenu()
    {
        InitializeComponent();
        AppState.PopupMenu = this;

        Loaded += (_, __) =>
        {
            this.ThemeToggleCheckBox.IsChecked = AppState.IsDarkTheme;
            _loaded = true;
        };
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (!_loaded) return;
        ThemeService.ToggleTheme();
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (!_loaded) return;
        ThemeService.ToggleTheme();
    }
    
    public void ToggleAtPosition(Button position)
    {   
        this.PopupMenuWindow.PlacementTarget = position;
        this.IsOpen = !this.IsOpen;
    }
}
