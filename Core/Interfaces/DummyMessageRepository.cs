using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{
    public class DummyMessageRepository : IMessageRepository
    {
        private static readonly HashSet<Message> _hs = new HashSet<Message>();

        public Task<Message[]> GetAllMessages()
        {
            return Task.FromResult(_hs.ToArray());
        }

        public Task AddMessage(Message message)
        {
            _hs.Add(message);
            return Task.CompletedTask;
        }

        public Task RemoveMessage(Message message)
        {
            _hs.Remove(message);
            return Task.CompletedTask;
        }
    }
}