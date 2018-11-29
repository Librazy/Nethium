using System.Threading;
using System.Threading.Tasks;

namespace Nethium.Demo.Abstraction
{
    public interface IToRomanService
    {
        Task<string> ToRomanAsync(ushort num, CancellationToken cancellationToken = default);
    }
}