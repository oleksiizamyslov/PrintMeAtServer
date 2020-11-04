using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces
{
    public interface IMessageProcessor
    {
        Task Process(Message message);
    }
}