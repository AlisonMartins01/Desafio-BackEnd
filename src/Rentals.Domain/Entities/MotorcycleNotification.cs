using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Abstractions;

namespace Rentals.Domain.Entities
{

    public sealed class MotorcycleNotification : Entity
    {
        public Guid MotorcycleId { get; private set; }
        public string Identifier { get; private set; } = default!;
        public int Year { get; private set; }
        public string Reason { get; private set; } = default!;
        public DateTime CreatedAtUtc { get; private set; }

        private MotorcycleNotification() { }

        public static MotorcycleNotification Create(Guid motoId, string identifier, int year, string reason)
            => new()
            {
                Id = Guid.NewGuid(),
                MotorcycleId = motoId,
                Identifier = identifier,
                Year = year,
                Reason = reason,
                CreatedAtUtc = DateTime.UtcNow
            };
    }
}
