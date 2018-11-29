using System;
using System.Diagnostics.CodeAnalysis;

namespace Nethium.DependencyInjection
{
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IServiceIdentifier<out TInterface>
    {
        string ServiceName { get; }

        Type ServiceType { get; }
    }
}