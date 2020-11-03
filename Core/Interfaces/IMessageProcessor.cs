using System.Threading.Tasks;
using Core;

namespace PrintMeAtServer
{
    public interface IMessageProcessor
    {
        Task Process(Message message);
    }
}