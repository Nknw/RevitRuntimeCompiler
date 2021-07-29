using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom.Compiler;

namespace RevitRuntimeCompiler.Executor
{
    public class CSharpCompiler : ICSharpCompiler
    {
        private readonly CodeDomProvider _compiler;
        private readonly CompilerParameters _parameters;
        private readonly Channel _channel;

        public CSharpCompiler(Channel channel)
        {
            _channel = channel;
            _compiler = CodeDomProvider.CreateProvider(CodeDomProvider.GetLanguageFromExtension("cs"));
            var referenceAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(ShouldCreateReference)
                .Select(asm => asm.Location)
                .Append(Assembly.GetAssembly(GetType()).Location)
                .ToArray();
            _parameters = new CompilerParameters(referenceAssemblies)
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
        }

        private bool ShouldCreateReference(Assembly assembly)
        {
            var fullName = assembly.FullName;
            var isSystem = fullName.StartsWith("System");
            var isCore = fullName.Contains("nscorelib");
            var name = fullName.Split(',').First();
            var isRevitApi = name == "RevitAPI" || name == "RevitAPIUI";
            return isSystem || isCore || isRevitApi;
        }

        public async Task<Assembly> CompileAsync(string code)
        {
            var compileResult = await Task.Factory
                .StartNew(() => _compiler.CompileAssemblyFromSource(_parameters, code));
            var errors = compileResult.Errors.Cast<CompilerError>();
            if (errors.Any())
            {
                foreach (var e in errors)
                    await _channel.WriteAsync(e.ErrorText);
                throw new Exception();
            }
            return compileResult.CompiledAssembly;
        }
    }
}
