using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Abstractions;
using Rentals.Domain.Events;
using Rentals.Domain.ValueObjects;

namespace Rentals.Domain.Entities
{
    public sealed class Motorcycle : Entity
    {
        public string Identifier { get; private set; } = default!;
        public int Year { get; private set; }
        public string Model { get; private set; } = default!;
        public Plate Plate { get; private set; } = default!;

        private Motorcycle() { } // EF

        private Motorcycle(string identifier, int year, string model, Plate plate)
        {
            DomainException.ThrowIf(string.IsNullOrWhiteSpace(identifier), "Identificador obrigatório.");
            DomainException.ThrowIf(year < 1900 || year > DateTime.UtcNow.Year + 1, "Ano inválido.");
            DomainException.ThrowIf(string.IsNullOrWhiteSpace(model), "Modelo obrigatório.");

            Identifier = identifier.Trim();
            Year = year;
            Model = model.Trim();
            Plate = plate;

            Raise(new MotorcycleRegistered(Id, Identifier, Year, Model, Plate.Value));
        }

        public static Motorcycle Create(string identifier, int year, string model, Plate plate)
            => new(identifier, year, model, plate);

        public void ChangePlate(Plate newPlate)
        {
            DomainException.ThrowIf(newPlate is null, "Placa inválida.");
            if (Plate.Value == newPlate.Value) return;
            Plate = newPlate;
        }
    }
}
