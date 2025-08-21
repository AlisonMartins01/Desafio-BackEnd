using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentals.Application.Exceptions
{
    public sealed class MotorcycleInvalidDataException : Exception
    {
        public MotorcycleInvalidDataException() : base("Dados inválidos") { }
    }
}
