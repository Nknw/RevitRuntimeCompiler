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
            vsProcess.Start();
            vsProcess.Exited += (s, e) => vsProcess = null;
        }

        public bool IsInstalled()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(@"""C:\Program Files(x86)\Microsoft Visual Studio\Installer\vswhere.exe"" -property catalog_productDisplayVersion");
            return char.IsNumber(cmd.StandardOutput.ReadLine().First());
        }

        public void Close() => vsProcess?.Dispose();
    }
}
