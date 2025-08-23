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

        // Init URL with sample data or saved text
        this.UrlTextBox.Text = AppState.IsFirstBoot
            ? Config.UrlSampleData
            : Properties.Settings.Default.UrlInputText ?? string.Empty;

        #if !RELEASE
            this.UrlTextBox.Text = Config.UrlSampleData;
        #endif
    }

    public string GetUrlText() => this.UrlTextBox.Text;

    public void Reset() => this.UrlTextBox.Text = string.Empty;
}
