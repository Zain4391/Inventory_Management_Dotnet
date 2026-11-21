using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_dotnet.Controllers;

/*
    Any authenticated user (no role restriction).
    Ownership is enforced in OrderService from the token's "sub" claim.
*/
[Authorize]
[Route("api/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> PlaceOrder([FromBody] PlaceOrderRequestDto placeOrderRequestDto)
    {
        var response = await _orderService.PlaceOrder(placeOrderRequestDto);
        return CreatedAtAction(
            nameof(GetOrderById),
            new { id = response.Id },
            response
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(Guid id)
    {
        var response = await _orderService.GetOrderById(id);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<OrderResponseDto>>> GetAllOrders([FromQuery] OrderQueryRequest orderQueryRequest)
    {
        return Ok(await _orderService.GetAllOrders(orderQueryRequest));
    }
}
