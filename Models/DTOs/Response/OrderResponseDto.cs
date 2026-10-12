
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Models.DTOs.Response;

public class OrderResponseDto
{
    public Guid Id {get; set;}

    public int Total_items {get; set;}

    public decimal Total_price {get; set;}

    public required string Delivery_address {get; set;}

    public DateTime CreatedAt {get; set;}

    public DateTime Delivery_date {get; set;}

    public STATUS Order_status {get; set;}
}