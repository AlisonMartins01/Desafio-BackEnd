using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Rentals.Application.Abstractions;
using Rentals.Application.Rentals.Create;
using Rentals.Application.Services;
using Rentals.Domain.Entities;
using Rentals.Domain.Enums;
using Rentals.Domain.ValueObjects;

namespace Rentals.UnitTests.Application
{
    public class CreateRentalHandlerTests
    {
        [Fact]
        public async Task Handle_should_create_rental_for_courier_with_A_and_start_tomorrow()
        {
            // Arrange
            var courRepo = Substitute.For<ICourierRepository>();
            var motoRepo = Substitute.For<IMotorcycleRepository>();
            var rentRepo = Substitute.For<IRentalRepository>();
            var uow = Substitute.For<IUnitOfWork>();
            var pricing = new PricingService();

            var courier = Courier.Create("cour-001", "João", Cnpj.Create("12345678000190"),
                new DateOnly(1990, 1, 1), CnhNumber.Create("12345678901"), LicenseType.A);
            var moto = Motorcycle.Create("moto-001", 2024, "CG 160", Plate.Create("ABC1D23"));

            courRepo.GetByIdentifierAsync("cour-001", Arg.Any<CancellationToken>()).Returns(courier);
            motoRepo.GetByIdentifierAsync("moto-001", Arg.Any<CancellationToken>()).Returns(moto);
            rentRepo.HasActiveByMotorcycleAsync(moto.Id, Arg.Any<CancellationToken>()).Returns(false);

            var handler = new CreateRentalHandler(courRepo, motoRepo, rentRepo, uow, pricing);

            var cmd = new CreateRentalCommand(
                CourierIdentifier: "cour-001",
                MotorcycleIdentifier: "moto-001",
                PlanDays: 7,
                RequestStartUtc: null,
                RequestEndUtc: null,
                RequestExpectedEndUtc: null
            );

            // Act
            var dto = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            dto.StartDate.Should().Be(today.AddDays(1));
            dto.PlanDays.Should().Be(7);

            await rentRepo.Received(1).AddAsync(Arg.Any<Rental>(), Arg.Any<CancellationToken>());
            await uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_should_fail_when_courier_not_A()
        {
            var courRepo = Substitute.For<ICourierRepository>();
            var motoRepo = Substitute.For<IMotorcycleRepository>();
            var rentRepo = Substitute.For<IRentalRepository>();
            var uow = Substitute.For<IUnitOfWork>();
            var pricing = new PricingService();

            var courier = Courier.Create("cour-002", "Maria", Cnpj.Create("11222333000199"),
                new DateOnly(1991, 2, 2), CnhNumber.Create("10987654321"), LicenseType.B);
            var moto = Motorcycle.Create("moto-002", 2024, "CG 160", Plate.Create("ABC1D23"));

            courRepo.GetByIdentifierAsync("cour-002", Arg.Any<CancellationToken>()).Returns(courier);
            motoRepo.GetByIdentifierAsync("moto-002", Arg.Any<CancellationToken>()).Returns(moto);
            rentRepo.HasActiveByMotorcycleAsync(moto.Id, Arg.Any<CancellationToken>()).Returns(false);

            var handler = new CreateRentalHandler(courRepo, motoRepo, rentRepo, uow, pricing);

            var cmd = new CreateRentalCommand("cour-002", "moto-002", 7, null, null, null);

            Func<Task> act = () => handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Entregador não habilitado na categoria A.");
        }
    }
}
