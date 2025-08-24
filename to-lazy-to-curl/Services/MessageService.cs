using System.Windows;
using System.Windows.Media.Animation;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.Settings;

namespace to_lazy_to_curl.Services;

static public class MessageService
{
    private static CancellationTokenSource? _messageCts;

    public static async Task ShowTextMessageAsync(
        string message,
        string color,
        StatusIcon icon = StatusIcon.None,
        int durationMs = Config.StatusMessageDuration)
    {
        // Cancel any previous messages
        _messageCts?.Cancel();
        _messageCts = new CancellationTokenSource();
        var token = _messageCts.Token;

        SetMessageText(message, color);
        SetMessageIcon(icon);

        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(1));
        AppState.HttpMessageBox.MessageTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        AppState.HttpMessageBox.MessageTextBlockSpinner.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        AppState.HttpMessageBox.MessageTextBlockSuccess.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        AppState.HttpMessageBox.MessageTextBlockFailure.BeginAnimation(UIElement.OpacityProperty, fadeIn);

        try
        {
            await Task.Delay(durationMs, token);

            // Fade out
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            AppState.HttpMessageBox.MessageTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            AppState.HttpMessageBox.MessageTextBlockSpinner.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            AppState.HttpMessageBox.MessageTextBlockSuccess.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            AppState.HttpMessageBox.MessageTextBlockFailure.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }
        catch (TaskCanceledException) { }
    }

    public static void ShowMessageBox(string msg)
    {
        MessageBox.Show(
            msg,
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }

    private static void SetMessageText(string message, string color)
    {
        AppState.HttpMessageBox.MessageTextBlock.SetResourceReference(
            System.Windows.Controls.TextBlock.ForegroundProperty, color
        );
        AppState.HttpMessageBox.MessageTextBlock.Text = message;
    }

    private static void SetMessageIcon(StatusIcon icon)
    {
        // Visability
        AppState.HttpMessageBox.MessageTextBlockSpinner.Visibility = Visibility.Collapsed;
        AppState.HttpMessageBox.MessageTextBlockSuccess.Visibility = Visibility.Collapsed;
        AppState.HttpMessageBox.MessageTextBlockFailure.Visibility = Visibility.Collapsed;

        if (icon == StatusIcon.Send)
            AppState.HttpMessageBox.MessageTextBlockSpinner.Visibility = Visibility.Visible;
        else if (icon == StatusIcon.Success)
            AppState.HttpMessageBox.MessageTextBlockSuccess.Visibility = Visibility.Visible;
        else if (icon == StatusIcon.Failure)
            AppState.HttpMessageBox.MessageTextBlockFailure.Visibility = Visibility.Visible;
    }
}
