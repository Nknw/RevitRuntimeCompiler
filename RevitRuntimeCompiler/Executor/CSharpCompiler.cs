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
        private readonly Channel<string> _channel;

        public CSharpCompiler(Channel<string> channel)
        {
            _channel = channel;
            _compiler = CodeDomProvider.CreateProvider(CodeDomProvider.GetLanguageFromExtension("cs"));
            var apiPath = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => asm.FullName.StartsWith("System") || asm.FullName.Contains("mscorelib") || asm.FullName.Split().First() == "RevitAPI")
                .Select(asm => asm.Location)
                .Append(Assembly.GetAssembly(GetType()).Location)
                .ToArray();
            _parameters = new CompilerParameters(apiPath)
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
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
