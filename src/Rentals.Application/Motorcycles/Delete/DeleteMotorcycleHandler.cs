using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;

namespace Rentals.Application.Motorcycles.Delete
{
    public sealed class DeleteMotorcycleHandler : IRequestHandler<DeleteMotorcycleCommand>
    {
        private readonly IMotorcycleRepository _repo;
        private readonly IRentalRepository _rentalRepo;
        private readonly IUnitOfWork _uow;

        public DeleteMotorcycleHandler(IMotorcycleRepository repo, IRentalRepository rentalRepo, IUnitOfWork uow)
        {
            _repo = repo;
            _rentalRepo = rentalRepo;
            _uow = uow;
        }

        public async Task Handle(DeleteMotorcycleCommand request, CancellationToken ct)
        {
            var moto = await _repo.GetByIdentifierAsync(request.Id, ct) ?? throw new Exception("Motocicleta não encontrada.");

            if (await _rentalRepo.HasAnyByMotorcycleAsync(moto.Id, ct))
                throw new InvalidOperationException("Motocicleta vinculada a locações. Exclusão não permitida.");

            await _repo.RemoveAsync(moto, ct);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
