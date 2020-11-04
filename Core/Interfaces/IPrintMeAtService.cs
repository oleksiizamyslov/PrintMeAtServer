using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces
{
    public interface IPrintMeAtService
    {
        Task EnqueueMessage(Message message);
    }
}