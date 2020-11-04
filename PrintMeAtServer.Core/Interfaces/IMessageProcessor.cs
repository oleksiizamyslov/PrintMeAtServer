using System.Threading.Tasks;
using PrintMeAtServer.Core.Models;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface IMessageProcessor
    {
        Task Process(Message message);
    }
}