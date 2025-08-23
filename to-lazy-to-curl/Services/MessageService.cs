using System.Windows;
using System.Windows.Media.Animation;

namespace to_lazy_to_curl.Services;

static public class MessageService
{
    private static CancellationTokenSource? _messageCts;

    public static async Task ShowTextMessageAsync(string msg, string colorName, int durationMs)
    {
        // Cancel any previous messages
        _messageCts?.Cancel();
        _messageCts = new CancellationTokenSource();

        var token = _messageCts.Token;
        AppState.MessageBox.MessageTextBlock.SetResourceReference(
            System.Windows.Controls.TextBlock.ForegroundProperty, colorName
        );

        AppState.MessageBox.MessageTextBlock.Text = msg;

        // Fade in
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(1));
        AppState.MessageBox.MessageTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeIn);

        // Fade out
        try
        {
            await Task.Delay(durationMs, token);

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            AppState.MessageBox.MessageTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeOut);
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
}
