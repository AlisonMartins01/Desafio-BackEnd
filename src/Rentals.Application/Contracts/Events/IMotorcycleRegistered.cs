using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentals.Application.Contracts.Events
{
    public interface IMotorcycleRegistered
    {
        Guid Id { get; }
        string Identifier { get; }
        string Model { get; }
        int Year { get; }
        string Plate { get; }
        DateTime CreatedAtUtc { get; }
    }
}
