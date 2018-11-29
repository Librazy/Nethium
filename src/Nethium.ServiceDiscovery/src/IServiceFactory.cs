namespace Nethium.ServiceDiscovery
{
    public interface IServiceFactory<TInterface> where TInterface : class
    {
        TInterface Build(IServiceLocation<TInterface> location);
    }
}