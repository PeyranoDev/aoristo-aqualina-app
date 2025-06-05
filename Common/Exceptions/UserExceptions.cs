using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(int userId)
            : base("User", userId) { }
    }

    public class UserWithoutApartmentException : NotFoundException
    {
        public UserWithoutApartmentException(int userId)
            : base("User", $"{userId} does not have an associated apartment.") { }
    }

    public class EmailAlreadyExistsException : ConflictException
    {
        public EmailAlreadyExistsException(string email)
            : base($"The email '{email}' is already registered.") { }
    }

    public class UsernameAlreadyExistsException : ConflictException
    {
        public UsernameAlreadyExistsException(string username)
            : base($"The username '{username}' is already registered.") { }
    }

    public class InvalidRoleException : BadRequestException
    {
        public InvalidRoleException(int roleId)
            : base($"The role with ID '{roleId}' is invalid.") { }
    }
}
