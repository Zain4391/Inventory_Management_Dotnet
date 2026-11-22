using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;

namespace Inventory_Management_dotnet.Services.Interfaces;

/*
    The caller is taken from ICookieService (the token's "sub" claim),
    never from the request, so no method takes a user id.
*/
public interface IOrderService
{
    // Places the order for the caller
    Task<OrderResponseDto> PlaceOrder(PlaceOrderRequestDto placeOrderRequestDto);

    // Owner or Admin only. Anyone else gets 404, same as a missing order
    Task<OrderResponseDto> GetOrderById(Guid id);

    // Admin sees all orders, everyone else only their own
    Task<PageResult<OrderResponseDto>> GetAllOrders(OrderQueryRequest orderQueryRequest);
}
