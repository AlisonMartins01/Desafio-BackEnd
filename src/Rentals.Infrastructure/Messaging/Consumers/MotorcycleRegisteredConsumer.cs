using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Application.Contracts.Events;
using Rentals.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Rentals.Application.Contracts.Events;
using MassTransit;

namespace Rentals.Infrastructure.Messaging.Consumers
{
    public sealed class MotorcycleRegisteredConsumer : IConsumer<IMotorcycleRegistered>
    {
        private readonly RentalsDbContext _db;

        public MotorcycleRegisteredConsumer(RentalsDbContext db) => _db = db;

        public async Task Consume(ConsumeContext<IMotorcycleRegistered> context)
        {
            var msg = context.Message;

            if (msg.Year == 2024)
            {
                var notif = Domain.Entities.MotorcycleNotification.Create(
                    msg.Id, msg.Identifier, msg.Year, "Year is 2024");

                _db.MotorcycleNotifications.Add(notif);
                await _db.SaveChangesAsync(context.CancellationToken);
            }
        }
    }
}
