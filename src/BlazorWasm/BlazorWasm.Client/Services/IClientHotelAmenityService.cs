using BlazorServer.Models;

namespace BlazorWasm.Client.Services;

public interface IClientHotelAmenityService
{
    Task<IEnumerable<HotelAmenityDto>?> GetHotelAmenities();
}