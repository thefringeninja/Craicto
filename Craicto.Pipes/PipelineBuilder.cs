using System.Collections.Generic;

namespace Craicto.Pipes
{
    internal class PipelineBuilder<T> : IPipelineBuilder<T>
    {
        private readonly Stack<Pipe<T>> _pipeline;

        public PipelineBuilder()
        {
            _pipeline = new Stack<Pipe<T>>();
        }

        public IPipelineBuilder<T> Pipe(Pipe<T> pipe)
        {
            _pipeline.Push(pipe);

            return this;
        }

        public Handler<T> Handle(Handler<T> handler)
        {
            while (_pipeline.Count > 0)
            {
                var pipe = _pipeline.Pop();

                handler = pipe(handler);
            }

            return handler;
        }
    }
}