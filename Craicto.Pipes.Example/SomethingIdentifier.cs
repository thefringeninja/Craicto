using System;

namespace Craicto.Pipes.Example
{
    public struct SomethingIdentifier : IEquatable<SomethingIdentifier>
    {
        private readonly Guid _somethingId;

        public SomethingIdentifier(Guid somethingId)
        {
            if (somethingId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }

            _somethingId = somethingId;
        }

        public bool Equals(SomethingIdentifier other) => _somethingId.Equals(other._somethingId);
        public override bool Equals(object obj) => obj is SomethingIdentifier other && Equals(other);
        public override int GetHashCode() => _somethingId.GetHashCode();
        public static bool operator ==(SomethingIdentifier left, SomethingIdentifier right) => left.Equals(right);
        public static bool operator !=(SomethingIdentifier left, SomethingIdentifier right) => !left.Equals(right);
        public Guid ToGuid() => _somethingId;
    }
}