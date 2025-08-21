using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Abstractions;
using Rentals.Domain.Enums;
using Rentals.Domain.ValueObjects;

namespace Rentals.Domain.Entities
{
    public sealed class Courier : Entity
    {
        public string Identifier { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public Cnpj Cnpj { get; private set; } = default!;
        public DateOnly BirthDate { get; private set; }
        public CnhNumber CnhNumber { get; private set; } = default!;
        public LicenseType LicenseType { get; private set; }
        public string? CnhImageUrl { get; private set; }

        private Courier() { }

        private Courier(string identifier, string name, Cnpj cnpj, DateOnly birthDate, CnhNumber cnhNumber, LicenseType licenseType)
        {
            DomainException.ThrowIf(string.IsNullOrWhiteSpace(name), "Nome obrigatório.");
            DomainException.ThrowIf(birthDate > DateOnly.FromDateTime(DateTime.UtcNow), "Data de nascimento inválida.");
            Identifier = identifier;
            Name = name.Trim();
            Cnpj = cnpj;
            BirthDate = birthDate;
            CnhNumber = cnhNumber;
            LicenseType = licenseType;
        }

        public static Courier Create(string identifier, string name, Cnpj cnpj, DateOnly birthDate, CnhNumber cnhNumber, LicenseType licenseType)
            => new(identifier, name, cnpj, birthDate, cnhNumber, licenseType);

        public void SetCnhImageUrl(string url)
        {
            DomainException.ThrowIf(string.IsNullOrWhiteSpace(url), "URL da CNH inválida.");
            CnhImageUrl = url.Trim();
        }

        public bool CanRentMotorcycle() => LicenseType is LicenseType.A or LicenseType.AB;
    }
}
