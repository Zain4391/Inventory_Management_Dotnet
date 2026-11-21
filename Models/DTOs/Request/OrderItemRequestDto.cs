using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_dotnet.Models.DTOs.Request;

public class OrderItemRequestDto
{
    // Nullable so a missing ProductId fails [Required] instead of binding to Guid.Empty
    [Required(ErrorMessage = "ProductId is required")]
    public Guid? ProductId {get; set;}

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity {get; set;}
}
