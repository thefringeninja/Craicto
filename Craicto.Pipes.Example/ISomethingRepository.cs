using System.Threading;
using System.Threading.Tasks;

namespace Craicto.Pipes.Example
{
    public interface ISomethingRepository
    {
        Task<Something> GetById(SomethingIdentifier identifier, CancellationToken cancellationToken = default);
        void Add(Something something);
    }
}