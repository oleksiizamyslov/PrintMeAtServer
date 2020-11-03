namespace Core
{
    public interface IMessageSerializer
    {
        string Serialize(Message message);
        Message Deserialize(string serialized);
    }
}