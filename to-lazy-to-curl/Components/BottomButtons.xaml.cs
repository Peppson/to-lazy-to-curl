using System.Windows;
using System.Windows.Controls;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class BottomButtons : UserControl
{
    public BottomButtons()
    {
        InitializeComponent();
    }

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        string url = AppState.UrlInput.UrlTextBox.Text;
        string body = AppState.EditorInput.PayloadEditorText;

        SubmitButton.IsEnabled = false;
        await HttpService.SubmitRequestAsync(url, body);
        SubmitButton.IsEnabled = true;        
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        AnimationService.ClearButtonAnimation(sender);
        AppState.SelectedHttpAction = Models.HttpAction.NONE;
        AppState.EditorInput.Reset();
        AppState.HttpMessageBox.Reset();
    }
}
