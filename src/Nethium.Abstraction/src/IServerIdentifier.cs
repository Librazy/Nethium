namespace Nethium.Abstraction
{
    public interface IServerIdentifier
    {
        string ServerId { get; }

        string BaseUrl { get; }

        int? Port { get; }
    }
}