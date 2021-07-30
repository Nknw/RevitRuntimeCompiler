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
                vsProcess.WaitForInputIdle();
                vsProcess.MainWindowHandle
                    .ShowWindow();
                return;
            }
            Close();
            _openedFilePath = filepath;
            vsProcess = new Process();
            vsProcess.StartInfo.FileName = "devenv.exe";
            vsProcess.StartInfo.Arguments = @$"""{_openedFilePath}""";
            vsProcess.EnableRaisingEvents = true;
            vsProcess.Start();
            vsProcess.Exited += (s, e) => Close();
        }

        public bool IsInstalled()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var vsWherePath = Path.Combine(programFiles, "Microsoft Visual Studio", "Installer", "vswhere.exe");
            return File.Exists(vsWherePath);
        }

        public void Close()
        {
            vsProcess?.Dispose();
            vsProcess = null;
        }
    }
}
