using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace RevitRuntimeCompiler.Editor
{
    public class VSSolutionEditor : IEditor
    {
        private Process vsProcess;

        public string EditorName => "vs";

        public void Edit()
        {
            if (vsProcess != null)
            {
                TopWindow();
                return;
            }
            vsProcess = new Process();
            vsProcess.StartInfo.FileName = "devenv.exe";
            vsProcess.StartInfo.Arguments = @$"""{_copySolutionPath + @"\EditorSolution.sln"}""";
            vsProcess.Start();
            vsProcess.Exited += (s, e) => vsProcess = null;
        }

        public void Dispose() => vsProcess?.Dispose();

        private void TopWindow()
        {
            vsProcess.WaitForInputIdle();
            ShowWindow(vsProcess.MainWindowHandle, 1);
            SetForegroundWindow(vsProcess.MainWindowHandle);
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
