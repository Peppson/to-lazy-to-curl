using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SetWindowSizeAndPosition();
        UiService.MessageTextBlock = MessageTextBlock;
    }

    private void SetWindowSizeAndPosition()
    {
        if (Properties.Settings.Default.WindowWidth <= 0) return;

        Width = Properties.Settings.Default.WindowWidth;
        Height = Properties.Settings.Default.WindowHeight;
        Top = Properties.Settings.Default.WindowTop;
        Left = Properties.Settings.Default.WindowLeft;
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);
        Properties.Settings.Default.WindowWidth = Width;
        Properties.Settings.Default.WindowHeight = Height;
        Properties.Settings.Default.WindowTop = Top;
        Properties.Settings.Default.WindowLeft = Left;
        Properties.Settings.Default.Save();
    }

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        string url = UrlInputControl.UrlInputText;
        string body = JsonInputControl.JsonRequestBody;

        AnimateSendButton(true);
        await HttpService.SubmitRequestAsync(url, body);
        AnimateSendButton(false);
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        AnimateResetButton(sender, e);
        AppState.SelectedHttpAction = Models.HttpAction.NONE;
        MessageTextBlock.Opacity = 0;
        MessageTextBlock.Text = "";
        UrlInputControl.Reset();
        JsonInputControl.Reset();
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
