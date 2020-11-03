using System.Threading.Tasks;

namespace Core
{
    public interface IMessageQueue
    {
        Task<Message> PeekNextScheduledMessage();
        Task<Message> DequeueNextScheduledMessage();
        Task EnqueueMessage(Message message);
    }
}