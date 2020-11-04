using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;

namespace Test.Infrastructure
{
    public class InMemoryMessageQueue : IMessageQueue
    {
        private readonly Heap<Message> _messageHeap = new Heap<Message>
            (Comparer<Message>.Create((m1, m2) => m1.DateTime.CompareTo(m2.DateTime)), 10);

        private readonly object _lock = new object();

        public Task EnqueueMessage(Message newMessage)
        {
            lock (_lock)
            {
                _messageHeap.Insert(newMessage);
            }
            return Task.CompletedTask;
        }

        public Task<Message> PeekNextScheduledMessage()
        {
            lock (_lock)
            {
                return Task.FromResult(_messageHeap.Peek());
            }
        }

        public Task<Message> DequeueNextScheduledMessage()
        {
            Message message;
            lock (_lock)
            {
                message = _messageHeap.Remove();
            }

            return Task.FromResult(message);
        }
    }
}