using JobBoard.Api.Application.Interfaces;
using System.Threading.Channels;

namespace JobBoard.Api.Infrastructure.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

        public BackgroundTaskQueue()
        {
            _queue = Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>();
        }

        public ValueTask QueueAsync(Func<CancellationToken, ValueTask> workItem)
        {
            return _queue.Writer.WriteAsync(workItem);
        }

        public ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
