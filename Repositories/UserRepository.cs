using Inventory_Management_dotnet.Data;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_dotnet.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _userContext;

    public UserRepository(AppDbContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<List<AppUser>> GetAllUsers()
    {
        return await _userContext.Users.ToListAsync();
    }

    public async Task<AppUser?> GetUserById(Guid id)
    {
        AppUser? user = await _userContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }
}