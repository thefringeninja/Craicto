using System.Linq;
using System.Security.Claims;

namespace Craicto.Pipes.Example
{
    internal static class SecurityPipelineExtensions
    {
        public static IPipelineBuilder<MessageEnvelope<T>> RequiresAllClaims<T>(
            this IPipelineBuilder<MessageEnvelope<T>> builder,
            params Claim[] claims) => builder.Pipe(next => (message, ct) =>
        {
            if (!claims.All(claim => message.Subject.HasClaim(claim.Type, claim.Value)))
            {
                throw new AuthorizationFailedException(message.Subject);
            }

            return next(message, ct);
        });
        
        public static IPipelineBuilder<MessageEnvelope<T>> RequiresAnyClaim<T>(
            this IPipelineBuilder<MessageEnvelope<T>> builder,
            params Claim[] claims) => builder.Pipe(next => (message, ct) =>
        {
            if (!claims.Any(claim => message.Subject.HasClaim(claim.Type, claim.Value)))
            {
                throw new AuthorizationFailedException(message.Subject);
            }

            return next(message, ct);
        });

    }
}