using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Net.Http;
using System.Text;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Highlighting;
using Newtonsoft.Json;
using to_lazy_to_curl.Components;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Services;

static public class UiService
{
    public static TextBlock MessageTextBlock { get; set; } = null!;
    public static Border JsonEditorBorder { get; set; } = null!;
    public static Border UrlInputBorder { get; set; } = null!;
    private static CancellationTokenSource? _messageCts;
        
    public static async Task ShowMessageAsync(string msg, string colorName, int durationMs)
    {
        // Cancel any previous messages
        _messageCts?.Cancel();
        _messageCts = new CancellationTokenSource();
        var token = _messageCts.Token;
        var color = (SolidColorBrush)Application.Current.FindResource(colorName);

        MessageTextBlock.Text = msg;
        MessageTextBlock.Foreground = color;

        // Fade in
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(1));
        MessageTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeIn);

        // Fade out
        try
        {
            await Task.Delay(durationMs, token);

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            MessageTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }
        catch (TaskCanceledException)
        {
            // Do nothing :)
        }
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

   

    /* private static async Task InvalidInputAnimationAsync(FormState formState)
    {
        const int durationMs = 450;
        const double shakeOffset = 3;
        var animation = GetAnimation(shakeOffset, durationMs);

        var urlTextBoxColor = UrlTextBox.BorderBrush;
        var jsonTextBoxColor = JsonTextBox.BorderBrush;
        var submitButtonColor = SubmitButton.Background;

        // Button
        SubmitButton.Background = Brushes.Red;
        SubmitButton.RenderTransform = new TranslateTransform();
        SubmitButton.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);


        // todo
        //JsonEditorBorder.BorderBrush = new SolidColorBrush(Colors.Red);
        //UrlInputBorder.BorderBrush = new SolidColorBrush(Colors.Red);




        // Fields
        if (formState == FormState.UrlEmpty || formState == FormState.BothEmpty)
        {
            UrlTextBox.BorderBrush = Brushes.Red;
            UrlTextBox.RenderTransform = new TranslateTransform();
            UrlTextBox.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        if (formState == FormState.JsonEmpty || formState == FormState.BothEmpty)
        {
            JsonTextBox.BorderBrush = Brushes.Red;
            JsonTextBox.RenderTransform = new TranslateTransform();
            JsonTextBox.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        await Task.Delay(durationMs);
        SubmitButton.Background = submitButtonColor;
        UrlTextBox.BorderBrush = urlTextBoxColor;
        JsonTextBox.BorderBrush = jsonTextBoxColor;

        SubmitButton.IsEnabled = true;
    } */

    private static DoubleAnimationUsingKeyFrames GetAnimation(double offset, int durationMs)
    {
        var animation = new DoubleAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromMilliseconds(durationMs)
        };

        animation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromPercent(0)));
        animation.KeyFrames.Add(new EasingDoubleKeyFrame(-offset, KeyTime.FromPercent(0.1)));
        animation.KeyFrames.Add(new EasingDoubleKeyFrame(offset, KeyTime.FromPercent(0.2)));
        animation.KeyFrames.Add(new EasingDoubleKeyFrame(-offset, KeyTime.FromPercent(0.3)));
        animation.KeyFrames.Add(new EasingDoubleKeyFrame(offset, KeyTime.FromPercent(0.4)));
        animation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromPercent(0.5)));

        return animation;
    }
}
