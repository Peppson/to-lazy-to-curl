using System.Windows.Controls;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class HttpMessageBox : UserControl
{
    public HttpMessageBox()
    {
        InitializeComponent();
        AppState.MessageBox = this;
    }

    public void Reset()
    {
        MessageTextBlock.Opacity = 0;
        MessageTextBlock.Text = "";
    }
}
