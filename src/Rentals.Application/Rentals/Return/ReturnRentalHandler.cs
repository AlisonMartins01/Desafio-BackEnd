using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;
using Rentals.Domain.Services;

namespace Rentals.Application.Rentals.Return
{
    public sealed class ReturnRentalHandler : IRequestHandler<ReturnRentalCommand, ReturnRentalResultDto>
    {
        private readonly IRentalRepository _rentals;
        private readonly IUnitOfWork _uow;
        private readonly IPricingService _pricing;

        public ReturnRentalHandler(IRentalRepository rentals, IUnitOfWork uow, IPricingService pricing)
        {
            _rentals = rentals;
            _uow = uow;
            _pricing = pricing;
        }

        public async Task<ReturnRentalResultDto> Handle(ReturnRentalCommand req, CancellationToken ct)
        {
            var rental = await _rentals.GetByIdAsync(req.RentalId, ct)
                         ?? throw new KeyNotFoundException("Locação não encontrada.");

            var total = rental.CalculateReturnTotal(req.ReturnDate, _pricing);
            var (done, missing, extra) = rental.DiffDays(req.ReturnDate);

            rental.Finish(req.ReturnDate);
            await _uow.SaveChangesAsync(ct);

            return new ReturnRentalResultDto(rental.Id, total, done, missing, extra);
        }
    }
}
