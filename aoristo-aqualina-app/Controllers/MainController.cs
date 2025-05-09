using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace aoristo_aqualina_app.Controllers
{
    public class MainController : ControllerBase   
    {
        internal int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        internal string GetUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }
    }
}
