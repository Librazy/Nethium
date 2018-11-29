namespace Nethium.Abstraction
{
    public class ServerIdentifier : IServerIdentifier
    {
        public ServerIdentifier()
        {
        }

        public ServerIdentifier(string serverId, string baseUrl)
        {
            ServerId = serverId;
            BaseUrl = baseUrl;
        }

        public string ServerId { get; set; } = "";

        public string BaseUrl { get; set; } = "";

        public int? Port { get; set; }
    }
}