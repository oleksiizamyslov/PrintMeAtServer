using System.Threading.Tasks;
using PrintMeAtServer.Core.Models;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface IPrintMeAtService
    {
        Task EnqueueMessage(Message message);
    }
}