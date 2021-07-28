﻿using NUnit.Framework;
using System.Threading.Tasks;
using RevitRuntimeCompiler.Executor;

namespace RevitRuntimeCompiler.tests
{
    public class RuntimeTests
    {
        [Test]
        public async Task ShouldCompile()
        {
            var channel = new Channel<string>();
            var runner = new CSharpExecutor(channel, new CSharpCompiler(channel));
            var writeToChannel = "test";

            await runner.ExecuteAsync(
                "using System.Linq;" +
                "using RevitRuntimeCompiler;" +
                "" +
                "public static class Executor" +
                "{" +
                "   public static int Execute(Channel<string> channel)" +
                "       {" +
                $"           channel.Write(\"{writeToChannel}\");" +
                "           return 3;" +
                "       }" +
                "};"
                );
            var result = await channel.ReadAsync();

            Assert.That(result, Is.EqualTo(result));
        }
    }
}
