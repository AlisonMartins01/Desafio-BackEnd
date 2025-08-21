using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Entities;

namespace Rentals.Application.Abstractions
{
    public interface ICourierRepository
    {
        Task AddAsync(Courier courier, CancellationToken ct);
        Task<Courier?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> CnpjExistsAsync(string cnpjDigits, CancellationToken ct);
        Task<bool> CnhNumberExistsAsync(string cnhDigits, CancellationToken ct);
        Task<Courier?> GetByIdentifierAsync(string identifier, CancellationToken ct);
    }
}
