using System.Threading.Tasks;
using Core.DataStructures;

namespace Core
{
    public class PersistentMessageQueue : IPersistentMessageQueue
    {
        private readonly IMessageRepository _messageRepository;

        private readonly Heap<Message> _sortedMessages = new Heap<Message>(10);

        private readonly object _lock = new object();

        public PersistentMessageQueue(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        
        public async Task Initialize()
        {
            var messages = await _messageRepository.GetAllMessages();
            foreach (var message in messages)
            {
                _sortedMessages.Insert(message);
            }
        }

        public async Task EnqueueMessage(Message newMessage)
        {
            await _messageRepository.AddMessage(newMessage);
            lock (_lock)
            {
                _sortedMessages.Insert(newMessage);
            }
        }

        public Task<Message> PeekNextScheduledMessage()
        {
            lock (_lock)
            {
                return Task.FromResult(_sortedMessages.Peek());
            }
        }

        public async Task<Message> PopNextScheduledMessage()
        {
            Message message;
            lock (_lock)
            {
                message = _sortedMessages.Remove();
            }

            if (message != null)
            {
                await _messageRepository.RemoveMessage(message);
            }

            return message;
        }
    }
}