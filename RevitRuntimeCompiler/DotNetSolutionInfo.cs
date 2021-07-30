using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitRuntimeCompiler
{
    public class DotNetSolutionInfo
    {
        public readonly string SolutionName;
        public readonly string ProjectFileName;
        public readonly string CodeFileName;

        public DotNetSolutionInfo(string solutionName, string projectFileName, string codeFileName)
        {
            SolutionName = solutionName;
            ProjectFileName = projectFileName;
            CodeFileName = codeFileName;
        }
    }
}
