using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Rentals.Infrastructure.Messaging.Consumers;

namespace Rentals.Infrastructure.Messaging.Definition
{
    public sealed class MotorcycleRegisteredConsumerDefinition : ConsumerDefinition<MotorcycleRegisteredConsumer>
    {
        public MotorcycleRegisteredConsumerDefinition()
        {
            // nome fixo da fila
            EndpointName = "motorcycle-registered-notifications";

            // opcional: limite de concorrência por instância do consumer
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpoint,
            IConsumerConfigurator<MotorcycleRegisteredConsumer> consumer)
        {
            endpoint.PrefetchCount = 16;

            consumer.UseMessageRetry(r =>
            {
                r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(5));
            });
        }
    }
}
