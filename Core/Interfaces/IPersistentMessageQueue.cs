using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Core
{
    public interface IPersistentMessageQueue
    {
        Task Initialize();

        Task<Message> PeekNextScheduledMessage();
        Task<Message> PopNextScheduledMessage();
        Task EnqueueMessage(Message message);
    }
}