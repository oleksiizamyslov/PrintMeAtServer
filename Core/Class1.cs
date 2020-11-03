using System.Threading.Tasks;

namespace Core
{

    public interface IMessageProcessor
    {
        Task Process(Message message);
    }
}
