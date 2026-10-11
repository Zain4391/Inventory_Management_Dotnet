
using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;

namespace Inventory_Management_dotnet.Services.Interfaces;

public interface IUserService
{
    Task<PageResult<UserResponseDto>> GetAllUsers(UserQueryRequest userQueryRequest);
    Task<UserResponseDto> GetUserById(Guid id);
    Task<UpdateUserResponseDto> UpdateUser(UpdateUserRequestDto updateUserRequestDto, Guid id);
}