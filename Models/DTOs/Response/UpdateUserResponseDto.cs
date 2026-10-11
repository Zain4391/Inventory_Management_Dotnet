
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Models.DTOs.Response;

public class UpdateUserResponseDto
{
    public required string Name {get; set;}

    public required string Email {get; set;}

    public int Age {get; set;}

    public GENDER Gender {get; set;}
}