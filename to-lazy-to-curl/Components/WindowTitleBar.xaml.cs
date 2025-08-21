using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using FontAwesome.WPF;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class WindowTitleBar : UserControl
{
    private const int WM_GETMINMAXINFO = 0x0024;
	private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
    private Window _parentWindow = null!;

	[DllImport("user32.dll")]
	private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

	[DllImport("user32.dll")]
	private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT(int left, int top, int right, int bottom)
    {
		public int Left = left;
		public int Top = top;
		public int Right = right;
		public int Bottom = bottom;
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
	public struct POINT(int x, int y)
    {
		public int X = x;
		public int Y = y;
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

    public WindowTitleBar()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void ToggleTheme_Click(object sender, RoutedEventArgs e)
    {
        ThemeService.ToggleTheme();
        SetThemeIcon(); 
    }
    
    private void OnMinimizeButton_Click(object sender, RoutedEventArgs e) =>
        _parentWindow.WindowState = WindowState.Minimized;
	
	private void OnMaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (_parentWindow.WindowState == WindowState.Maximized)
            _parentWindow.WindowState = WindowState.Normal;
        else
            _parentWindow.WindowState = WindowState.Maximized;
    }

	private void OnCloseButton_Click(object sender, RoutedEventArgs e) => _parentWindow.Close();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SetThemeIcon();    
        _parentWindow = Window.GetWindow(this);
        _parentWindow.StateChanged += (_, __) => RefreshMaximizeRestoreButton();
        RefreshMaximizeRestoreButton();
    }

    private void SetThemeIcon()
    {
        if (AppState.IsDarkTheme)
        {
            ThemeIcon.Icon = FontAwesomeIcon.SunOutline;
            ThemeIcon.Margin = new Thickness(0, -3, 0, 0);
        }
        else
        {
            ThemeIcon.Icon = FontAwesomeIcon.MoonOutline;
            ThemeIcon.Margin = new Thickness(0, -2, 0, 0);
        }
    }

	private void RefreshMaximizeRestoreButton()
    {
        if (_parentWindow.WindowState == WindowState.Maximized)
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

    public void InitializeHook()
    {
        var window = Window.GetWindow(this);
        if (window != null)
        {
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
        }
    }
    
	public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_GETMINMAXINFO)
        {
            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
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
}
