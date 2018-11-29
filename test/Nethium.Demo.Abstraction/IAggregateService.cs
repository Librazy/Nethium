using System.Threading;
using System.Threading.Tasks;

namespace Nethium.Demo.Abstraction
{
    public interface IAggregateService
    {
        Task<AggregateResult> AggregateAsync(int num, CancellationToken cancellationToken = default);

        Task SetBaseAsync(int num, CancellationToken cancellationToken = default);
    }
}