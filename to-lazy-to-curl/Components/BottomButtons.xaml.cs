using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        AnimateSendButton(true);
        await HttpService.SubmitRequestAsync(url, body);
        AnimateSendButton(false);
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        AnimateResetButton(sender, e);
        AppState.SelectedHttpAction = Models.HttpAction.NONE;
        AppState.EditorInput.Reset();
        AppState.MessageBox.Reset();
    }

    private void AnimateSendButton(bool isSending)
    {
        if (isSending)
        {
            SubmitButton.IsEnabled = false;
            SendButtonStaticIcon.Visibility = Visibility.Collapsed;
            SendButtonSpinnerIcon.Visibility = Visibility.Visible;

        }
        else
        {
            SendButtonSpinnerIcon.Visibility = Visibility.Collapsed;
            SendButtonStaticIcon.Visibility = Visibility.Visible;
            SubmitButton.IsEnabled = true;
        }
    }
    
    private void AnimateResetButton(object sender, RoutedEventArgs e)
    {
        var rotateAnimation = new DoubleAnimation
        {
            From = 0,
            To = 180,
            Duration = TimeSpan.FromSeconds(0.2),
            FillBehavior = FillBehavior.Stop
        };
        ClearButtonRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
    }
}
