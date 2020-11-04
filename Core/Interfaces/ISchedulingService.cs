using System;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISchedulingService
    {
        Task ScheduleProcessing(DateTimeOffset dateTime);
    }
}