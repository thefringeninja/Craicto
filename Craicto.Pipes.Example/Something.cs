using System;

namespace Craicto.Pipes.Example
{
    public class Something : AggregateRoot
    {
        private bool _somethingHappened;
        private bool _somethingElseHappened;
        private SomethingIdentifier _identifier;

        public SomethingIdentifier Identifier => _identifier;

        private Something()
        {
            Register<SomethingHappened>(e =>
            {
                _identifier = new SomethingIdentifier(e.SomethingId);
                _somethingHappened = true;
            });
            Register<SomethingElseHappened>(_ => _somethingElseHappened = true);
        }

        public static Something Factory() => new Something();

        public static Something Happens(SomethingIdentifier identifier)
        {
            var something = Factory();

            something.Apply(new SomethingHappened {SomethingId = identifier.ToGuid()});

            return something;
        }

        public void ElseDo()
        {
            if (!_somethingHappened)
            {
                throw new InvalidOperationException();
            }

            if (_somethingElseHappened)
            {
                throw new InvalidOperationException();
            }

            Apply(new SomethingElseHappened {SomethingId = _identifier.ToGuid()});
        }
    }
}