using System.Threading.Tasks;
using Core.Data;

namespace Core.Interfaces
{
    public interface IMessageSerializer
    {
        Task<string> Serialize(Message message);
        Task<Message> Deserialize(string serialized);
    }
}