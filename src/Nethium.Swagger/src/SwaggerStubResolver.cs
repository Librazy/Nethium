using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nethium.Swagger
{
    public class SwaggerStubResolver : ISwaggerStubResolver
    {
        private readonly Dictionary<Type, Type> _resolved = new Dictionary<Type, Type>();
        private readonly HashSet<Type> _stubs = new HashSet<Type>();

        public SwaggerStubResolver(params Assembly[] stubAssemblies)
        {
            foreach (var assembly in stubAssemblies)
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IStub).IsAssignableFrom(type))
                    {
                        _stubs.Add(type);
                    }
                }
        }

        public Type Resolve<T>()
        {
            var t = typeof(T);
            _resolved.TryGetValue(t, out var v);
            return v ?? (_resolved[t] = (from s in _stubs where t.IsAssignableFrom(s) select s).Single());
        }
    }
}