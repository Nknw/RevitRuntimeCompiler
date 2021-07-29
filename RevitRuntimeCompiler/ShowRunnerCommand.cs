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

namespace RevitRuntimeCompiler
{
    [Transaction(TransactionMode.Manual)]
    public class ShowRunnerCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitTask.Initialize();
            var version = commandData.Application.Application.VersionNumber;
            var window = new RunnerWindow
            {
                DataContext = new RunnerViewModel()
            };
            window.Show();
            return Result.Succeeded;
        }
    }
}
