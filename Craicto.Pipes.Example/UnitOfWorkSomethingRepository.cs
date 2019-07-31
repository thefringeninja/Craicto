using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore;

namespace Craicto.Pipes.Example
{
    internal class UnitOfWorkSomethingRepository : ISomethingRepository
    {
        private readonly IStreamStore _streamStore;

        public UnitOfWorkSomethingRepository(IStreamStore streamStore)
        {
            _streamStore = streamStore;
        }

        public async Task<Something> GetById(
            SomethingIdentifier identifier,
            CancellationToken cancellationToken = default)
        {
            var streamId = GetStreamId(identifier);

            if (UnitOfWork.Current.TryGet(streamId, out var aggregateRoot)) return (Something) aggregateRoot;
            var something = Something.Factory();
            await something.LoadFromHistory(ReadEvents(streamId, cancellationToken));
            UnitOfWork.Current.Add(streamId, something);
            return something;
        }

        public void Add(Something something) => UnitOfWork.Current.Add(GetStreamId(something.Identifier), something);

        private static string GetStreamId(SomethingIdentifier identifier) => $"something-{identifier.ToGuid():n}";

        private async IAsyncEnumerable<object> ReadEvents(string streamId, CancellationToken cancellationToken)
        {
            var page = await _streamStore.ReadStreamForwards(streamId, 0, int.MaxValue, cancellationToken);

            foreach (var message in page.Messages)
            {
                var type = typeof(DoSomething).Assembly.GetType(message.Type);

                yield return JsonSerializer.Deserialize(await message.GetJsonData(cancellationToken), type);
            }
        }
    }
}