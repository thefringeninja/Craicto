namespace Craicto.Pipes
{
    public static class Handler
    {
        public static IPipelineBuilder<T> Of<T>() => new PipelineBuilder<T>();

        public static Handler<TInput> Narrow<TOutput, TInput>(Handler<TOutput> handler)
            where TOutput : TInput
            => (message, ct) => handler((TOutput) message, ct);

        public static Handler<TOutput> Widen<TInput, TOutput>(Handler<TOutput> handler)
            where TInput : class, TOutput
            => handler;

        public static Handler<T> Multiplex<T>(params Handler<T>[] handlers)
            => async (message, ct) =>
            {
                foreach (var handler in handlers)
                {
                    await handler(message, ct);
                }
            };
    }
}