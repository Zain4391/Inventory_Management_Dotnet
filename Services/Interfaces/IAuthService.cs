
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;

namespace Inventory_Management_dotnet.Services.Interfaces;

public interface IAuthService
{
    Task<SignUpResponseDto> SignUp(SignUpRequestDto signUpRequestDto);

    Task<LogInResponseDto> LogIn(LogInRequestDto logInRequestDto);

    Task<RefreshTokenResponseDto> Refresh(RefreshTokenRequestDto refreshTokenRequestDto);
}