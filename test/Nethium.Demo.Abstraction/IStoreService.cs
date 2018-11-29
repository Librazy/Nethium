using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nethium.Demo.Abstraction
{
    public interface IStoreService
    {
        Task<IDictionary<string, string>> AllAsync(CancellationToken cancellationToken = default);

        Task<string> GetAsync(string id, CancellationToken cancellationToken = default);

        Task<string> SetAsync(string id, string value, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}