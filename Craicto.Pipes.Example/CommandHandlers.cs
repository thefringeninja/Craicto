using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Craicto.Pipes.Example
{
    static class CommandHandlers
    {
        public static Handler<MessageEnvelope<T>> Wrap<T>(
            this Handler<T> inner,
            Expression<Func<T, Guid>> messageIdProperty)
        {
            var property = (PropertyInfo) ((MemberExpression) messageIdProperty.Body).Member;

            return (envelope, ct) =>
            {
                var subjectId = Guid.Parse(envelope.Subject.Claims.Single(x => x.Type == "sub").Value);
                property.SetValue(envelope.Message, subjectId);

                return inner(envelope.Message, ct);
            };
        }

        public static Handler<DoSomething> DoSomething(UnitOfWorkSomethingRepository somethings)
            => (message, ct) =>
            {
                var something = Something.Happens(new SomethingIdentifier(message.SomethingId));

                somethings.Add(something);

                return Task.CompletedTask;
            };

        public static Handler<DoSomethingElse> DoSomethingElse(ISomethingRepository somethings)
            => async (message, ct) =>
            {
                var something = await somethings.GetById(new SomethingIdentifier(message.SomethingId));

                something.ElseDo();
            };
    }
}