using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentals.Domain.Abstractions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public static void ThrowIf(bool condition, string message)
        {
            if (condition) throw new DomainException(message);
        }
    }
}
