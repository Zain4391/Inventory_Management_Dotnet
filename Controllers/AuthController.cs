using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_dotnet.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    public readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult<SignUpResponseDto>> SignUp(SignUpRequestDto signUpRequestDto)
    {
        var response = await _authService.SignUp(signUpRequestDto);
        return Created($"/sign-up/{response.Id}", response);
    }

    [HttpPost("log-in")]
    public async Task<ActionResult<LogInResponseDto>> Login(LogInRequestDto logInRequestDto)
    {
        var response = await _authService.LogIn(logInRequestDto);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponseDto>> Refresh(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        var response = await _authService.Refresh(refreshTokenRequestDto);
        return Ok(response);
    }
}