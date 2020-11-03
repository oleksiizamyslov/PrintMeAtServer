namespace Core
{
    /// <summary>
    /// All global app settings related to core functionality go here.
    /// </summary>
    public interface IServerConfiguration
    {
        string ServerName { get; }
    }
}