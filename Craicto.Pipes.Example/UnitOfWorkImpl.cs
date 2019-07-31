using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace Craicto.Pipes.Example
{
    internal class UnitOfWorkImpl : IUnitOfWork
    {
        private readonly Guid _commitId;
        private readonly IStreamStore _streamStore;
        private readonly Action _complete;
        private readonly IDictionary<string, AggregateRoot> _aggregateRoots;

        public UnitOfWorkImpl(Guid commitId, IStreamStore streamStore, Action complete)
        {
            _commitId = commitId;
            _streamStore = streamStore;
            _complete = complete;
            _aggregateRoots = new Dictionary<string, AggregateRoot>();
        }

        public bool Contains(string streamId) => _aggregateRoots.ContainsKey(streamId);

        public bool TryGet(string streamId, out AggregateRoot aggregateRoot) =>
            _aggregateRoots.TryGetValue(streamId, out aggregateRoot);

        public void Add(string streamId, AggregateRoot aggregateRoot) => _aggregateRoots.Add(streamId, aggregateRoot);

        public async Task<string> Commit(CancellationToken cancellationToken)
        {
            var (streamId, aggregateRoot) = _aggregateRoots.SingleOrDefault(a => a.Value.HasChanges);

            if (aggregateRoot == null)
            {
                return default;
            }

            var result = await _streamStore.AppendToStream(
                streamId,
                0,
                aggregateRoot
                    .GetChanges()
                    .Select(e => new NewStreamMessage(
                        _commitId,
                        e.GetType().FullName.ToString(),
                        JsonSerializer.Serialize(e)))
                    .ToArray(),
                cancellationToken);

            aggregateRoot.MarkChangesAsCommitted();

            return result.CurrentPosition.ToString();
        }

        public void Dispose() => _complete();
    }
}