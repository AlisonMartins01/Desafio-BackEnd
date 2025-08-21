using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rentals.Api.Contracts;
using Rentals.Api.Contracts.Rentals;
using Rentals.Api.Swagger;
using Rentals.Application.Rentals;
using Rentals.Application.Rentals.Create;
using Rentals.Application.Rentals.GetById;
using Rentals.Application.Rentals.Return;
using Swashbuckle.AspNetCore.Filters;
using static Rentals.Api.Infrastructure.DateMapping;

namespace Rentals.Api.Controllers
{
    [ApiController]
    [Route("locacao")]
    public class RentalsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RentalsController(IMediator mediator) => _mediator = mediator;


        [HttpPost]
        [ProducesResponseType(typeof(RentalResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErroResponseExample))]
        public async Task<IActionResult> Create([FromBody] CreateRentalRequest body, CancellationToken ct)
        {
            var dto = await _mediator.Send(new CreateRentalCommand(
                body.EntregadorId, body.MotoId, body.Plano,
                body.DataInicio, body.DataTermino, body.DataPrevisaoTermino
            ), ct);

            return Created();
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RentalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]

        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErroResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorNotFoundRentalExample))]

        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _mediator.Send(new GetRentalByIdQuery(id), ct);
            return Ok(Map(dto));
        }

        [HttpPut("{id:guid}/devolucao")]
        [ProducesResponseType(typeof(SuccessOkDevolution), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]

        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SuccessOkDevolution))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErroResponseExample))]
        public async Task<IActionResult> Return(Guid id, [FromBody] DevolutionRequest body, CancellationToken ct)
        {
            var result = await _mediator.Send(
                new ReturnRentalCommand(id, DateOnly.FromDateTime(body.DataDevolucao.ToUniversalTime())), ct);

            return Ok(new DevolutionSuccessResponse());
        }

        //Mapeamento na controller apenas para fins de facilidade,
        //o correto seria criar um profile para fazer o mapeamento
        private static RentalResponse Map(RentalDto dto)
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
