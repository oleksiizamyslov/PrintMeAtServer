using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces
{
    public interface IMessageQueue
    {
        Task<Message> PeekNextScheduledMessage();
        Task<Message> DequeueNextScheduledMessage();
        Task EnqueueMessage(Message message);
    }
}