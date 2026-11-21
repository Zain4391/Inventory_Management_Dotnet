using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Exceptions;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Inventory_Management_dotnet.Services.Interfaces;

namespace Inventory_Management_dotnet.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<IOrderService> _logger;

    // Total_price is stored as numeric(10, 2), anything larger fails on insert
    private const decimal MaxOrderTotal = 99_999_999.99m;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<IOrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<OrderResponseDto> PlaceOrder(PlaceOrderRequestDto placeOrderRequestDto, Guid userId)
    {
        _logger.LogInformation("Placing order for user {userId} with {itemCount} items", userId, placeOrderRequestDto.Items.Count);

        /*
            Merge duplicate ProductIds (unique (OrderId, ProductId) index allows one line per product).
            Summed as long: two quantities near int.MaxValue would overflow an int into a negative number
            and slip past the stock check.
        */
        Dictionary<Guid, long> requestedQuantities = placeOrderRequestDto.Items
            .GroupBy(i => i.ProductId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(i => (long)i.Quantity));

        // Load all products in one query
        List<Product> productList = await _productRepository.GetProductsByIds(requestedQuantities.Keys);
        Dictionary<Guid, Product> products = productList.ToDictionary(p => p.Id);

        List<OrderItem> items = [];

        foreach ((Guid productId, long quantity) in requestedQuantities)
        {
            if(!products.TryGetValue(productId, out Product? product))
            {
                _logger.LogWarning("Product {productId} in order for user {userId} was not found", productId, userId);
                throw new NotFoundException("Product", productId);
            }

            // Early check for a clear message, the repository's conditional UPDATE is the real guard
            if(quantity > product.Quantity)
            {
                _logger.LogWarning("Not enough stock for product {productId}: requested {requested}, available {available}",
                    productId, quantity, product.Quantity);
                throw new ConflictException($"Not enough stock for product '{product.Name}'. Requested {quantity}, available {product.Quantity}");
            }

            /*
                Only ProductId is set, not the Product navigation.
                Attaching an untracked Product here would make EF try to INSERT it again.
                UnitPrice is the price snapshot at this moment.
            */
            items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = (int)quantity,
                UnitPrice = product.Price
            });
        }

        // Stored totals, kept consistent with Items here
        int totalItems = items.Sum(i => i.Quantity);
        decimal totalPrice = items.Sum(i => i.Quantity * i.UnitPrice);

        if(totalPrice > MaxOrderTotal)
        {
            _logger.LogWarning("Order total {totalPrice} for user {userId} exceeds the maximum", totalPrice, userId);
            throw new BadRequestException($"Order total cannot exceed {MaxOrderTotal}");
        }

        Order newOrder = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Delivery_address = placeOrderRequestDto.DeliveryAddress,
            Items = items,
            Total_items = totalItems,
            Total_price = totalPrice,
            Order_status = STATUS.Pending,
            Delivery_date = null
        };

        Order? placedOrder = await _orderRepository.PlaceOrder(newOrder);

        // null = stock was taken by a concurrent order between our check and the UPDATE
        if(placedOrder is null)
        {
            _logger.LogWarning("Stock changed while placing order for user {userId}, order rolled back", userId);
            throw new ConflictException("Not enough stock for one or more products, please try again");
        }

        _logger.LogInformation("Placed order {orderId} for user {userId}", placedOrder.Id, userId);

        // map to DTO, using the products already loaded for the names
        return MapOrder(placedOrder, products);
    }

    public async Task<OrderResponseDto> GetOrderById(Guid id)
    {
        _logger.LogInformation("Getting Order with ID: {id}", id);

        Order? order = await _orderRepository.GetOrderById(id);

        if(order is null)
        {
            _logger.LogWarning("Order with {id} was not found", id);
            throw new NotFoundException("Order", id);
        }

        return MapOrder(order);
    }

    public async Task<PageResult<OrderResponseDto>> GetAllOrders(OrderQueryRequest orderQueryRequest, Guid? userId)
    {
        _logger.LogInformation("Getting Orders for user {userId}", userId);

        if(orderQueryRequest.Page < 1)
        {
            _logger.LogWarning("Invalid Page Number {page}", orderQueryRequest.Page);
            throw new BadRequestException("Page must be at least 1");
        }

        if(orderQueryRequest.PageSize < 1 || orderQueryRequest.PageSize > 100)
        {
            _logger.LogWarning("Invalid Page size {pageSize}", orderQueryRequest.PageSize);
            throw new BadRequestException("Page size must be between 1 and 100");
        }

        PageResult<Order> orders = await _orderRepository.GetAllOrders(orderQueryRequest, userId);

        return new PageResult<OrderResponseDto>
        {
            Items = orders.Items.Select(o => MapOrder(o)).ToList(),
            Page = orders.Page,
            PageSize = orders.PageSize,
            TotalCount = orders.TotalCount
        };
    }

    /*
        Single entity -> DTO mapping for all methods.
        PlaceOrder passes the products it already loaded, reads use the Items.Product navigation
        that the repository includes.
    */
    private static OrderResponseDto MapOrder(Order order, IReadOnlyDictionary<Guid, Product>? products = null)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            Total_items = order.Total_items,
            Total_price = order.Total_price,
            Delivery_address = order.Delivery_address,
            CreatedAt = order.CreatedAt,
            Delivery_date = order.Delivery_date,
            Order_status = order.Order_status,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = products is not null ? products[i.ProductId].Name : i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.Quantity * i.UnitPrice
            }).ToList()
        };
    }
}
