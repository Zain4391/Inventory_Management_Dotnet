using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;

namespace Inventory_Management_dotnet.Services.Interfaces;

public interface IOrderService
{
    // userId comes from the caller's "sub" claim, never from the request body
    Task<OrderResponseDto> PlaceOrder(PlaceOrderRequestDto placeOrderRequestDto, Guid userId);

    Task<OrderResponseDto> GetOrderById(Guid id);

    Task<PageResult<OrderResponseDto>> GetAllOrders(OrderQueryRequest orderQueryRequest);
}
