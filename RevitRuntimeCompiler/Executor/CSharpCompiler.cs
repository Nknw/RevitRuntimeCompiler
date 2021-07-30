using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;

namespace RevitRuntimeCompiler.Executor
{
    public class CSharpCompiler : DotNetCompilerBase
    {
        private readonly CodeDomProvider _provider;
        private readonly string[] _referenceAssemblyPaths;

        public CSharpCompiler(IEnumerable<string> coreAssemblyPaths, Channel channel) : base(channel)
        {
            _provider = CodeDomProvider.CreateProvider(CodeDomProvider.GetLanguageFromExtension("cs"));
            _referenceAssemblyPaths = coreAssemblyPaths.ToArray();
        }

        protected override CodeDomProvider GetProvider() => _provider;

        protected override string[] GetReferenceAssemblyPaths() => _referenceAssemblyPaths;
    }
}
