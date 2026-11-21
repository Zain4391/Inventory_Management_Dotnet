namespace Inventory_Management_dotnet.Models.DTOs.Response;

public class OrderItemResponseDto
{
    public Guid ProductId {get; set;}

    public required string ProductName {get; set;}

    public int Quantity {get; set;}

    // Price snapshot taken when the order was placed, not the current Product.Price
    public decimal UnitPrice {get; set;}

    // Quantity * UnitPrice
    public decimal LineTotal {get; set;}
}
