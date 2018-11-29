using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethium.Demo.Abstraction;
using SharpRomans;

namespace Nethium.Demo.Service.ToRoman
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToRomanController : ControllerBase, IToRomanService
    {
        [HttpGet("{num}")]
        public async Task<string> ToRomanAsync(ushort num, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken);
            return new RomanNumeral(num).ToString();
        }

        [HttpGet("health")]
        public ActionResult HealthAsync() => NoContent();
    }
}