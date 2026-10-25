using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Exceptions;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Inventory_Management_dotnet.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<IUserService> _logger;

    private readonly IUserRepository _userRepository;

    public UserService(UserManager<AppUser> userManager, ILogger<IUserService> logger, IUserRepository userRepository)
    {
        _userManager = userManager;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<PageResult<UserResponseDto>> GetAllUsers(UserQueryRequest userQueryRequest)
    {
        _logger.LogInformation($"Getting Users");
        if(userQueryRequest.Page < 1)
        {
            _logger.LogError($"Invalid Page Number");
            throw new BadRequestException("Page cannot be negative");
        }

        if(userQueryRequest.PageSize < 1 || userQueryRequest.PageSize > 100)
        {
            _logger.LogError($"Invalid Page size");
            throw new BadRequestException("Page must be between 1 and 100");
        }

        PageResult<AppUser> users  =await _userRepository.GetAllUsers(userQueryRequest);
        return new PageResult<UserResponseDto>
        {
            Items = users.Items.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email!,
                Gender = u.Gender,
                CreatedAt = u.CreatedAt,
                Orders = u.Orders.Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    Total_items = o.Total_items,
                    Total_price = o.Total_price,
                    Delivery_address = o.Delivery_address,
                    CreatedAt = o.CreatedAt,
                    Delivery_date = o.Delivery_date,
                    Order_status = o.Order_status
                }).ToList()
            }),
            Page = users.Page,
            PageSize = users.PageSize,
            TotalCount = users.TotalCount
        };

    }

    public async Task<UserResponseDto> GetUserById(Guid id)
    {
        _logger.LogInformation("Getting User with ID: {id}", id);

        AppUser? user = await _userRepository.GetUserById(id);
        
        if(user is null)
        {
            _logger.LogWarning("User with {id} was not found", id);
           throw new NotFoundException("User", id);
        }

        UserResponseDto response = new()
        {
          Id = user.Id,
          Name = user.Name,
          Email = user.Email!,
          Gender = user.Gender,
          CreatedAt = user.CreatedAt 
        };

        return response;
    }

    public async Task<UpdateUserResponseDto> UpdateUser(UpdateUserRequestDto updateUserResponseDto, Guid id)
    {
        AppUser? user = await _userManager.FindByIdAsync(id.ToString());
        if(user is null)
        {
            _logger.LogError("User with {id} not found", id);
            throw new NotFoundException($"User with {id} not found", id);
        }

        if(id != user.Id)
        {
            _logger.LogError("User ids do not match");
            throw new UnauthorizedException("Invalid user, please enter valid credentials");
        }

        if(updateUserResponseDto.Age.HasValue)
        {
            _logger.LogInformation("Update user Age");
            user.Age = updateUserResponseDto.Age.Value;
        }

        if(updateUserResponseDto.Name != null)
        {
            _logger.LogInformation("Update user Name");
            user.Name = updateUserResponseDto.Name;
        }

        if(updateUserResponseDto.Email != null)
        {
            _logger.LogInformation("Update user Email");
            await _userManager.SetEmailAsync(user, updateUserResponseDto.Email);
        }

        if(updateUserResponseDto.Gender.HasValue)
        {
            _logger.LogInformation("Update user Gender");
            user.Gender = updateUserResponseDto.Gender.Value;
        }

        var result = await _userManager.UpdateAsync(user);

        // Fail hard with errors
        if(result.Errors != null)
        {
            foreach(var error in result.Errors)
            {
                throw new ValidationException(error.Code, error.Description);
            }
        }

        // map to DTO
        UpdateUserResponseDto dto = new UpdateUserResponseDto
        {
            Name = user.Name,
            Age = user.Age,
            Email = user.Email!
        };

        return dto;

    }
}