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
            var version = commandData.Application.Application.VersionNumber;
            var channel = new Channel();
            var ctx = new DotNetExecuteContext("Executor", "Execute", "C#");
            var info = new DotNetSolutionInfo("EditorSolution", "EditorSolution.csproj", "Executor.cs");
            _window = new RunnerWindow
            {
                DataContext = new RunnerViewModel(
                        _editors,
                        new (IExecutor, ICodeProvider)[]
                        {
                            (new DotNetExecutor(channel, new CSharpCompiler(channel), ctx), new SolutionCodeProvider(version, info))
                        },
                        channel
                   )
            };
            _window.Show();
            _window.Closed += (s, e) =>
            {
                _window = null;
                _editors.ForEach(e => e.Close());
                channel.Close();
            };
            return Result.Succeeded;
        }
    }
}
