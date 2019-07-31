using System;
using System.Threading;
using System.Threading.Tasks;

namespace Craicto.Pipes.Example
{
    public interface IUnitOfWork : IDisposable
    {
        void Add(string streamId, AggregateRoot aggregateRoot);
        bool Contains(string streamId);
        bool TryGet(string streamId, out AggregateRoot aggregateRoot);
        Task<string> Commit(CancellationToken cancellationToken = default);
    }
}