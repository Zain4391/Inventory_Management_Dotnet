
using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Data;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventory_Management_dotnet.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _orderContext;

    public OrderRepository(AppDbContext orderDbContext)
    {
        _orderContext = orderDbContext;
    }

    // Method Implementations
    public async Task<Order?> PlaceOrder(Order order)
    {
        await using IDbContextTransaction transaction = await _orderContext.Database.BeginTransactionAsync();

        foreach (OrderItem item in order.Items.OrderBy(i => i.ProductId))
        {
            int rowsAffected = await _orderContext.Products
                .Where(p => p.Id == item.ProductId && p.Quantity >= item.Quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Quantity, p => p.Quantity - item.Quantity));

            // 0 rows = not enough stock (or product gone): undo any decrements already made
            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        // Insert the order, EF also inserts order.Items and fills in their OrderId
        await _orderContext.Orders.AddAsync(order);
        await _orderContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return order;
    }

    public async Task<Order?> GetOrderById(Guid id)
    {
        // Items.Product is needed so the service can map ProductName
        Order? order = await _orderContext.Orders
                        .AsNoTracking()
                        .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(o => o.Id == id);
        
        return order;
    }

    public async Task<PageResult<Order>> GetAllOrders(OrderQueryRequest orderQueryRequest, Guid? userId)
    {
        IQueryable<Order> query = _orderContext.Orders
                                .AsNoTracking()
                                .Include(o => o.Items)
                                .ThenInclude(i => i.Product);

        // Limit to one user's orders
        if(userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        if(orderQueryRequest.Status.HasValue)
        {
            query = query.Where(o => o.Order_status == orderQueryRequest.Status.Value);
        }

        query = orderQueryRequest.SortBy?.ToLower() switch
        {
            "price" => orderQueryRequest.SortDescending
                ? query.OrderByDescending(o => o.Total_price)
                : query.OrderBy(o => o.Total_price),

            "items" => orderQueryRequest.SortDescending
                ? query.OrderByDescending(o => o.Total_items)
                : query.OrderBy(o => o.Total_items),

            "status" => orderQueryRequest.SortDescending
                ? query.OrderByDescending(o => o.Order_status)
                : query.OrderBy(o => o.Order_status),

            "createdat" => orderQueryRequest.SortDescending
                ? query.OrderByDescending(o => o.CreatedAt)
                : query.OrderBy(o => o.CreatedAt),

            _ => query.OrderBy(o => o.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var orders = await query.Skip((orderQueryRequest.Page - 1) * orderQueryRequest.PageSize)
        .Take(orderQueryRequest.PageSize)
        .ToListAsync();

        PageResult<Order> dto = new PageResult<Order>
        {
            Items = orders,
            Page = orderQueryRequest.Page,
            PageSize = orderQueryRequest.PageSize,
            TotalCount = totalCount
        };

        return dto;
    }
}