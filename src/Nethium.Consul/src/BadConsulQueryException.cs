using System;
using System.Collections;
using System.Collections.Generic;
using Consul;

namespace Nethium.Consul
{
    public class BadConsulQueryException<T> : Exception where T : ConsulResult
    {
        public BadConsulQueryException(T queryResult) : this(queryResult, null)
        {
        }

        public BadConsulQueryException(T queryResult, string? message) : this(queryResult, message,
            null)
        {
        }

        public BadConsulQueryException(T queryResult, string? message, IDictionary? data)
        {
            QueryResult = queryResult;
            Message = message;
            Data = data ?? new Dictionary<string, object>();
        }

        public T QueryResult { get; }

        public override IDictionary Data { get; }

        public override string? Message { get; }
    }
}