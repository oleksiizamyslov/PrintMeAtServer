using System.Threading.Tasks;
using Core.Data;

namespace Core.Interfaces
{
    public interface IMessageQueue
    {
        Task<Message> PeekNextScheduledMessage();
        Task<Message> DequeueNextScheduledMessage();
        Task EnqueueMessage(Message message);
    }
}