using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using to_lazy_to_curl.Properties;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
    private readonly Brush _emptyBorder = Brushes.Gray;
    private readonly Brush _filledBorder = Brushes.Green;

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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string url = UrlTextBox.Text;
        string json = JsonTextBox.Text;

        if (!ValidateInputs(url, json))
            return; 

        // For now, just show what was entered
        MessageBox.Show($"URL: {url}\nJSON:\n{json}", "Debug");
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateBorderColors();
    }

    private void UpdateBorderColors()
    {
        if (UrlTextBox == null && JsonTextBox == null) return;

        var UrlhasText = !string.IsNullOrWhiteSpace(UrlTextBox.Text);
        var JsonhasText = !string.IsNullOrWhiteSpace(JsonTextBox.Text);

        UrlTextBox.BorderBrush = UrlhasText ? _filledBorder : _emptyBorder;
        JsonTextBox.BorderBrush = JsonhasText ? _filledBorder : _emptyBorder;


        Console.WriteLine($"URL TextBox Border: {UrlTextBox.BorderBrush}, JSON TextBox Border: {JsonTextBox.BorderBrush}");
    }

    private static bool ValidateInputs(string url, string json)
    {
        if (string.IsNullOrWhiteSpace(url) || url.Length > 300)
        {
            MessageBox.Show("Please enter a URL.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (string.IsNullOrWhiteSpace(json) || json.Length > 700)
        {
            MessageBox.Show("Please enter JSON data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }

    
}
