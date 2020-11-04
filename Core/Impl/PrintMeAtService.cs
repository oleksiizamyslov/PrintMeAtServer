using System.Threading.Tasks;
using Core.Data;
using Core.Interfaces;

namespace Core.Impl
{
    public class PrintMeAtService : IPrintMeAtService
    {
        private readonly IMessageQueue _messageQueue;
        private readonly ISchedulingService _schedulingService;
        
        public PrintMeAtService(IMessageQueue messageQueue, ISchedulingService schedulingService)
        {
            _messageQueue = messageQueue;
            _schedulingService = schedulingService;
        }

        public async Task EnqueueMessage(Message message)
        {
            await _messageQueue.EnqueueMessage(message);
            await _schedulingService.ScheduleProcessing(message.DateTime);
        }
    }
}