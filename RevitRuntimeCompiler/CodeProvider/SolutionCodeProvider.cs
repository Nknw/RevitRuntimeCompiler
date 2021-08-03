using RevitRuntimeCompiler.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace RevitRuntimeCompiler.CodeProvider
{
    public class SolutionCodeProvider : ICodeProvider
    {
        private readonly string _originalSolutionPath;
        private readonly string _copySolutionPath;
        private readonly string _codePath;
        private readonly string _thisDir;
        private readonly string _solutionAndProjectName = "EditorSolution";
        private readonly DotNetSolutionInfo _solutionInfo;

        public string ExecutableFilePath => _copySolutionPath + @$"\{_solutionAndProjectName}.sln";

        public SolutionCodeProvider(string revitVersion, DotNetSolutionInfo solutionInfo)
        {
            _solutionInfo = solutionInfo;
            _thisDir = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);
            _originalSolutionPath = Path.Combine(_thisDir, _solutionInfo.SolutionName);
            _copySolutionPath = _originalSolutionPath + "_";
            _codePath = Path.Combine(_copySolutionPath, _solutionInfo.CodeFileName);
            PatchProject(revitVersion);
            Refresh(false);
        }

        private void PatchProject(string revitVersion)
        {
            using var revitRGkey = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Autodesk\Revit\{revitVersion}");
            var revitKey = revitRGkey.GetSubKeyNames().First(s => s.StartsWith("REVIT"));
            using var strangeDir = revitRGkey.OpenSubKey(revitKey);
            var revitPath = strangeDir.GetValue("InstallationLocation");
            var projectPath = Path.Combine(_originalSolutionPath, _solutionInfo.ProjectFileName);
            var rewritedProj = "";
            using (var reader = new StreamReader(projectPath))
            {
                rewritedProj = string.Format(reader.ReadToEnd(), revitPath);
            }
            using var writer = new StreamWriter(projectPath);
            writer.Write(rewritedProj);
        }

        public string GetCode()
        {
            using var reader = new StreamReader(_codePath);
            return reader.ReadToEnd();
        }

        public void Refresh() => Refresh(true);

        private void Refresh(bool shouldOverwrite)
        {
            if (Directory.Exists(_copySolutionPath))
            {
                if (!shouldOverwrite)
                    return;
                Directory.Delete(_copySolutionPath, true);
            }
            var target = Directory.CreateDirectory(_copySolutionPath);
            new DirectoryInfo(_originalSolutionPath)
                .CopyFilesRecursively(target, info => !(info is DirectoryInfo d && d.Name.StartsWith(".")));
        }
    }
}
