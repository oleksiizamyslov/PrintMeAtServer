using System;
using System.Threading.Tasks;
using Core;

namespace PrintMeAtServer
{
    public interface IScheduledMessageService : IDisposable
    {
        Task EnqueueMessage(Message message);
        Task Initialize();
    }
}