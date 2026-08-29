namespace Inventory_Management_dotnet.Models.Entities;

public enum STATUS
{
    Pending,
    Delivered,
    Cancelled
}

public class Order
{
    public Guid Id {get; set;}

    public int Total_items {get; set;}

    public decimal Total_price {get; set;}

    public required string Delivery_address {get; set;}

    public Guid UserId {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime Delivery_date {get; set;}

    public STATUS Order_status {get; set;}

    //For Reference
    public AppUser User {get; set;} = null!;
}
