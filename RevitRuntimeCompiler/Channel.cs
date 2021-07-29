using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace RevitRuntimeCompiler
{
    public class Channel
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
        private readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();

        public async Task WriteAsync(string message)
        {
            _messages.Enqueue(message);
            _semaphore.Release();
            await Task.Yield();
        }
        
        public async Task<string> ReadAsync()
        {
            await _semaphore.WaitAsync();
            _messages.TryDequeue(out var message);
            return message;
        }
    }
}
