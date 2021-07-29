using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitRuntimeCompiler.Editor;

namespace RevitRuntimeCompiler.CodeProvider
{
    public interface ICodeProvider
    {
        string ExecutableFilePath { get; }
        string GetCode();
        void Refresh();
    }
}
