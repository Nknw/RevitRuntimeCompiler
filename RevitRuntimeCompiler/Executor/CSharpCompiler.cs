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
    public class CSharpCompiler : IDotNetCompiler
    {
        private readonly CodeDomProvider _compiler;
        private readonly Channel _channel;
        private readonly string _tempFolder;
        private readonly string[] _referenceAssemblyPaths;

        public CSharpCompiler(Channel channel)
        {
            var location = Assembly.GetAssembly(GetType()).Location;
            _tempFolder = Path.GetDirectoryName(location) + @"\temp";
            _channel = channel;
            _compiler = CodeDomProvider.CreateProvider(CodeDomProvider.GetLanguageFromExtension("cs"));
            _referenceAssemblyPaths = AppDomain.CurrentDomain.GetAssemblies()
                .Where(ShouldCreateReference)
                .Select(asm => asm.Location)
                .Append(location)
                .ToArray();
            TryRefreshTemp();
        }

        private bool ShouldCreateReference(Assembly assembly)
        {
            var fullName = assembly.FullName;
            var isSystem = fullName.StartsWith("System");
            var isCore = fullName.Contains("mscorlib");
            var isStandart = fullName.Contains("netstandard");
            var name = fullName.Split(',').First();
            var isRevitApi = name == "RevitAPI" || name == "RevitAPIUI";
            return isSystem || isCore || isStandart || isRevitApi;
        }

        private void TryRefreshTemp()
        {
            try
            {
                Directory.Delete(_tempFolder, true);
            }
            catch(Exception) { }
            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);
        }

        public async Task<Assembly> CompileAsync(string code)
        {
            var compileResult = await Task.Factory
                .StartNew(() => _compiler.CompileAssemblyFromSource(CreateParameters(), code));
            var errors = compileResult.Errors.Cast<CompilerError>();
            if (errors.Any())
            {
                foreach (var e in errors)
                    await _channel.WriteAsync(e.ErrorText);
                throw new CompileFailedException();
            }
            return compileResult.CompiledAssembly;
        }

        private CompilerParameters CreateParameters()
        {
            var parameters = new CompilerParameters(_referenceAssemblyPaths)
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
    }
}
