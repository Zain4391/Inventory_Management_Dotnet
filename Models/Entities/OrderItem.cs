namespace Inventory_Management_dotnet.Models.Entities;

public class OrderItem
{
    public Guid Id {get; set;}

    // FK, links to Orders Table
    public Guid OrderId {get; set;}

    // FK, links to Products Table
    public Guid ProductId {get; set;}

    public int Quantity {get; set;}

    /*
        Price of ONE unit copied from Product.Price when the order is placed.
        Product prices change over time, reading Product.Price later would rewrite
        what the customer was actually charged and break Order.Total_price.
    */
    public decimal UnitPrice {get; set;}

    // for referencing
    public Order Order {get; set;} = null!;

    // for referencing
    public Product Product {get; set;} = null!;
}
