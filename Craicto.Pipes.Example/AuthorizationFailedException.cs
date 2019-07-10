using System;
using System.Security.Claims;

namespace Craicto.Pipes.Example
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException(ClaimsPrincipal subject)
            : base($"Subject {subject.Identity.Name} was not authorized.")
        {
            
        }
    }
}