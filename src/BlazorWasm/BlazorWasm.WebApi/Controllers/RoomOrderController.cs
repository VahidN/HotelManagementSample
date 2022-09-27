using BlazorServer.Models;
using BlazorServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RoomOrderController : Controller
{
    private readonly IRoomOrderDetailsService _roomOrderService;

    public RoomOrderController(IRoomOrderDetailsService roomOrderService) =>
        _roomOrderService = roomOrderService ?? throw new ArgumentNullException(nameof(roomOrderService));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoomOrderDetailsDto details)
    {
        var result = await _roomOrderService.CreateAsync(details);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderDetail(int trackingNumber)
    {
        var result = await _roomOrderService.GetOrderDetailByTrackingNumberAsync(trackingNumber);
        return Ok(result);
    }
}