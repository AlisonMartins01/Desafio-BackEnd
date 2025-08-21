using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentals.Application.Exceptions
{
    public sealed class BadRequestException : Exception
    {
        public BadRequestException() : base("Request mal formada") { }
        public BadRequestException(string message) : base(message) { }
    }
}
