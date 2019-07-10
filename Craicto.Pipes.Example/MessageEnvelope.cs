using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Craicto.Pipes.Example
{
    public class MessageEnvelope<T>
    {
        public T Message { get; set; }
        public ClaimsPrincipal Subject { get; set; }
        public Guid MessageId { get; set; }
        public IReadOnlyDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}