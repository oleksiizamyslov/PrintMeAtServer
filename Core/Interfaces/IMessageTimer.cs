using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPrintMeAtService
    {
        Task EnqueueMessage(Message message);

    }
}