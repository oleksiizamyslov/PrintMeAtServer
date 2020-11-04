using System.Threading.Tasks;
using PrintMeAtServer.Core.Models;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface IMessageQueue
    {
        Task<Message> PeekNextScheduledMessage();
        Task<Message> DequeueNextScheduledMessage();
        Task EnqueueMessage(Message message);
    }
}