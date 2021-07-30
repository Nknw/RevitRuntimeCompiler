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
        private readonly CancellationTokenSource _source = new CancellationTokenSource();
        private bool _closed;

        public async Task WriteAsync(string message)
        {
            _messages.Enqueue(message);
            _semaphore.Release();
            await Task.Yield();
        }
        
        public async Task<string> ReadAsync()
        {
            if (_closed)
                throw new ChannelClosedException();
            try
            {
                await _semaphore.WaitAsync(_source.Token);
            }
            catch (AggregateException ae)
            {
                var exs = ae.InnerExceptions;
                if (exs.Count == 1 && exs.First() is TaskCanceledException)
                    throw new ChannelClosedException();
                throw;
            }
            _messages.TryDequeue(out var message);
            return message;
        }

        public void Close()
        {
            _closed = true;
            _source.Cancel();
        }
    }
}
