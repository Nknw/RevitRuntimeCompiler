using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevitRuntimeCompiler.Executor
{
    public class FSharpCompiler : DotNetCompilerBase
    {
        private readonly CodeDomProvider _provider;
        private readonly string[] _referenceAssemblyPaths;

        public FSharpCompiler(IEnumerable<string> coreAssemblyPaths, Channel channel) : base(channel)
        {
            var core = Assembly.LoadFrom(@"C:\Users\Ivan\source\repos\RevitRuntimeCompiler\RevitRuntimeCompiler\bin\x64\Debug\FSharp.Core.dll");
            var resources = Assembly.LoadFrom(@"C:\Users\Ivan\source\repos\RevitRuntimeCompiler\RevitRuntimeCompiler\bin\x64\Debug\ru\FSharp.Core.resources.dll");
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                if (e.Name.Contains("FSharp"))
                {
                    if (e.Name.Contains("resources"))
                        return resources;
                    return core;
                }
                return null;
            };
            _provider = new FSharpCleanCodeProvider();
            _referenceAssemblyPaths = coreAssemblyPaths
                .Concat(new[] { core.Location, resources.Location })
                .ToArray();
        }

        protected override CodeDomProvider GetProvider() => _provider;

        protected override string[] GetReferenceAssemblyPaths() => _referenceAssemblyPaths;
    }
}
