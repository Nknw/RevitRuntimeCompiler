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
        private readonly string _originalSolutionPath;
        private readonly string _copySolutionPath;
        private readonly string _codePath;
        private readonly string _thisDir;

        public string EditorName => "vs";

        public VSSolutionEditor()
        {
            _thisDir = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);
            _originalSolutionPath = _thisDir + @"\EditorSolution";
            _copySolutionPath = _originalSolutionPath + "_";
            _codePath = _copySolutionPath + @"\Executor.cs";
            Refresh();
        }

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

        public string GetCode()
        {
            using var reader = new StreamReader( _codePath);
            return reader.ReadToEnd();
        }


        public void Refresh()
        {
            if (Directory.Exists(_copySolutionPath))
                Directory.Delete(_copySolutionPath, true);
            var target = Directory.CreateDirectory(_copySolutionPath);
            new DirectoryInfo(_originalSolutionPath).CopyFilesRecursively(target);
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
