using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;
using Autodesk.Revit.Attributes;
using RevitRuntimeCompiler.UI;
using RevitRuntimeCompiler.Editor;
using RevitRuntimeCompiler.Executor;
using RevitRuntimeCompiler.CodeProvider;
using System.Reflection;

namespace RevitRuntimeCompiler
{
    [Transaction(TransactionMode.Manual)]
    public class ShowRunnerCommand : IExternalCommand
    {
        private static RunnerWindow _window;
        private readonly static List<IEditor> _editors = new List<IEditor>()
        {
            new VSEditor()
        };
        private static Channel _channel = new Channel();

        static ShowRunnerCommand()
        {
            RevitTask.Initialize();
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (_window != null)
            {
                _window.ShowWindow();
                return Result.Succeeded;
            }
            CreateWindow(commandData.Application);
            _window.Show();
            _window.Closed += (s, e) =>
            {
                _window = null;
                _editors.ForEach(e => e.Close());
                _channel.Close();
            };
            return Result.Succeeded;
        }

        private void CreateWindow(UIApplication app)
        {
            _channel = new Channel();
            var version = app.Application.VersionNumber;
            var csCtx = new DotNetExecuteContext("Executor", "Execute", "C#");
            var csInfo = new DotNetSolutionInfo("EditorSolution", "EditorSolution.csproj", "Executor.cs");
            var fsCtx = new DotNetExecuteContext("Executor", "Execute", "F#");
            var fsInfo = new DotNetSolutionInfo("FSharpSolution", "FSharpSolution.fsproj", "Executor.fs");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(IsCoreAssembly)
                .Select(asm => asm.Location)
                .ToArray();
            _window = new RunnerWindow
            {
                DataContext = new RunnerViewModel(
                        _editors,
                        new (IExecutor, ICodeProvider)[]
                        {
                            (new DotNetExecutor(_channel, new CSharpCompiler(assemblies, _channel), csCtx), new SolutionCodeProvider(version, csInfo)),
                            (new DotNetExecutor(_channel, new FSharpCompiler(assemblies, _channel), fsCtx), new SolutionCodeProvider(version, fsInfo))
                        },
                        _channel
                   )
            };
        }

        private bool IsCoreAssembly(Assembly assembly)
        {
            var fullName = assembly.FullName;
            var isSystem = fullName.StartsWith("System");
            var isCore = fullName.Contains("mscorlib");
            var isStandart = fullName.Contains("netstandard");
            var name = fullName.Split(',').First();
            var isRevitApi = name == "RevitAPI" || name == "RevitAPIUI";
            var isThisAsm = name.StartsWith("RevitRuntimeCompiler");
            return isSystem || isCore || isStandart || isRevitApi || isThisAsm;
        }
    }
}
