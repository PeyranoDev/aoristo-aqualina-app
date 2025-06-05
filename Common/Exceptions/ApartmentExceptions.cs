using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class ApartmentNotFoundException : NotFoundException
    {
        public ApartmentNotFoundException(int id)
            : base("Apartment", id) { }
    }

    public class ApartmentConflictException : ConflictException
    {
        public ApartmentConflictException(string identifier)
            : base($"Apartment with identifier '{identifier}' already exists.") { }
    }

    public class ApartmentAlreadyDeactivatedException : BadRequestException
    {
        public ApartmentAlreadyDeactivatedException(int id)
            : base($"Apartment with id {id} is already deactivated.") { }
    }

    public class ApartmentRequiredException : BadRequestException
    {
        public ApartmentRequiredException()
            : base("An apartment is required for this role.") { }
    }
}
