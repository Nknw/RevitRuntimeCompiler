using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace RevitRuntimeCompiler
{
    public class Channel<TMessage>
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
        private readonly ConcurrentQueue<TMessage> _messages = new ConcurrentQueue<TMessage>();

        public void Write(TMessage message)
        {
            _messages.Enqueue(message);
            _semaphore.Release();
        }
        
        public async Task<TMessage> ReadAsync()
        {
            await _semaphore.WaitAsync();
            _messages.TryDequeue(out var message);
            return message;
        }
    }
}
