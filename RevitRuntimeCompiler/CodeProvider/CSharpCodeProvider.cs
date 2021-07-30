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
    public class CSharpCodeProvider : ICodeProvider
    {
        private readonly string _originalSolutionPath;
        private readonly string _copySolutionPath;
        private readonly string _codePath;
        private readonly string _thisDir;
        private readonly string _solutionAndProjectName = "EditorSolution";

        public string ExecutableFilePath => _copySolutionPath + @$"\{_solutionAndProjectName}.sln";

        public CSharpCodeProvider(string revitVersion)
        {
            _thisDir = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);
            _originalSolutionPath = _thisDir + @$"\{_solutionAndProjectName}";
            _copySolutionPath = _originalSolutionPath + "_";
            _codePath = _copySolutionPath + @"\Executor.cs";
            PatchProject(revitVersion);
            Refresh(false);
        }

        private void PatchProject(string revitVersion)
        {
            var revitPath = Registry.LocalMachine
                .OpenSubKey(@$"SOFTWARE\Autodesk\Revit\{revitVersion}\REVIT-05:0419")
                .GetValue("InstallationLocation");
            var projectPath = _originalSolutionPath + @$"\{_solutionAndProjectName}.csproj";
            using var reader = new StreamReader(projectPath);
            var rewritedProj = string.Format(reader.ReadToEnd(), revitPath);
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
