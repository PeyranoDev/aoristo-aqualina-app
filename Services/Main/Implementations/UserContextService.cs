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
        return int.TryParse(claim, out var id) ? id : throw new UnauthorizedAccessException("Invalid user ID claim.");
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