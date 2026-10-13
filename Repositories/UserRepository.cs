using Inventory_Management_dotnet.Common;
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

    public async Task<PageResult<AppUser>> GetAllUsers(UserQueryRequest userQueryRequest)
    {
        IQueryable<AppUser> query = _userContext.Users.AsNoTracking().Include(u => u.Orders);

        if(!string.IsNullOrEmpty(userQueryRequest.Name))
        {
            query = query.Where(p => p.Name == userQueryRequest.Name);
        }

        if(userQueryRequest.Gender.HasValue)
        {
            query = query.Where(p => p.Gender == userQueryRequest.Gender.Value);
        }

        if(userQueryRequest.MinAge.HasValue)
        {
            query = query.Where(p => p.Age >= userQueryRequest.MinAge);
        }

        if(userQueryRequest.MaxAge.HasValue)
        {
            query = query.Where(p => p.Age <= userQueryRequest.MaxAge);
        }

        query = userQueryRequest.SortBy?.ToLower() switch
        {
            "createdat" => userQueryRequest.SortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),

            "name" => userQueryRequest.SortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

            _ => query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync();
        var appUsers = await query.Skip((userQueryRequest.Page - 1) * userQueryRequest.PageSize)
        .Take(userQueryRequest.PageSize)
        .ToListAsync();

        PageResult<AppUser> dto = new PageResult<AppUser>
        {
            Items = appUsers,
            Page = userQueryRequest.Page,
            PageSize = userQueryRequest.PageSize,
            TotalCount = totalCount
        };

        return dto;
    }

    public async Task<AppUser?> GetUserById(Guid id)
    {
        AppUser? user = await _userContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }
}