using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethium.Demo.Abstraction;

namespace Nethium.Demo.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase, IStoreService
    {
        private static readonly Dictionary<string, string> _Store = new Dictionary<string, string>();

        // GET api/store
        [HttpGet]
        public Task<IDictionary<string, string>> AllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult((IDictionary<string, string>) _Store);

        // GET api/store/5
        [HttpGet("{id}")]
        public async Task<string> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken);
            return _Store[id];
        }

        // PUT api/store/5
        [HttpPut("{id}")]
        public Task<string> SetAsync(string id, [FromBody] string value, CancellationToken cancellationToken = default)
        {
            _Store[id] = value;
            return Task.FromResult(_Store[id]);
        }

        // DELETE api/store/5
        [HttpDelete("{id}")]
        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            _Store.Remove(id);
            return Task.CompletedTask;
        }

        [HttpGet("health")]
        public ActionResult HealthAsync() => NoContent();
    }
}