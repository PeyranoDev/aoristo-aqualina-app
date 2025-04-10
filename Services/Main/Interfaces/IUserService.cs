using Common.Models;

namespace Services.Main.Interfaces
{
    public interface IUserService
    {
        User? Validate(CreedentialsDTO dto);
    }
}