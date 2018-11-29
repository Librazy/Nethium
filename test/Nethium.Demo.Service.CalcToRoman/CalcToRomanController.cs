using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethium.Demo.Abstraction;

namespace Nethium.Demo.Service.CalcToRoman
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalcToRomanController : ControllerBase, ICalcToRomanService
    {
        private readonly ICalcService _calcService;
        private readonly IToRomanService _toRomanService;

        public CalcToRomanController(ICalcService calcService, IToRomanService toRomanService)
        {
            _calcService = calcService;
            _toRomanService = toRomanService;
        }

        [HttpGet("add/{a}/{b}")]
        public async Task<string> AddAsync(int a, int b, CancellationToken cancellationToken = default)
        {
            var r = await _calcService.AddAsync(a, b, cancellationToken);
            return Math.Abs(r) != r ? "-" : "" + await _toRomanService.ToRomanAsync((ushort)Math.Abs(r), cancellationToken);
        }

        [HttpGet("mul/{a}/{b}")]
        public async Task<string> MulAsync(int a, int b, CancellationToken cancellationToken = default)
        {
            var r = await _calcService.MulAsync(a, b, cancellationToken);
            return Math.Abs(r) != r ? "-" : "" + await _toRomanService.ToRomanAsync((ushort)Math.Abs(r), cancellationToken);
        }

        [HttpGet("health")]
        public ActionResult HealthAsync() => NoContent();
    }
}