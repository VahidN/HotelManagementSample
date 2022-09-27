using BlazorServer.Models;
using BlazorServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasm.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelRoomController : ControllerBase
{
    private readonly IHotelRoomService _hotelRoomService;

    public HotelRoomController(IHotelRoomService hotelRoomService) => _hotelRoomService = hotelRoomService;

    [HttpGet]
    public async Task<IActionResult> GetHotelRooms(DateTime? checkInDate, DateTime? checkOutDate)
    {
        if (!checkInDate.HasValue || !checkOutDate.HasValue)
        {
            return BadRequest(new ErrorModel
                              {
                                  StatusCode = StatusCodes.Status400BadRequest,
                                  ErrorMessage = "All parameters need to be supplied",
                              });
        }

        var allRooms = await _hotelRoomService.GetAllHotelRoomsAsync(checkInDate, checkOutDate);
        return Ok(allRooms);
    }

    [HttpGet("{roomId}")]
    public async Task<IActionResult> GetHotelRoom(int? roomId, DateTime? checkInDate, DateTime? checkOutDate)
    {
        if (roomId == null)
        {
            return BadRequest(new ErrorModel
                              {
                                  Title = "",
                                  ErrorMessage = "Invalid Room Id",
                                  StatusCode = StatusCodes.Status400BadRequest,
                              });
        }

        var roomDetails = await _hotelRoomService.GetHotelRoomAsync(roomId.Value, checkInDate, checkOutDate);
        if (roomDetails == null)
        {
            return BadRequest(new ErrorModel
                              {
                                  Title = "",
                                  ErrorMessage = "Invalid Room Id",
                                  StatusCode = StatusCodes.Status404NotFound,
                              });
        }

        return Ok(roomDetails);
    }
}