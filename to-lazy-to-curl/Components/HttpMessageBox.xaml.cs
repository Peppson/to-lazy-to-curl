using System.Windows;
using System.Windows.Controls;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class HttpMessageBox : UserControl
{
    public HttpMessageBox()
    {
        InitializeComponent();
        AppState.HttpMessageBox = this;
    }

    public void Reset()
    {
        this.MessageTextBlock.Opacity = 0;
        this.MessageTextBlock.Text = "";
        this.MessageTextBlockSpinner.Visibility = Visibility.Collapsed;
        this.MessageTextBlockSuccess.Visibility = Visibility.Collapsed;
        this.MessageTextBlockFailure.Visibility = Visibility.Collapsed;
    }
}
