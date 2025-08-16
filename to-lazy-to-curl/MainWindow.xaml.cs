using System.Windows;
using to_lazy_to_curl.State;

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

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);
        Properties.Settings.Default.WindowWidth = this.Width;
        Properties.Settings.Default.WindowHeight = this.Height;
        Properties.Settings.Default.WindowTop = this.Top;
        Properties.Settings.Default.WindowLeft = this.Left;
        Properties.Settings.Default.Save();
    }

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitButton.IsEnabled = false;
        Console.WriteLine("TEST");
        /* _ = ShowMessageAsync("Sending...", Brushes.Black, 10000);

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
            _ = ShowMessageAsync(ErrorMsg!, Brushes.Red, _messageDuration); */

        SubmitButton.IsEnabled = true;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        States.SelectedAction = Models.HttpAction.None;

        /*UrlTextBox.Text = string.Empty;
        JsonTextBox.Text = string.Empty;
        UrlTextBox.BorderBrush = Brushes.Gray;
        JsonTextBox.BorderBrush = Brushes.Gray;
        MessageTextBlock.Opacity = 0;
        MessageTextBlock.Text = "";*/
    }
}
