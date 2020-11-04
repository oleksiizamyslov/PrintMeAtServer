using System;
using System.Threading.Tasks;

namespace Core.Data
{
    public interface ISchedulingService
    {
        Task ScheduleProcessing(DateTimeOffset dateTime);
    }
}