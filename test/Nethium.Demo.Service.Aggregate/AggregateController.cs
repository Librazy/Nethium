using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethium.Demo.Abstraction;

namespace Nethium.Demo.Service.Aggregate
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregateController : ControllerBase, IAggregateService
    {
        private readonly ICalcToRomanService _calcToRomanService;
        private readonly IStoreService _storeService;

        public AggregateController(ICalcToRomanService calcToRomanService, IStoreService storeService)
        {
            _calcToRomanService = calcToRomanService;
            _storeService = storeService;
        }

        [HttpPost("{num}")]
        public async Task SetBaseAsync(int num, CancellationToken cancellationToken = default)
        {
            await _storeService.SetAsync("base", num.ToString(), cancellationToken);
        }

        [HttpGet("{num}")]
        public async Task<AggregateResult> AggregateAsync(int num, CancellationToken cancellationToken = default)
        {
            var baseNum = Convert.ToInt32(await _storeService.GetAsync("base", cancellationToken) ?? "2");
            var addResult = _calcToRomanService.AddAsync(baseNum, num, cancellationToken);
            var mulResult = _calcToRomanService.MulAsync(baseNum, num, cancellationToken);
            return new AggregateResult(baseNum, await addResult, await mulResult);
        }

        [HttpGet("health")]
        public ActionResult HealthAsync() => NoContent();
    }
}