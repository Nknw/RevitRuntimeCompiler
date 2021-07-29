using RevitRuntimeCompiler.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace RevitRuntimeCompiler.CodeProvider
{
    public class SolutionCodeProvider : ICodeProvider
    {
        private readonly string _originalSolutionPath;
        private readonly string _copySolutionPath;
        private readonly string _codePath;
        private readonly string _thisDir;

        public string Location => _copySolutionPath + @"\EditorSolution.sln";

        public SolutionCodeProvider()
        {
            _thisDir = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);
            _originalSolutionPath = _thisDir + @"\EditorSolution";
            _copySolutionPath = _originalSolutionPath + "_";
            _codePath = _copySolutionPath + @"\Executor.cs";
            Refresh();
        }

        public string GetCode()
        {
            using var reader = new StreamReader(_codePath);
            return reader.ReadToEnd();
        }

        public void Refresh()
        {
            if (Directory.Exists(_copySolutionPath))
                Directory.Delete(_copySolutionPath, true);
            var target = Directory.CreateDirectory(_copySolutionPath);
            new DirectoryInfo(_originalSolutionPath).CopyFilesRecursively(target);
        }

        public void ConfigureEditor(IEditor editor)
        {
            throw new NotImplementedException();
        }
    }
}
