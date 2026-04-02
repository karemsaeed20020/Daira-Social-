using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daira.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message)
       : base(message, "NOT_FOUND")
        {
        }
        public NotFoundException(string entityName, object key)
           : base($"Entity \"{entityName}\" ({key}) was not found.", "NOT_FOUND")
        {
        }
        public static NotFoundException UserNotFound(string identifier)
            => new("User", identifier);
    }
}
