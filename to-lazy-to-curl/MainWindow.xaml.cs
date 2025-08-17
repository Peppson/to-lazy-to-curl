using System.Drawing;
using System.IO;
using System.Windows;
using FontAwesome.WPF;
using to_lazy_to_curl.Components;
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

        this.Width = Properties.Settings.Default.WindowWidth;
        this.Height = Properties.Settings.Default.WindowHeight;
        this.Top = Properties.Settings.Default.WindowTop;
        this.Left = Properties.Settings.Default.WindowLeft;
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
        string url = UrlInputControl.UrlInputText;
        string body = JsonInputControl.JsonRequestBody;

        SubmitButton.IsEnabled = false;
        await HttpService.SubmitRequestAsync(url, body);
        SubmitButton.IsEnabled = true;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        States.SelectedHttpAction = Models.HttpAction.NONE;
        MessageTextBlock.Opacity = 0;
        MessageTextBlock.Text = "";
        UrlInputControl.Reset();
        JsonInputControl.Reset();
    }
}
