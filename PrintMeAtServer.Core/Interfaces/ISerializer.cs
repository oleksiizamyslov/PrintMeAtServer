using System.Threading.Tasks;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface ISerializer<T>
    {
        Task<string> Serialize(T obj);
        Task<T> Deserialize(string serialized);
    }
}