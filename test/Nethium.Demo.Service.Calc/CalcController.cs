using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethium.Demo.Abstraction;

namespace Nethium.Demo.Service.Calc
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalcController : ControllerBase, ICalcService
    {
        [HttpGet("add/{a}/{b}")]
        public async Task<int> AddAsync(int a, int b, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken);
            return a + b;
        }

        [HttpGet("mul/{a}/{b}")]
        public async Task<int> MulAsync(int a, int b, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken);
            return a * b;
        }

        [HttpGet("health")]
        public ActionResult HealthAsync() => NoContent();
    }
}