using System.Threading;
using System.Threading.Tasks;

namespace Nethium.Demo.Abstraction
{
    public interface ICalcService
    {
        Task<int> AddAsync(int a, int b, CancellationToken cancellationToken = default);

        Task<int> MulAsync(int a, int b, CancellationToken cancellationToken = default);

    }
}