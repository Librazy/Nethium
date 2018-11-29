using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethium.Demo.Abstraction;

namespace Nethium.Demo.Web
{
    public class ViewController : Controller
    {
        private readonly IAggregateService _aggregateService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ViewController> _logger;

        public ViewController(IAggregateService aggregateService, IConfiguration configuration, ILogger<ViewController> logger)
        {
            _aggregateService = aggregateService;
            _configuration = configuration;
            _logger = logger;
        }

        [Route("View")]
        [Route("View/Index")]
        public async Task<IActionResult> Index([FromQuery] int a = 2, [FromQuery] int b = 3)
        {
            await _aggregateService.SetBaseAsync(a);
            var aggregateResult = await _aggregateService.AggregateAsync(b);
            ViewData["addFormat"] = _configuration["addFormat"] ?? "{0} + {1} = {2}";
            ViewData["mulFormat"] = _configuration["mulFormat"] ?? "{0} * {1} = {2}";
            _logger.LogError(_configuration["addFormat"]);
            _logger.LogError(_configuration["mulFormat"]);
            return View(aggregateResult);
        }
    }
}