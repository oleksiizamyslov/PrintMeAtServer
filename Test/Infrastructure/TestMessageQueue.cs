using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Data;
using Core.Interfaces;

namespace Test.Infrastructure
{
    public class TestMessageQueue : IMessageQueue
    {
        private readonly Heap<Message> _sortedMessages = new Heap<Message>
            (Comparer<Message>.Create((m1, m2) => m1.DateTime.CompareTo(m2.DateTime)), 10);

        private readonly object _lock = new object();
        public Task EnqueueMessage(Message newMessage)
        {
            lock (_lock)
            {
                _sortedMessages.Insert(newMessage);
            }
            return Task.CompletedTask;
        }

        public Task<Message> PeekNextScheduledMessage()
        {
            lock (_lock)
            {
                return Task.FromResult(_sortedMessages.Peek());
            }
        }

        public Task<Message> DequeueNextScheduledMessage()
        {
            Message message;
            lock (_lock)
            {
                message = _sortedMessages.Remove();
            }

            return Task.FromResult(message);
        }
    }
}