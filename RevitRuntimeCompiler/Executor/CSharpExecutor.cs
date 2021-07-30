﻿using System;
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
    public class CSharpExecutor : IExecutor
    {
        private readonly ICSharpCompiler _compiler;
        private readonly Channel _channel;
        private readonly Dictionary<Type, Func<UIApplication, object>> _executeFuncs = new Dictionary<Type, Func<UIApplication, object>>();

        public string Language => "C#";

        public CSharpExecutor(Channel channel, ICSharpCompiler compiler)
        {
            _compiler = compiler;
            _channel = channel;
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
        }

        private async Task ExecuteAsync(Assembly compiledAssembly)
        {
            var method = compiledAssembly.GetType("Executor")
                .GetMethod("Execute");
            var arguement = method.GetParameters()
                .First()
                .ParameterType;
            var parameterHandler = _executeFuncs[arguement];
            await RevitTask.RunAsync(async uiApp =>
            {
                try
                {
                    await (Task)method.Invoke(null, new[] { parameterHandler(uiApp), _channel });
                }
                catch (Exception ex)
                {
                    await _channel.WriteAsync(ex.GetType().FullName);
                    await _channel.WriteAsync(ex.Message);
                    await _channel.WriteAsync(ex.StackTrace);
                }
            });
        }
    }
}
