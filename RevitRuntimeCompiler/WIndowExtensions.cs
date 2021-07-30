using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace RevitRuntimeCompiler
{
    public static class WindowExtensions
    {
        public static void ShowWindow(this Window window)
        {
            var hWnd = Window.GetWindow(window);
            new WindowInteropHelper(hWnd).Handle
                .ShowWindow();
        }

        public static void ShowWindow(this IntPtr hWnd)
        {
            ShowWindow(hWnd, 1);
            SetForegroundWindow(hWnd);
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
