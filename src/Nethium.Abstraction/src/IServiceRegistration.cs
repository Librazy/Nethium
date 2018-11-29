using System;
using System.Diagnostics.CodeAnalysis;

namespace Nethium.DependencyInjection
{
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IServiceRegistration<out TInterface, out TImpl>
        where TInterface : class where TImpl : class, TInterface
    {
        IServiceIdentifier<TInterface> ServiceIdentifier { get; }

        string ServiceName { get; }

        string Path { get; }

        string? CheckBaseUrl { get; }

        string CheckPath { get; }

        Type InterfaceType { get; }

        Type ImplType { get; }
    }
}