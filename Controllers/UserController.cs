using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_dotnet.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Staff}")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
    {
        var response = await _userService.GetUserById(id);
        return Ok(response);
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Staff}")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateUserResponseDto>> UpdateUserFields(Guid id, [FromBody] UpdateUserRequestDto updateUserRequestDto)
    {
        var response = await _userService.UpdateUser(updateUserRequestDto, id);
        return Ok(response);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers([FromQuery] UserQueryRequest userQueryRequest)
    {
        return Ok(await _userService.GetAllUsers(userQueryRequest));
    }
    
}