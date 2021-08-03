using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Reflection;
using Revit.Async;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitRuntimeCompiler.Executor
{
    public class DotNetExecutor : IExecutor
    {
        private readonly IDotNetCompiler _compiler;
        private readonly Channel _channel;
        private readonly Dictionary<Type, Func<UIApplication, object>> _executeFuncs = new Dictionary<Type, Func<UIApplication, object>>();
        private readonly DotNetExecuteContext _context;

        public string Language => _context.Language;

        public DotNetExecutor(Channel channel, IDotNetCompiler compiler, DotNetExecuteContext context)
        {
            _compiler = compiler;
            _channel = channel;
            _context = context;
            _executeFuncs[typeof(UIApplication)] = uiApp => uiApp;
            _executeFuncs[typeof(Document)] = uiApp => uiApp?.ActiveUIDocument?.Document;
        }

        public async Task ExecuteAsync(string code)
        {
            try
            {
                var compiledAssembly = await _compiler.CompileAsync(code);
                await ExecuteAsync(compiledAssembly);
            }
            catch (CompileFailedException) { }
            catch (Exception ex)
            {
                await LogExceptionAsync(ex);
            }
        }

        private async Task ExecuteAsync(Assembly compiledAssembly)
        {
            var method = compiledAssembly.GetType(_context.ClassName)
                .GetMethod(_context.MethodName);
            var arguement = method.GetParameters()
                .First()
                .ParameterType;
            var parameterHandler = _executeFuncs[arguement];
            await RevitTask.RunAsync(async uiApp =>
            {
                var doc = uiApp?.ActiveUIDocument?.Document;
                var parameter = parameterHandler(uiApp);
                if (doc == null)
                {
                    await Invoke(method, parameter);
                    return;
                }
                using var transaction = new Transaction(doc, "Runtime compiled assembly code");
                transaction.Start();
                await Invoke(method, parameter);
                transaction.Commit();
            });
        }

        private async Task Invoke(MethodInfo method, object parameter)
        {
            await (Task)method.Invoke(null, new[] { parameter, _channel });
        }
        
        private async Task LogExceptionAsync(Exception ex)
        {
            await _channel.WriteAsync(ex.GetType().FullName);
            await _channel.WriteAsync(ex.Message);
            await _channel.WriteAsync(ex.StackTrace);
            if (ex.InnerException != null)
            {
                await _channel.WriteAsync("\r\n\r\nInnerException:\r\n");
                await LogExceptionAsync(ex.InnerException);
            }
        }
    }
}
