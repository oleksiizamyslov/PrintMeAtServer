using System;
using System.Threading.Tasks;
using Core;

namespace PrintMeAtServer
{
    public class PrintMeService : IPrintMeService
    {
        private readonly IPersistentMessageQueue _messageQueue;
        private readonly IScheduledMessageService _scheduledMessageService;

        public PrintMeService(IPersistentMessageQueue messageQueue, IScheduledMessageService scheduledMessageService, IMessageProcessor messageProcessor)
        {
            _messageQueue = messageQueue;
            _scheduledMessageService = scheduledMessageService;
        }

        public async Task Initialize()
        {
            _scheduledMessageService.Initialize();
            await _messageQueue.Initialize();
        }

        public async Task AddMessage(Message newMessage)
        {
            await _scheduledMessageService.EnqueueMessage(newMessage);
        }
    }
}