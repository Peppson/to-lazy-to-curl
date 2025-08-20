using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Serilog;
using to_lazy_to_curl.Services;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Media;

namespace to_lazy_to_curl;

public partial class MainWindow : Window
{
	private static bool _isDarkTheme;

	public MainWindow()
	{
		InitializeComponent();
		InitWindowSizeAndPosition();
		InitColorTheme();
		LogService.Init();
		RefreshMaximizeRestoreButton(); // todo
	}

	private void ToggleTheme_Click(object sender, RoutedEventArgs e)
	{
		_isDarkTheme = !_isDarkTheme;
		SetColorTheme(_isDarkTheme);
		SetSyntaxColorTheme(_isDarkTheme);
	}

	protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
	{
		base.OnClosing(e);
		LogService.Shutdown();

		Properties.Settings.Default.IsDarkTheme = _isDarkTheme;
		Properties.Settings.Default.WindowWidth = Width;
		Properties.Settings.Default.WindowHeight = Height;
		Properties.Settings.Default.WindowTop = Top;
		Properties.Settings.Default.WindowLeft = Left;
		Properties.Settings.Default.Save();
	}

	private void InitWindowSizeAndPosition()
	{
		if (Properties.Settings.Default.WindowWidth <= 0) return;

		Width = Properties.Settings.Default.WindowWidth;
		Height = Properties.Settings.Default.WindowHeight;
		Top = Properties.Settings.Default.WindowTop;
		Left = Properties.Settings.Default.WindowLeft;
	}

	private static void InitColorTheme()
	{
		_isDarkTheme = Properties.Settings.Default.IsDarkTheme;
		SetColorTheme(_isDarkTheme);
		SetSyntaxColorTheme(_isDarkTheme);
	}

	private static void SetColorTheme(bool darkTheme)
	{
		Log.Debug($"Theme: {(darkTheme ? "Dark" : "Light")}");

		var dict = new ResourceDictionary
		{
			Source = new Uri(
				darkTheme ? "Settings/Colors.Dark.xaml" : "Settings/Colors.Light.xaml",
				UriKind.Relative)
		};

		Application.Current.Resources.MergedDictionaries[0].Clear();
		Application.Current.Resources.MergedDictionaries.Add(dict);
	}
	
	private static void SetSyntaxColorTheme(bool darkTheme)
    {	
		Log.Debug($"Syntax theme: {(darkTheme ? "Dark" : "Light")}");

		// Set syntax colors
		var syntaxColor = AppState.JsonInput.JsonTextBox.SyntaxHighlighting;
		foreach (var color in syntaxColor.NamedHighlightingColors)
		{
			if (Application.Current.FindResource(color.Name) is SolidColorBrush brush)
				color.Foreground = new SimpleHighlightingBrush(brush.Color);
		}

		AppState.JsonInput.JsonTextBox.TextArea.TextView.Redraw();
		AppState.JsonInput.ResponseEditor.TextArea.TextView.Redraw();
    }






























	private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
	{
		this.WindowState = WindowState.Minimized;
	}

	private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
	{
		if (this.WindowState == WindowState.Maximized)
		{
			this.WindowState = WindowState.Normal;
		}
		else
		{
			this.WindowState = WindowState.Maximized;
		}
	}

	private void OnCloseButtonClick(object sender, RoutedEventArgs e)
	{
		this.Close();
	}


	private void RefreshMaximizeRestoreButton()
	{
		if (this.WindowState == WindowState.Maximized)
		{
			this.maximizeButton.Visibility = Visibility.Collapsed;
			this.restoreButton.Visibility = Visibility.Visible;
		}
		else
		{
			this.maximizeButton.Visibility = Visibility.Visible;
			this.restoreButton.Visibility = Visibility.Collapsed;
		}
	}


	private void Window_StateChanged(object sender, EventArgs e)
	{
		this.RefreshMaximizeRestoreButton();
	}


	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
		((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
	}

	public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == WM_GETMINMAXINFO)
		{
			// We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
			// including the task bar.
			MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

			// Adjust the maximized size and position to fit the work area of the correct monitor
			IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

			if (monitor != IntPtr.Zero)
			{
				MONITORINFO monitorInfo = new MONITORINFO();
				monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
				GetMonitorInfo(monitor, ref monitorInfo);
				RECT rcWorkArea = monitorInfo.rcWork;
				RECT rcMonitorArea = monitorInfo.rcMonitor;
				mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
				mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
				mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
				mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
			}

			Marshal.StructureToPtr(mmi, lParam, true);
		}

		return IntPtr.Zero;
	}

	private const int WM_GETMINMAXINFO = 0x0024;

	private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

	[DllImport("user32.dll")]
	private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

	[DllImport("user32.dll")]
	private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public RECT(int left, int top, int right, int bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MONITORINFO
	{
		public int cbSize;
		public RECT rcMonitor;
		public RECT rcWork;
		public uint dwFlags;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public int X;
		public int Y;

		public POINT(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MINMAXINFO
	{
		public POINT ptReserved;
		public POINT ptMaxSize;
		public POINT ptMaxPosition;
		public POINT ptMinTrackSize;
		public POINT ptMaxTrackSize;
	}



}