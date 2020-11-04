using System.Threading.Tasks;
using Core.Data;

namespace Core.Interfaces
{
    public interface ISerializer<T>
    {
        Task<string> Serialize(T obj);
        Task<T> Deserialize(string serialized);
    }
}