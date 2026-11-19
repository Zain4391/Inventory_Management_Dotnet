namespace Inventory_Management_dotnet.Models.Entities;

/*
    Lifecycle:
        Pending -> Shipped -> Delivered
        Pending | Shipped -> Cancelled
    Shipped separates "accepted, stock committed" from "handed to the customer",
    so an order can be tracked in transit and Delivery_date only gets a value at the end.
*/
public enum STATUS
{
    Pending,
    Shipped,
    Delivered,
    Cancelled
}

public class Order
{
    public Guid Id {get; set;}

    /*
        Stored totals (not derived from Items):
        list/user queries can read them without loading every line item,
        and they freeze what the customer was charged.
        The service that places the order must set them from Items in the same SaveChanges.
    */
    public int Total_items {get; set;}

    public decimal Total_price {get; set;}

    public required string Delivery_address {get; set;}

    public Guid UserId {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    // Server-set when the order moves to Delivered, null until then
    public DateTime? Delivery_date {get; set;}

    public STATUS Order_status {get; set;} = STATUS.Pending;

    // Line items of the order
    public ICollection<OrderItem> Items {get; set;} = [];

    //For Reference
    public AppUser User {get; set;} = null!;
}
