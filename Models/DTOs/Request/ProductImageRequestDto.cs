using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_dotnet.Models.DTOs.Request;

public class ProductImageRequestDto
{   
    public required IFormFile File {get; set;}
}