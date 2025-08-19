using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace to_lazy_to_curl.Components;

public partial class ResizeHandle : UserControl
{
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTBOTTOMRIGHT = 17;

    public ResizeHandle()
    {
        InitializeComponent();
    }

    private void DragHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window == null) return;

        var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        SendMessage(hwnd, WM_NCLBUTTONDOWN, HTBOTTOMRIGHT, IntPtr.Zero);
    }
}
