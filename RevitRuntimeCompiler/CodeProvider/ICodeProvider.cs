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
        string Location { get; }
        string GetCode();
        void Refresh();
    }
}
