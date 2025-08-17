using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Components;

public partial class UrlInput : UserControl
{
    public string UrlInputText
    {
        get => UrlTextBox.Text;
        set => UrlTextBox.Text = value;
    }

    public UrlInput()
    {
        InitializeComponent();
        UrlInputText = Config.UrlStartupData;
        UiService.UrlInputBorder = UrlBorder;
    }

    private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (UrlTextBox == null) return;

        /*var validUrl =
            !string.IsNullOrWhiteSpace(UrlTextBox!.Text) &&
            Uri.IsWellFormedUriString(UrlTextBox!.Text, UriKind.Absolute);

        var validColor = Brushes.Green;
        var defaultBorderColor = (Brush)(TryFindResource("Border") ?? Brushes.LightGray);
        UrlBorder.BorderBrush = validUrl ? validColor : defaultBorderColor;*/
    }

    public void Reset()
    {
        UrlInputText = string.Empty;
        //UrlBorder.BorderBrush = (Brush)Application.Current.FindResource("Border");
    }
}
