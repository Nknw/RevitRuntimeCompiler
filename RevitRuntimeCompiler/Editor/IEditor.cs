using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitRuntimeCompiler.Editor
{
    public interface IEditor : IDisposable
    {
        string EditorName { get; }
        void Edit();
        string GetCode();
        void Refresh();
    }
}
