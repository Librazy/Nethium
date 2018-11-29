using System;
using Consul;

namespace Nethium.Consul
{
    public interface IConsulBuilder
    {
        Action<IConsulClient>? ClientOptions { get; set; }

        IConsulClient Build();
    }
}