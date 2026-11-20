using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_dotnet.Models.DTOs.Request;

public class PlaceOrderRequestDto
{
    // Max length matches OrderConfig.Delivery_address
    [Required(ErrorMessage = "DeliveryAddress is required")]
    [StringLength(100, ErrorMessage = "DeliveryAddress cannot exceed 100 characters")]
    public required string DeliveryAddress {get; set;}

    // Duplicate ProductIds are allowed here, the service merges them into one line
    [Required(ErrorMessage = "Items is required")]
    [MinLength(1, ErrorMessage = "Order must contain at least one item")]
    public required List<OrderItemRequestDto> Items {get; set;} = [];
}
