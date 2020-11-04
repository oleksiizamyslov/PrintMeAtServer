using System.Threading.Tasks;
using Core.Data;

namespace Core.Interfaces
{
    public interface IMessageProcessor
    {
        Task Process(Message message);
    }
}