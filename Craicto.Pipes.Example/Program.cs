using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SqlStreamStore;

namespace Craicto.Pipes.Example
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var streamStore = new InMemoryStreamStore();
            var somethings = new UnitOfWorkSomethingRepository(streamStore);
            var dispatcher = new Dispatcher();
            using var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(services => services.AddHttpContextAccessor())
                .Start();

            var httpContextAccessor = host.Services.GetService<IHttpContextAccessor>();

            dispatcher.Subscribe(
                Handler.Of<MessageEnvelope<DoSomething>>()
                    .Log()
                    .UseUnitOfWork(streamStore, httpContextAccessor)
                    .RequiresAnyClaim(new Claim("role", "something-doer"))
                    .Handle(CommandHandlers.DoSomething(somethings).Wrap(x => x.SubjectId)));
            dispatcher.Subscribe(
                Handler.Of<MessageEnvelope<DoSomethingElse>>()
                    .Log()
                    .UseUnitOfWork(streamStore, httpContextAccessor)
                    .RequiresAllClaims(new Claim("role", "something-doer"), new Claim("role", "somethingelse-doer"))
                    .Handle(CommandHandlers.DoSomethingElse(somethings).Wrap(x => x.SubjectId)));

            await host.RunAsync();

            return 0;
        }
    }
}