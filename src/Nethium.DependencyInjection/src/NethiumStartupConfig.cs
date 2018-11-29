using System.Collections.Generic;
using System.Threading;
using Nethium.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public class NethiumStartupConfig
    {
        public ISet<IServiceRegistration<object, object>> ServiceRegistrations { get; set; } = new HashSet<IServiceRegistration<object, object>>();

        public CancellationTokenSource? CancellationTokenSource { get; set; }
    }
}