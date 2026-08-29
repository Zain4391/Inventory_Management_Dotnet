using Inventory_Management_dotnet.Exceptions;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Inventory_Management_dotnet.Services;

public class AuthService : IAuthService
{
    // User manager has the creation logic and is an abstraction already
    public readonly UserManager<AppUser> _userManager;
    public readonly ITokenService _tokenService;
    public readonly ILogger<IAuthService> _logger;
    public readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(UserManager<AppUser> userManager, ITokenService tokenService, 
    ILogger<IAuthService> logger, IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<SignUpResponseDto> SignUp(SignUpRequestDto signUpRequestDto)
    {
        Guid id = Guid.NewGuid();

        _logger.LogInformation("Creating User with ID: {}", id);

        AppUser user = new AppUser
        {
            Id = id,
            Name = signUpRequestDto.Name,
            Age = signUpRequestDto.Age,
            Gender = signUpRequestDto.Gender,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Saving user {} to Database", id);

        var response = await _userManager.CreateAsync(user, signUpRequestDto.Password);

        if(!response.Succeeded)
        {
            _logger.LogError("Invalid arguments or fields for user {}", id);
            throw new BadRequestException("Invalid request arguments");
        }

        // assign default role of Staff
        await _userManager.AddToRoleAsync(user, Roles.Staff);

        // Map tp response dto and return
        SignUpResponseDto dto = new SignUpResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email!,
            Age = user.Age,
            Gender = user.Gender,
            CreatedAt = user.CreatedAt
        };

        return dto;
    }

    public async Task<LogInResponseDto> LogIn(LogInRequestDto logInRequestDto)
    {
        _logger.LogInformation("Finding user with email {}", logInRequestDto.Email);
        
        // Check if user exists, if not throw bad request exception
        AppUser? existingUser = await _userManager.FindByEmailAsync(logInRequestDto.Email);

        if(existingUser is null)
        {
            _logger.LogError("User with Email {} not found", logInRequestDto.Email);
            throw new NotFoundException("User with provided email not found", logInRequestDto.Email);
        }

        // Generate now Access and Refresh Tokens
        _logger.LogInformation("User found, generating tokens");
        var roles = await _userManager.GetRolesAsync(existingUser);
        var (token, ExpiresAt) = _tokenService.CreateToken(existingUser, roles);

        string refreshToken = _tokenService.CreateRefreshToken();

        // hash the token to store it in DB
        string tokenHash = _tokenService.HashRefreshToken(refreshToken);
        
        _logger.LogInformation("Saving token to the database");
        await _refreshTokenRepository.SaveRefreshToken(tokenHash, existingUser.Id);

        LogInResponseDto dto = new LogInResponseDto
        {
            Access_token = token,
            ExpiresAt = ExpiresAt,
            Refresh_token = refreshToken
        };

        return dto;

    }

    public async Task<RefreshTokenResponseDto> Refresh(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        _logger.LogInformation("Revoking the Token");
        RefreshToken? token = await _refreshTokenRepository.RevokeRefreshToken(refreshTokenRequestDto);

        if(token is null)
        {
            _logger.LogError("Token not found");
            throw new UnauthorizedException("Token must be provided");
        }

        // Generate the new token
        string refreshToken = _tokenService.CreateRefreshToken();

        // hash the token to store it in DB
        string tokenHash = _tokenService.HashRefreshToken(refreshToken);
        
        _logger.LogInformation("Saving token to the database");
        await _refreshTokenRepository.SaveRefreshToken(tokenHash, token.User.Id);

        RefreshTokenResponseDto dto = new RefreshTokenResponseDto
        {
            RefreshToken = refreshToken
        };

        return dto;
    }
    
}