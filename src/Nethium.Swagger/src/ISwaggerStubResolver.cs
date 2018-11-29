using System;

namespace Nethium.Swagger
{
    public interface ISwaggerStubResolver
    {
        Type Resolve<T>();
    }
}