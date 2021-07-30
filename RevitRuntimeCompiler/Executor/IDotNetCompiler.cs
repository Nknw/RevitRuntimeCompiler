using System.Reflection;
using System.Threading.Tasks;

namespace RevitRuntimeCompiler.Executor
{
    public interface IDotNetCompiler
    {
        Task<Assembly> CompileAsync(string code);
    }
}