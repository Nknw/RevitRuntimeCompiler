using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.CodeDom.Compiler;

namespace RevitRuntimeCompiler.Executor
{
    public abstract class DotNetCompilerBase : IDotNetCompiler
    {
        protected static readonly string _location;
        protected static readonly string _tempFolder;
        private readonly Channel _channel;

        static DotNetCompilerBase()
        {
            _location = Assembly.GetAssembly(typeof(DotNetCompilerBase)).Location;
            _tempFolder = Path.GetDirectoryName(_location) + @"\temp";
            TryRefreshTemp();
        }

        public DotNetCompilerBase(Channel channel)
        {
            _channel = channel;
        }

        private static void TryRefreshTemp()
        {
            try
            {
                Directory.Delete(_tempFolder, true);
            }
            catch (Exception) { }
            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);
        }

        protected virtual CompilerParameters CreateParameters()
        {
            var parameters = new CompilerParameters(GetReferenceAssemblyPaths())
            {
                GenerateExecutable = false,
                IncludeDebugInformation = true
            };
            var folderName = Guid.NewGuid().ToString();
            var folderPath = Path.Combine(_tempFolder, folderName);
            Directory.CreateDirectory(folderPath);
            parameters.TempFiles = new TempFileCollection(folderPath, false);
            return parameters;
        }

        public async Task<Assembly> CompileAsync(string code)
        {
            var compileResult = await Task.Factory
                .StartNew(() =>
                {
                    var provider = GetProvider();
                    var parameters = CreateParameters();
                    return provider.CompileAssemblyFromSource(parameters, code);
                });

            var errors = compileResult.Errors.Cast<CompilerError>();
            if (errors.Any())
            {
                foreach (var e in errors)
                    await _channel.WriteAsync(e.ErrorText);
                throw new CompileFailedException();
            }
            return compileResult.CompiledAssembly;
        }

        protected abstract CodeDomProvider GetProvider();
        protected abstract string[] GetReferenceAssemblyPaths();
    }
}
