using System.Windows.Controls;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class UrlInput : UserControl
{
    public UrlInput()
    {
        InitializeComponent();
        UrlTextBox.Text = StartupData.GetUrlSampleData();
    }

    private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (UrlTextBox == null) return;

        var UrlHasText =
            !string.IsNullOrWhiteSpace(UrlTextBox!.Text) &&
            Uri.IsWellFormedUriString(UrlTextBox!.Text, UriKind.Absolute);

        //UrlTextBox.BorderBrush = UrlHasText ? Brushes.Green : Brushes.Gray; todo
        //UrlTextBox.BorderBrush = (Brush)TryFindResource(brushKey);
    }
}
