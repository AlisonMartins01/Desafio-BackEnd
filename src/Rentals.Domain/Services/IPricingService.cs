using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Enums;

namespace Rentals.Domain.Services
{
    public interface IPricingService
    {
        /// Retorna (valor diário, percentual de multa por término antecipado) para o plano.
        (decimal Daily, decimal? EarlyPenaltyPct) For(RentalPlan plan);

        /// Calcula o total com base nas regras: base (diárias feitas) + multa antecipação (se houver) + atraso (R$ 50/dia).
        decimal TotalFor(RentalPlan plan, decimal dailyRateSnapshot, int doneDays, int missingDays, int extraDays);
    }
}
