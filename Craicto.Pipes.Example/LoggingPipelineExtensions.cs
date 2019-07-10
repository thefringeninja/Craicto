using System;

namespace Craicto.Pipes.Example
{
    internal static class LoggingPipelineExtensions
    {
        public static IPipelineBuilder<T> Log<T>(this IPipelineBuilder<T> builder)
            => builder.Pipe(next => async (message, ct) =>
            {
                Serilog.Log.Information(message.ToString());
                try
                {
                    await next(message, ct);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error");
                    throw;
                }
            });
    }
}