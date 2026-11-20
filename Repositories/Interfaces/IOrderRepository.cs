using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Repositories.Interfaces;

public interface IOrderRepository
{
    /*
        Inserts the order with its items and decrements stock in ONE transaction.
        Returns null (after rolling back) if any conditional stock update affects 0 rows,
        the service turns that into a ConflictException.
    */
    Task<Order?> PlaceOrder(Order order);

    // Includes Items and Items.Product, AsNoTracking
    Task<Order?> GetOrderById(Guid id);

    // userId limits the results to one user's orders, null returns all orders
    Task<PageResult<Order>> GetAllOrders(OrderQueryRequest orderQueryRequest, Guid? userId);
}
