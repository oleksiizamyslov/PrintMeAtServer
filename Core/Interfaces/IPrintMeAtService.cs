using System.Threading.Tasks;
using Core.Data;

namespace Core.Interfaces
{
    public interface IPrintMeAtService
    {
        Task EnqueueMessage(Message message);
    }
}