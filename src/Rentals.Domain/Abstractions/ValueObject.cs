using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentals.Domain.Abstractions
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj) =>
            obj is ValueObject other && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

        public bool Equals(ValueObject? other) => Equals((object?)other);

        public override int GetHashCode() =>
            GetEqualityComponents().Aggregate(1, (hash, obj) => HashCode.Combine(hash, obj?.GetHashCode() ?? 0));

    }
}
