using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
    private CancellationTokenSource? _messageCts;
    private const int _messageDuration = 5000; // ms
    private const long _connectionTimeout = 5; // s

    private enum FormState
    {
        Filled,
        UrlEmpty,
        JsonEmpty,
        BothEmpty
    }



    public MainWindow()
    {
        InitializeComponent();
        SetWindowSizeAndPosition();
        UpdateBorderColors();

        JsonTextBox.Options.EnableHyperlinks = false;
        JsonTextBox.Options.EnableEmailHyperlinks = false;

        //#if !RELEASE
        LoadDebugSampleData();
        //#endif
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);
        Properties.Settings.Default.WindowWidth = this.Width;
        Properties.Settings.Default.WindowHeight = this.Height;
        Properties.Settings.Default.WindowTop = this.Top;
        Properties.Settings.Default.WindowLeft = this.Left;
        Properties.Settings.Default.Save();
    }

    private void SetWindowSizeAndPosition()
    {
        if (Properties.Settings.Default.WindowWidth > 0)
        {
            this.Width = Properties.Settings.Default.WindowWidth;
            this.Height = Properties.Settings.Default.WindowHeight;
            this.Top = Properties.Settings.Default.WindowTop;
            this.Left = Properties.Settings.Default.WindowLeft;
        }
    }

    private void LoadDebugSampleData()
    { 
        string jsonRaw = "{" +
            "\"id\": 42," +
            "\"name\": \"Name\"," +
            "\"email\": \"Name@example.com\"," +
            "\"isActive\": true," +
            "\"roles\": [\"admin\", \"programmer\"]," +
            "\"projects\": [" +
                "{" +
                    "\"id\": 1," +
                    "\"name\": \"Worst WPF App in History\"," +
                    "\"status\": \"Done-in-a-day-maybe\"" +
                "}," +
                "{" +
                    "\"id\": 2," +
                    "\"name\": \"ESP32 Shenanigans\"," +
                    "\"status\": \"Completed\"" +
                "}" +
            "]" +
        "}";

        JsonTextBox.Text = JsonConvert.SerializeObject(
            JsonConvert.DeserializeObject(jsonRaw), 
            Formatting.Indented
        ); 
        UrlTextBox.Text = "https://localhost:7291/snus/test";
    }

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitButton.IsEnabled = false;
        _ = ShowMessageAsync("Sending...", Brushes.Black, 10000);

        string url = UrlTextBox.Text;
        string json = JsonTextBox.Text;
        var state = ValidateInputs(url, json);
        var submitButtonColor = SubmitButton.Background;

        if (state != FormState.Filled)
        {
            _ = ShowMessageAsync("Please fill in all fields correctly!", Brushes.Red, _messageDuration);
            await InvalidInputAnimationAsync(state);
            return;
        }

        var (Success, ErrorMsg) = await SendPostRequestAsync(url, json);
        if (Success)
            _ = ShowMessageAsync("POST request sent successfully!", Brushes.Green, _messageDuration);
        else
            _ = ShowMessageAsync(ErrorMsg!, Brushes.Red, _messageDuration);

        SubmitButton.IsEnabled = true;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        UrlTextBox.Text = string.Empty;
        JsonTextBox.Text = string.Empty;
        UrlTextBox.BorderBrush = Brushes.Gray;
        JsonTextBox.BorderBrush = Brushes.Gray;
        MessageTextBlock.Opacity = 0;
        MessageTextBlock.Text = "";
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateBorderColors();
    }

    private static async Task<(bool Success, string? ErrorMsg)> SendPostRequestAsync(string url, string json)
    {
        const string genericError = "Error sending POST request!";
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(_connectionTimeout) };
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (false, "404: Endpoint not found!");
            }

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                var reason = response.ReasonPhrase;
                ShowMessageBox($"{genericError} \n\nStatus: {statusCode} \nReason: {reason}\n ");
                return (false, genericError);
            }

            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            ShowMessageBox($"{genericError}\n\n{ex.Message}");
            return (false, $"{genericError}");
        }
        catch (TaskCanceledException)
        {
            return (false, $"POST request timed out after {_connectionTimeout} seconds!");
        }
        catch (Exception ex)
        {
            ShowMessageBox($"{genericError}\n\n {ex.Message}");
            return (false, $"{genericError}");
        }
    }

    private static FormState ValidateInputs(string url, string json)
    {
        var isUrlEmpty = string.IsNullOrWhiteSpace(url);
        var isJsonEmpty = string.IsNullOrWhiteSpace(json);

        isUrlEmpty = isUrlEmpty || !Uri.IsWellFormedUriString(url, UriKind.Absolute);

        if (isUrlEmpty && isJsonEmpty) return FormState.BothEmpty;
        if (isUrlEmpty) return FormState.UrlEmpty;
        if (isJsonEmpty) return FormState.JsonEmpty;

        return FormState.Filled;
    }

    private void UpdateBorderColors()
    {
        if (UrlTextBox == null && JsonTextBox == null) return;

        var UrlHasText =
            !string.IsNullOrWhiteSpace(UrlTextBox!.Text) &&
            Uri.IsWellFormedUriString(UrlTextBox!.Text, UriKind.Absolute);

        var JsonHasText = !string.IsNullOrWhiteSpace(JsonTextBox!.Text);

        //UrlTextBox.BorderBrush = UrlHasText ? Brushes.Green : Brushes.Gray;
        JsonTextBox.BorderBrush = JsonHasText ? Brushes.Green : Brushes.Gray;
    }

    private async Task ShowMessageAsync(string message, SolidColorBrush color, int durationMs)
    {
        // Cancel any previous messages
        _messageCts?.Cancel();
        _messageCts = new CancellationTokenSource();
        var token = _messageCts.Token;

        MessageTextBlock.Text = message;
        MessageTextBlock.Foreground = color;

        // Fade in
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(1));
        MessageTextBlock.BeginAnimation(OpacityProperty, fadeIn);

        try
        {
            await Task.Delay(durationMs, token);

            // Fade out
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            MessageTextBlock.BeginAnimation(OpacityProperty, fadeOut);
        }
        catch (TaskCanceledException)
        {
            // Do nothing
        }
    }
 
    private static void ShowMessageBox(string msg)
    {
        MessageBox.Show(
            msg,
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }

    private async Task InvalidInputAnimationAsync(FormState formState)
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
    }

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
