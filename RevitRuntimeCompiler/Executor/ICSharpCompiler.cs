using System.Reflection;
using System.Threading.Tasks;

namespace RevitRuntimeCompiler.Executor
{
    public interface ICSharpCompiler
    {
        Task<Assembly> CompileAsync(string code);
    }
}