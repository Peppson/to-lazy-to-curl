using System.Windows.Controls;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.Settings;

namespace to_lazy_to_curl.Components;

public partial class UrlInput : UserControl
{
    public UrlInput()
    {
        InitializeComponent();
        AppState.UrlInput = this;
        UrlTextBox.Text = Config.UrlStartupData;
    }

    private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Todo border colors?
    }

    public void Reset()
    {
        UrlTextBox.Text = string.Empty;
    }
}
