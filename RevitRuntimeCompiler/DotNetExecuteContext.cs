using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitRuntimeCompiler
{
    public class DotNetExecuteContext
    {
        public readonly string ClassName;
        public readonly string MethodName;
        public readonly string Language;

        public DotNetExecuteContext(string className, string methodName, string language)
        {
            ClassName = className;
            MethodName = methodName;
            Language = language;
        }
    }
}
