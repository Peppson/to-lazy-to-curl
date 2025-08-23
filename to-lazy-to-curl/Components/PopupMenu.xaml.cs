using System.Windows.Controls;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class PopupMenu : UserControl
{
    public bool IsOpen
    {
        get => this.PopupMenuWindow.IsOpen;
        set => this.PopupMenuWindow.IsOpen = value;
    }

    public PopupMenu()
    {
        InitializeComponent();
        AppState.PopupMenu = this;
    }
}

