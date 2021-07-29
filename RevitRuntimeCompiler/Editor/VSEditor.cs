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
    public class VSEditor : IEditor
    {
        private Process vsProcess;
        private string _openedFilePath;

        public string EditorName => "vs";

        public void Edit(string filepath)
        {
            if (vsProcess != null && _openedFilePath == filepath)
            {
                TopWindow();
                return;
            }
            Close();
            _openedFilePath = filepath;
            vsProcess = new Process();
            vsProcess.StartInfo.FileName = "devenv.exe";
            vsProcess.StartInfo.Arguments = @$"""{_openedFilePath}""";
            vsProcess.Start();
            vsProcess.Exited += (s, e) => vsProcess = null;
        }

        public bool IsInstalled()
        {
            throw new NotImplementedException();
        }

        public void Close() => vsProcess?.Dispose();

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
