using Rentals.Api.Contracts.Rentals;
using Rentals.Application.Rentals;

namespace Rentals.Api.Infrastructure
{
    public static class RentalMapping
    {
        public static RentalResponse ToResponse(this RentalDto dto)
        {
            var termino = dto.ExpectedEndDate.ToUtcDateTime();
            return new RentalResponse(
                Identificador: dto.Id.ToString("D"),
                ValorDiaria: dto.DailyRate,
                EntregadorId: dto.CourierIdentifier,
                MotoId: dto.MotorcycleIdentifier,
                DataInicio: dto.StartDate.ToUtcDateTime(),
                DataTermino: termino,
                DataPrevisaoTermino: termino,
                DataDevolucao: dto.EndDate.ToUtcDateTime()
            );
        }
    }
}
