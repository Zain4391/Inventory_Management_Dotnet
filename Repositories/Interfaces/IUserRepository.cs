using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Repositories.Interfaces;

/*

This abstraction will contain methods specific to our usecase
Other methods are already abstracted by User-Manager

*/
public interface IUserRepository
{
    Task<List<AppUser>> GetAllUsers();

    Task<AppUser?> GetUserById(Guid id);
}