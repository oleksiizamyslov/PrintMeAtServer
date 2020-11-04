using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISerializer<T>
    {
        Task<string> Serialize(T obj);
        Task<T> Deserialize(string serialized);
    }
}