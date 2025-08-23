using System.Windows;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitIsFirstBoot();
		InitializeComponent();
		InitWindowSizeAndPosition();
		AppState.MainWindow = this;
		LogService.Init();
		ThemeService.Init();
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
		TopTitleBar.InitializeHook();
	}

	private void InitWindowSizeAndPosition()
	{
		if (Properties.Settings.Default.WindowWidth <= 0) return;

		Width = Properties.Settings.Default.WindowWidth;
		Height = Properties.Settings.Default.WindowHeight;
		Top = Properties.Settings.Default.WindowTop;
		Left = Properties.Settings.Default.WindowLeft;
	}

	private void InitIsFirstBoot()
	{
		if (Properties.Settings.Default.IsFirstBoot)
		{
			Properties.Settings.Default.IsFirstBoot = false;
			Properties.Settings.Default.Save();
			AppState.IsFirstBoot = true;
		}
		else
		{
			AppState.IsFirstBoot = false;
		}
	}

	protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
	{
		base.OnClosing(e);
		LogService.Shutdown();

		Properties.Settings.Default.WindowWidth = Width;
		Properties.Settings.Default.WindowHeight = Height;
		Properties.Settings.Default.WindowTop = Top;
		Properties.Settings.Default.WindowLeft = Left;

		Properties.Settings.Default.UrlInputText = AppState.UrlInput.GetUrlText();
		Properties.Settings.Default.PayloadText = AppState.EditorInput.GetPayloadText();
		Properties.Settings.Default.ResponseText = AppState.EditorInput.GetResponseText();
		Properties.Settings.Default.HeaderText = AppState.EditorInput.GetHeaderText();
		Properties.Settings.Default.IsDarkTheme = ThemeService.GetIsDarkTheme();

		Properties.Settings.Default.Save();
	}
}
