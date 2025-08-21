using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Motorcycles.ChangePlate
{
    public sealed record ChangeMotorcyclePlateCommand(string Id,
        [property: JsonPropertyName("placa")] string NewPlate) : IRequest;
}
