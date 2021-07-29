using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitRuntimeCompiler.Executor
{
    public interface IExecutor
    {
        string Language { get; }
        Task ExecuteAsync(string code);
    }
}
