using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key '{key}' was not found.", 404) { }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, 409) { }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized access.")
            : base(message, 401) { }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Forbidden access.")
            : base(message, 403) { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, 400) { }
    }
}