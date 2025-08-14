using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
    private enum FormState
    {
        HasText,
        UrlEmpty,
        JsonEmpty,
        BothEmpty
    }



    public MainWindow()
    {
        InitializeComponent();
        SetWindowSizeAndPosition();
        UpdateBorderColors();
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

        if (state != FormState.HasText)
        {
            await AnimateInvalidInputs(state);
            return;
        }

        MessageBox.Show($"URL: {url}\nJSON:\n{json}", "Debug");
    }

    private static FormState ValidateInputs(string url, string json)
    {
        bool isUrlEmpty = string.IsNullOrWhiteSpace(url);
        bool isJsonEmpty = string.IsNullOrWhiteSpace(json);

        if (isUrlEmpty && isJsonEmpty) return FormState.BothEmpty;
        if (isUrlEmpty) return FormState.UrlEmpty;
        if (isJsonEmpty) return FormState.JsonEmpty;

        return FormState.HasText;
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateBorderColors();
    }

    private void UpdateBorderColors()
    {
        if (UrlTextBox == null && JsonTextBox == null) return;

        var UrlhasText = !string.IsNullOrWhiteSpace(UrlTextBox!.Text);
        var JsonhasText = !string.IsNullOrWhiteSpace(JsonTextBox!.Text);

        UrlTextBox.BorderBrush = UrlhasText ? Brushes.Green : Brushes.Gray;
        JsonTextBox.BorderBrush = JsonhasText ? Brushes.Green : Brushes.Gray;
    }






    private async Task AnimateInvalidInputs(FormState formState)
    {
        const int durationMs = 350;
        const double shakeOffset = 3;

        var urlTextBoxColor = UrlTextBox.BorderBrush;
        var jsonTextBoxColor = JsonTextBox.BorderBrush;
        var submitButtonColor = SubmitButton.Background;
        var animation = GetAnimation(shakeOffset, durationMs);

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
