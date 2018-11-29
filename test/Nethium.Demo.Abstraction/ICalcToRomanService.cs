using System.Threading;
using System.Threading.Tasks;

namespace Nethium.Demo.Abstraction
{
    public interface ICalcToRomanService
    {
        Task<string> AddAsync(int a, int b, CancellationToken cancellationToken = default);

        Task<string> MulAsync(int a, int b, CancellationToken cancellationToken = default);
    }
}