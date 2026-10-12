

using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Models.DTOs.Response;

public class UserResponseDto
{
    public Guid Id {get; set;}

    public required string Name {get; set;}

    public required string Email {get; set;}

    public GENDER Gender {get; set;}

    public DateTime CreatedAt {get; set;}

    // TODO: Add ICollection<OrderResponseDto> here after making order service
    public ICollection<OrderResponseDto> Orders {get; set;} = [];
}