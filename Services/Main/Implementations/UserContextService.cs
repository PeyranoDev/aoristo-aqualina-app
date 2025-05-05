using Microsoft.AspNetCore.Http;
using System.Security.Claims;


public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(claim))
        {
            throw new UnauthorizedAccessException("User ID claim 'sub' is missing.");
        }

        if (!int.TryParse(claim, out var id))
        {
            throw new UnauthorizedAccessException($"User ID claim 'sub' is not a valid int: {claim}");
        }

        return id;
    }


    public string GetUserRole()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    public string GetFullName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("full name")?.Value ?? string.Empty;
    }

    public string GetUsername()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value ?? string.Empty;
    }
}