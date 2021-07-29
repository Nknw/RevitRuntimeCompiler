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
        private static RunnerWindow window;

        static ShowRunnerCommand()
        {
            RevitTask.Initialize();
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (window?.IsActive ?? false)
                return Result.Succeeded;
            var version = commandData.Application.Application.VersionNumber;
            window = new RunnerWindow
            {
                DataContext = new RunnerViewModel(
                        new[] { new VSEditor() },
                        channel => new (IExecutor,ICodeProvider)[]
                        {
                            (new CSharpExecutor(channel, new CSharpCompiler(channel)), new CSharpCodeProvider(version))
                        }
                    )
            };
            window.Show();
            return Result.Succeeded;
        }
    }
}
