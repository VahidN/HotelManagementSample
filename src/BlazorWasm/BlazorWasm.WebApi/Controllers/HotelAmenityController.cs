using BlazorServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasm.WebApi.Controllers;

[Route("api/[controller]")]
public class HotelAmenityController : Controller
{
    private readonly IAmenityService _hotelAmenityService;

    public HotelAmenityController(IAmenityService hotelAmenityService) => _hotelAmenityService = hotelAmenityService;

    [HttpGet]
    public async Task<IActionResult> GetHotelAmenities()
    {
        var allAmenity = await _hotelAmenityService.GetAllHotelAmenityAsync();
        return Ok(allAmenity);
    }
}