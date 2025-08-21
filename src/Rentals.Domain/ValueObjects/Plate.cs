using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rentals.Domain.Abstractions;

namespace Rentals.Domain.ValueObjects
{
    public sealed class Plate : ValueObject
    {
        public string Value { get; }

        private Plate(string value) => Value = value;

        public static Plate Create(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                throw new DomainException("Placa obrigatória.");

            var v = raw.Trim().ToUpperInvariant().Replace("-", "");
            var old = new Regex(@"^[A-Z]{3}[0-9]{4}$", RegexOptions.Compiled);
            var mercosul = new Regex(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$", RegexOptions.Compiled);

            if (!old.IsMatch(v) && !mercosul.IsMatch(v))
                throw new DomainException("Placa inválida.");

            return new Plate(v);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
