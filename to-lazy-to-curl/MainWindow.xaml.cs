using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
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

        UrlTextBox.Text = "https://localhost:7291/snus/test";
        //JsonTextBox.Text = @"{""name"":""John Doe"",""age"":30,""city"":""New York""}";
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

    

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        string url = UrlTextBox.Text;
        string json = JsonTextBox.Text;
        var state = ValidateInputs(url, json);
        var submitButtonColor = SubmitButton.Background;

        if (state != FormState.Filled)
        {
            var tasks = new List<Task>
            {
                ShowMessageAsync("Please fill in all fields correctly!", Brushes.Red, 3000),
                AnimateInvalidInputs(state)
            };
            await Task.WhenAll(tasks);

            return;
        }

        //SubmitButton.Background = Brushes.Green;


        if (await SendPostRequestAsync(url, json))
        {
            Console.WriteLine("POST request sent successfully!");
        }
        else
        {
            Console.WriteLine("Failed to send POST request.");
        }

        SubmitButton.Background = submitButtonColor;
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





    private static async Task<bool> SendPostRequestAsync(string url, string json)
    {
        try
        {
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            return response.IsSuccessStatusCode;
        }
        catch (TaskCanceledException)
        {
            MessageBox.Show("POST request timed out after 5 seconds.", "Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error sending POST request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private static FormState ValidateInputs(string url, string json)
    {
        bool isUrlEmpty = string.IsNullOrWhiteSpace(url);
        bool isJsonEmpty = string.IsNullOrWhiteSpace(json);

        isUrlEmpty = isUrlEmpty || !Uri.IsWellFormedUriString(url, UriKind.Absolute);

        if (isUrlEmpty && isJsonEmpty) return FormState.BothEmpty;
        if (isUrlEmpty) return FormState.UrlEmpty;
        if (isJsonEmpty) return FormState.JsonEmpty;

        return FormState.Filled;
    }

    

    private void UpdateBorderColors()
    {
        if (UrlTextBox == null && JsonTextBox == null) return;

        var UrlhasText =
            !string.IsNullOrWhiteSpace(UrlTextBox!.Text) &&
            Uri.IsWellFormedUriString(UrlTextBox!.Text, UriKind.Absolute);

        var JsonhasText = !string.IsNullOrWhiteSpace(JsonTextBox!.Text);

        UrlTextBox.BorderBrush = UrlhasText ? Brushes.Green : Brushes.Gray;
        JsonTextBox.BorderBrush = JsonhasText ? Brushes.Green : Brushes.Gray;
    }


    private async Task ShowMessageAsync(string message, SolidColorBrush color, int durationMs)
    { 
        MessageTextBlock.Text = message;
        MessageTextBlock.Foreground = color;
        MessageTextBlock.Opacity = 1;

        // Fade in
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(1));
        MessageTextBlock.BeginAnimation(OpacityProperty, fadeIn);

        // Wait
        await Task.Delay(durationMs);

        // Fade out
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
        MessageTextBlock.BeginAnimation(OpacityProperty, fadeOut);
    }






    private async Task AnimateInvalidInputs(FormState formState)
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
