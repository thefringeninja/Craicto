using System.Data;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Serilog.Events;

namespace Craicto.Pipes.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var dispatcher = new Dispatcher();

            dispatcher.Subscribe(
                Handler.Of<MessageEnvelope<DoSomething>>()
                    .Log()
                    .RequiresAnyClaim(new Claim("role", "something-doer"))
                    .Handle(CommandHandlers.DoSomething(dispatcher).Wrap(x => x.SubjectId)));
            dispatcher.Subscribe(
                Handler.Of<MessageEnvelope<DoSomethingElse>>()
                    .Log()
                    .RequiresAllClaims(new Claim("role", "something-doer"), new Claim("role", "somethingelse-doer"))
                    .Handle(CommandHandlers.DoSomethingElse(dispatcher).Wrap(x => x.SubjectId)));
        }
    }
}