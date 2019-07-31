using Microsoft.AspNetCore.Http;
using SqlStreamStore;

namespace Craicto.Pipes.Example
{
    internal static class UnitOfWorkPipelineExtensions
    {
        public static IPipelineBuilder<MessageEnvelope<T>> UseUnitOfWork<T>(
            this IPipelineBuilder<MessageEnvelope<T>> builder,
            IStreamStore streamStore,
            IHttpContextAccessor httpContextAccessor) => builder.Pipe(next =>
            async (message, ct) =>
            {
                using var unitOfWork = UnitOfWork.Start(message.MessageId, streamStore);
                await next(message, ct);
                var positionOfLastWrite = await unitOfWork.Commit(ct);

                httpContextAccessor.HttpContext.Response.Headers.Add("MyApp-PositionOfLastWrite", positionOfLastWrite);
            });
    }
}