using Common.Models;
using Data.Entities;

namespace Services.Main.Interfaces
{
    public interface IUserService
    {
        User? Validate(CreedentialsDTO dto);
    }
}