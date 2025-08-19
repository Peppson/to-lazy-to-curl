using System.Windows;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SetWindowSizeAndPosition();
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
}
