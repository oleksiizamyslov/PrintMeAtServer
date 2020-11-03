using System.Threading.Tasks;

namespace Core
{
    public interface IMessageRepository
    {
        Task<Message[]> GetAllMessages();
        Task AddMessage(Message message);
        Task RemoveMessage(Message message);
    }
}
