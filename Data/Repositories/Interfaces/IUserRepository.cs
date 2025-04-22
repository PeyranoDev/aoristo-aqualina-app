using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User? Validate(string username, string password);
    }
}