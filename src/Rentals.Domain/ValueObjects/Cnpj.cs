using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Abstractions;

namespace Rentals.Domain.ValueObjects
{
    public sealed class Cnpj : ValueObject
    {
        public string Digits { get; }

        private Cnpj(string digits) => Digits = digits;

        public static Cnpj Create(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) throw new DomainException("CNPJ obrigatório.");

            var digits = new string(raw.Where(char.IsDigit).ToArray());
            if (digits.Length != 14) throw new DomainException("CNPJ inválido.");

            return new Cnpj(digits);
        }


        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Digits;
        }

        public override string ToString() => Digits;
    }
}
