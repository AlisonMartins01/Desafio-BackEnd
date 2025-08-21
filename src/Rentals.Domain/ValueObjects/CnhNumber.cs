using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Abstractions;

namespace Rentals.Domain.ValueObjects
{
    public sealed class CnhNumber : ValueObject
    {
        public string Digits { get; }

        private CnhNumber(string digits) => Digits = digits;

        public static CnhNumber Create(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) throw new DomainException("CNH obrigatória.");
            var digits = new string(raw.Where(char.IsDigit).ToArray());
            if (digits.Length is < 8 or > 14) // tolerante; alguns ambientes usam 11
                throw new DomainException("CNH inválida.");
            return new CnhNumber(digits);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Digits;
        }

        public override string ToString() => Digits;
    }
}
