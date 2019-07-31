using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Craicto.Pipes
{
    public class Dispatcher
    {
        private readonly IDictionary<Type, Handler<object>> _handlers;

        public Dispatcher()
        {
            _handlers = new Dictionary<Type, Handler<object>>();
        }

        public void Subscribe<T>(Handler<T> handler)
            where T : class
        {
            if (!_handlers.TryGetValue(typeof(T), out var handlers))
            {
                _handlers[typeof(T)] = Handler.Narrow<T, object>(handler);
            }
            else
            {
                _handlers[typeof(T)] = Handler.Multiplex(handlers, Handler.Narrow<T, object>(handler));
            }
        }

        public Task Handle(object message, CancellationToken ct)
        {
            var type = message.GetType();

            return !_handlers.TryGetValue(type, out var handler) ? Task.CompletedTask : handler(message, ct);
        }
    }
}