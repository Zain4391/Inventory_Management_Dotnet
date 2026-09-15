using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Repositories.Interfaces;

/*

This abstraction will contain methods specific to our usecase
Other methods are already abstracted by User-Manager

*/
public interface IUserRepository
{
    Task<PageResult<AppUser>> GetAllUsers(UserQueryRequest userQueryRequest);

    Task<AppUser?> GetUserById(Guid id);
}